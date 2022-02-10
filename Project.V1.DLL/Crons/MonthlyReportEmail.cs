using Microsoft.AspNetCore.Identity;
using Project.V1.DLL.Helpers;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Helpers.HTML.Table;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project.V1.DLL.Crons
{
    public class MonthlyReportEmail : IJob, IDisposable
    {
        private readonly IRequest _request;
        private readonly ICLogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly List<string> tableColumnNames = new() { "S/N", "TECH", "Spectrum", "SiteID", "Region", "Vendor", "Submission Date", "Acceptance Date", "Scope", "State" };

        private readonly Dictionary<string, string> VendorRecipientsCSV = new()
        {
            { "ERICSSON", "emmanuel.idoko@mtn.com,oghenekevwe.kofi@ericsson.com,tola.daramola@ericsson.com,asuku.aliu.mohammed@ericsson.com,adebayo.sulaiman.oshijirin@ericsson.com,chinedu.obi@ericsson.com,esther.igbinakenzua@ericsson.com,emmanuel.ekpendu@ericsson.com,chinyere.tina.ejiofor@ericsson.com,simeon.oladipo@ericsson.com,abiodun.abimbola.kayode@ericsson.com,joseph.ogundiran@ericsson.com,david.aweh@ericsson.com,opeyemi.tokoya.oluwadamilare@ericsson.com,ejiofor.asogwa@ericsson.com,kikelomo.sofolahan@ericsson.com,stephen.seyi.ademoloye@ericsson.com,yusuf.adejumo.salau@ericsson.com,paul.ajayi@ericsson.com,junaid.omotade@ericsson.com,oluwafunmiso.inaolaji@ericsson.com,victoria.nsiamuna@ericsson.com,chiamaka.ohaji@ericsson.com,elmer.ambrose@ericsson.com,adaugo.okezie@ericsson.com,joseph.nwokeafor@ericsson.com,mirian.nnanyere@ericsson.com,lilian.onyedim@ericsson.com,echezona.madu@ericsson.com,john.nnoli@ericsson.com,precious.nwaorgu@ericsson.com,salvation.peter@ericsson.com,ajirioghene.manawa@ericsson.com,sydney.onukwugha@ericsson.com,oluwatoba.abe@ericsson.com,oluwatobi.allen@ericsson.com,frederick.kpam@ericsson.com,damilola.adeyemi@ericsson.com,ifeanyichukwu.oparaeke@ericsson.com" }
        };
        private Dictionary<string, int> TotalRow = new();
        private Dictionary<string, int> TotalRowInit = new()
        {
            { "2G", 0 },
            { "3G", 0 },
            { "U900", 0 },
            { "Multi Sector", 0 },
            { "700M", 0 },
            { "800M", 0 },
            { "1800M", 0 },
            { "2600M", 0 }
        };
        private readonly string ENV = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private readonly Dictionary<string, string> TableProperties = new()
        {
            { "width", "100%" },
            { "border", "1" },
            { "cellspacing", "2" },
            { "cellpadding", "2" },
        };

        private readonly Dictionary<string, string> CellTHProperties = new()
        {
            { "align", "left" },
            { "valign", "middle" },
        };

        private readonly Dictionary<string, string> CellTDProperties = new()
        {
            { "align", "center" },
            { "valign", "middle" },
        };

        public MonthlyReportEmail(IRequest request, IVendor vendor, IProjectType projectType,
            ICLogger logger, UserManager<ApplicationUser> userManager)
        {
            _request = request;
            _logger = logger;
            _userManager = userManager;
            RequestSummary.Initialize(projectType, vendor, request);
        }

        public void Dispose()
        {

        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var yesterDay = DateTime.Now.AddDays(-1);
                int lastDayOfMth = DateTime.DaysInMonth(yesterDay.Year, yesterDay.Month);
                var MinDateTime = new DateTime(yesterDay.Year, yesterDay.Month, 1).Date;
                var MaxDateTime = new DateTime(yesterDay.Year, yesterDay.Month, lastDayOfMth).AddDays(1).Date;

                var regionUsers = (await _userManager.GetUsersInRoleAsync("Engineer"))
                    .GroupBy(x => x.Regions.Select(y => y.Name)).ToList();

                var regions = regionUsers.SelectMany(x => x.Key).Distinct().ToList();

                var MonthlyRequests = RequestSummary.GetVendorRequests("Month", yesterDay, MinDateTime, MaxDateTime);
                var MonthlyProjectTypeRequests = RequestSummary.GetProjectTypeRequests("Month", yesterDay);

                foreach (var region in regions)
                {
                    var engineers = regionUsers.Where(x => x.Key.Contains(region)).SelectMany(x => x.ToList()).Select(x => x.Email);
                    var engineerRecipientCSV = string.Join(",", engineers);

                    var summaryTableHeader = TotalRowInit.Select(x => x.Key).Distinct().ToList();
                    var VendorMthRequest = MonthlyRequests.Where(x => x.Region == region).GroupBy(x => x.Vendor).ToList();
                    var ProjectMthRequest = MonthlyProjectTypeRequests.Where(x => x.Region == region).GroupBy(x => x.ProjectType).ToList();

                    var table = GenerateSummaryTable(ProjectMthRequest, summaryTableHeader, yesterDay);
                    table += GenerateSummaryTable(VendorMthRequest, summaryTableHeader, yesterDay, "vendor");
                    table += "<p></p><p></p>";

                    var regionMonthRequests = (await _request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                        && x.EngineerAssigned.DateApproved.Date >= MinDateTime.Date && x.EngineerAssigned.DateApproved.Date < MaxDateTime.Date
                                        && !x.Spectrum.Name.Contains("MOD")
                                        && x.Region.Name.ToUpper() == region.ToUpper()
                                        && x.ProjectType.Name != "Layer Expansion" && x.ProjectType.Name != "Small Cell"))
                                         .Select(x => new AcceptanceDTO
                                         {
                                             SiteId = x.SiteId,
                                             Region = x.Region.Name,
                                             State = x.State,
                                             Vendor = x.Requester.Vendor.Name,
                                             ProjectType = RequestSummary.GetProjectTypeName(x.ProjectType.Name),
                                             TechType = x.TechType.Name,
                                             Spectrum = x.Spectrum.Name,
                                             AcceptanceCount = 1,
                                             DateSubmitted = x.DateSubmitted,
                                             DateAccepted = x.EngineerAssigned.DateApproved
                                         }).ToList();

                    if (regionMonthRequests.Any())
                    {
                        table += $"<p><b><br><br>Sites accepted on {yesterDay:MMMM.yyyy} in {region} region are as follows: </b></p>";

                        var requestsByFrequency = regionMonthRequests.GroupBy(x => x.Spectrum).ToList();

                        foreach (var request in requestsByFrequency)
                        {
                            table += GenerateTable(request, tableColumnNames) + "<br><br>";
                        }
                    }

                    SendNotification(yesterDay, table, engineerRecipientCSV, $"{region} Monthly: Tuning Acceptance {yesterDay:yyyy-MMMM}");
                }

                var lastMonthRequests = (await _request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                    && x.EngineerAssigned.DateApproved.Date >= MinDateTime.Date && x.EngineerAssigned.DateApproved.Date < MaxDateTime.Date
                                    && !x.Spectrum.Name.Contains("MOD")
                                    && x.ProjectType.Name != "Layer Expansion" && x.ProjectType.Name != "Small Cell"))
                                     .Select(x => new AcceptanceDTO
                                     {
                                         SiteId = x.SiteId,
                                         Region = x.Region.Name,
                                         State = x.State,
                                         Vendor = x.Requester.Vendor.Name,
                                         ProjectType = RequestSummary.GetProjectTypeName(x.ProjectType.Name),
                                         TechType = x.TechType.Name,
                                         Spectrum = x.Spectrum.Name,
                                         AcceptanceCount = 1,
                                         DateSubmitted = x.DateSubmitted,
                                         DateAccepted = x.EngineerAssigned.DateApproved
                                     }).ToList();

                SendVendorSpecificAccecptanceNotification(yesterDay, lastMonthRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
            }
        }

        private void SendVendorSpecificAccecptanceNotification(DateTime yesterDay, List<AcceptanceDTO> yesterdaysRequests)
        {
            var vendorYesterdayData = yesterdaysRequests.GroupBy(x => x.Vendor.ToUpper()).ToList();

            foreach (var vendorData in vendorYesterdayData)
            {
                var vendorTables = string.Empty;

                if (vendorData.Key.ToUpper() == "ERICSSON" && vendorData.Any())
                {
                    vendorTables += $"<p><b><br><br>{vendorData.Key} Sites accepted on {yesterDay:MMMM.yyyy} are as follows: </b></p>";

                    var vendorRequestsByFrequency = vendorData.GroupBy(x => x.Spectrum).ToList();

                    foreach (var request in vendorRequestsByFrequency)
                    {
                        vendorTables += GenerateTable(request, tableColumnNames) + "<br><br>";
                    }

                    SendNotification(yesterDay, vendorTables, VendorRecipientsCSV[vendorData.Key.ToUpper()], $"{vendorData.Key}: Tuning Acceptance {yesterDay:yyyy-MMMM}", isGeneral: false);
                }
            }
        }

        private async void SendNotification(DateTime yesterDay, string table, string recipients, string subject, bool isGeneral = true)
        {
            var mvm = new MailerViewModel<DailyReportEmail>
            {
                Name = $"<p>Dear All,</p>",
                //Greetings = $"<p>Kindly find attached the updated tracker for all accepted sites and their details/summary.</p>"
                Greetings = $"<p>The following sites were accepted as at <strong>{yesterDay:MMMM, yyyy}</strong>, by MTNN RF.</p>"
            };
            mvm.Greetings += $"<p>For all site details, kindly refer to the <a href='https://ojtssapp1/smp/' target='_blank'>SMP Portal</a>. In case of any further queries please feel free to contact MTN RF.</p>";

            if (isGeneral)
            {
                mvm.Greetings += $"<p>In {yesterDay:MMMM, yyyy}, <b>total of {TotalRow["2G"]} 2G, {TotalRow["3G"]} 3G, {TotalRow["Multi Sector"]} Multi-Sector, {TotalRow["U900"]} U900,  {TotalRow["700M"]} LTE7, {TotalRow["800M"]} LTE8, {TotalRow["1800M"]} LTE18, {TotalRow["2600M"]} LTE26</b> ARE RF ATP-ed:</p>";
                mvm.Greetings += $"<p> &nbsp; </p>";
            }

            var devRecipient = new List<SenderBody>
            {
                new SenderBody { Name = "", Address = "Adekunle.Adeyemi@mtn.com" },
            };

            var recipientList = (ENV == "Development") ? devRecipient : HelperFunctions.ConvertMailStringToList(recipients);

            mvm.To = recipientList;
            mvm.CC = new List<SenderBody>
            {
                new SenderBody { Name = "", Address = "Adekunle.Adeyemi@mtn.com" },
                new SenderBody { Name = "", Address = "Anthony.Nwosu@mtn.com" },
                new SenderBody { Name = "", Address = "Kehinde.Ayoola-Oni@mtn.com" },
            };

            mvm.Subject = subject;
            mvm.From = new SenderBody { Name = "SMP Portal", Address = "smp_request@mtnnigeria.net" };

            CancellationTokenSource cts = new();
            HelperFunctionFactory<DailyReportEmail> hff = new(cts);

            mvm.Request = table;
            mvm.DailyReportMailBody = mvm.DailyReportMailBody;
            mvm.MailBody = mvm.DailyReportMailBody;

            await hff.SendRequestMessage(mvm);
        }

        private string GenerateSummaryTable(List<IGrouping<string, AcceptanceDTO>> tableData, List<string> tableColumnNames, DateTime date,
            string type = "project")
        {
            StringBuilder sb = new();

            using (HTMLTable.Initialize table = new(sb, null, null, TableProperties))
            {
                Dictionary<string, int> totalRow = new()
                {
                    { "2G", 0 },
                    { "3G", 0 },
                    { "U900", 0 },
                    { "Multi Sector", 0 },
                    { "700M", 0 },
                    { "800M", 0 },
                    { "1800M", 0 },
                    { "2600M", 0 }
                };

                table.StartHead();

                using (HTMLTable.Row rowDateHeader = table.AddRow())
                {
                    rowDateHeader.AddCell($"{date:MMMM, yyyy}", null, null, colSpan: (tableColumnNames.Count + 1).ToString(), CellTDProperties);
                }

                using (HTMLTable.Row rowHeader = table.AddRow())
                {
                    string scenerio = $"Accepted {date.Date:MM.yyyy} (Spectrum by Vendor)";

                    if (type == "project")
                    {
                        scenerio = $"Accepted in {date:MMMM} (Spectrum by Project Type) ";
                    }
                    if (type == "vendor")
                    {
                        scenerio = $"Accepted in {date:MMMM} (Spectrum by Vendor) ";
                    }

                    rowHeader.AddCell($"<b>Scenerio - {scenerio}</b>", null, null, null, CellTHProperties);
                    foreach (var tData in tableColumnNames)
                    {
                        rowHeader.AddCell($"<b>{tData}</b>", null, null, null, CellTDProperties);

                        if (totalRow.ContainsKey(tData))
                            totalRow[tData] += 0;
                        else
                            totalRow.Add(tData, 0);
                    }
                }
                table.EndHead();

                table.StartBody();

                foreach (var itemByVendor in tableData)
                {
                    using (HTMLTable.Row row = table.AddRow())
                    {
                        if (type == "project")
                            row.AddCell($"<b>{itemByVendor.Key} so far in {date:MMMM}</b>", null, null, null, CellTHProperties);
                        else
                            row.AddCell($"<b>Accepted so far in {date:MMMM}: ({itemByVendor.Key})</b>", null, null, null, CellTHProperties);

                        foreach (var name in tableColumnNames)
                        {
                            var spectSum = 0;
                            var spectrumData = itemByVendor.Where(x => x.Spectrum?.ToUpper() == name.ToUpper()).ToList();


                            if (spectrumData.Any() || name.ToUpper().EndsWith("PHY"))
                            {
                                if (name.ToUpper() == "3G PHY")
                                {
                                    spectSum = itemByVendor.Sum(x => x.UMTSPhyCount);
                                    row.AddCell(spectSum.ToString(), null, null, null, CellTDProperties);
                                    totalRow[name] += spectSum;
                                    continue;
                                }

                                if (name.ToUpper() == "4G PHY")
                                {
                                    spectSum = itemByVendor.Sum(x => x.LTEPhyCount);
                                    row.AddCell(spectSum.ToString(), null, null, null, CellTDProperties);
                                    totalRow[name] += spectSum;
                                    continue;
                                }

                                if (type == "vendor")
                                {
                                    spectSum = spectrumData.Where(x => x.Vendor.ToUpper() == itemByVendor.Key.ToUpper()).Sum(x => x.AcceptanceCount);
                                    totalRow[name] += spectSum;
                                    row.AddCell(spectSum.ToString(), null, null, null, CellTDProperties);
                                }
                                else
                                    row.AddCell(spectrumData.Where(x => x.ProjectType.ToUpper() == itemByVendor.Key.ToUpper()).Sum(x => x.AcceptanceCount).ToString(), null, null, null, CellTDProperties);

                            }
                            else
                                row.AddCell("0", null, null, null, CellTDProperties);
                        }
                    }
                }

                if (type == "vendor")
                    using (HTMLTable.Row row = table.AddRow())
                    {
                        row.AddCell("<b>Monthly Total</b>", "strong", null, null, CellTHProperties);

                        foreach (var colName in tableColumnNames)
                        {
                            row.AddCell(totalRow[colName].ToString(), null, null, null, CellTDProperties);
                        }
                    }
                TotalRow = totalRow;
                table.EndBody();
            }

            return sb.ToString();
        }

        private string GenerateTable(IGrouping<string, AcceptanceDTO> tableData, List<string> tableColumnNames)
        {
            StringBuilder sb = new();
            var sn = 1;

            using (HTMLTable.Initialize table = new(sb, null, null, TableProperties))
            {
                table.StartHead();
                using (HTMLTable.Row rowHeader = table.AddRow())
                {
                    foreach (var tData in tableColumnNames)
                    {
                        rowHeader.AddCell($"<b>{tData}</b>", null, null, null, CellTDProperties);
                    }
                }
                table.EndHead();

                table.StartBody();
                foreach (var request in tableData)
                {
                    using (HTMLTable.Row rowBody = table.AddRow())
                    {
                        rowBody.AddCell(sn.ToString(), null, null, null, CellTDProperties);
                        rowBody.AddCell(request.TechType, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.Spectrum, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.SiteId, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.Region, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.Vendor, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.DateSubmitted.ToString("d"), null, null, null, CellTDProperties);
                        rowBody.AddCell(request.DateAccepted.ToString("d"), null, null, null, CellTDProperties);
                        rowBody.AddCell(request.ProjectType, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.State.Trim(), null, null, null, CellTDProperties);
                    }

                    sn++;
                }
                table.EndBody();
            }

            return sb.ToString();
        }
    }
}
