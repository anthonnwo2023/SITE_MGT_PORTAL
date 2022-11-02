using Project.V1.DLL.Helpers;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Helpers.HTML.Table;
using Project.V1.Lib.Interfaces;
using Quartz;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Project.V1.DLL.Crons
{
    public class MonthlyReportEmail : IJob, IDisposable
    {
        private readonly IRequest _request;
        private readonly ICLogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly string RecipientsCSV = "Ogonna.Aneke@mtn.com,chinedu.obi@ericsson.com,mtnnocsl@huawei.com,akinola@huawei.com,Joseph.Yakubu@mtn.com,he.jin2@zte.com.cn,Abraham.Uanzekin@mtn.com,Akeem.Alabi@mtn.com,Harold.Obodozie@mtn.com,Olayinka.Esan@mtn.com,Olabode.Aluko@mtn.com,Peter.Okewumi@mtn.com,Oladipo.Bajo@mtn.com,Henry.Obukoadata@mtn.com,lei.yifang@zte.com.cn,#NIDPSOReports.NG@mtn.com,gao.shuang@zte.com.cn,nity.dangwal@zte.com.cn,zengruile@huawei.com,iakhidenor@gmail.com,zhang.yabo111@zte.com.cn,#MTNNigeriaTSSNWG.NG@mtn.com,leey.liyi@huawei.com,cui.haibo5@zte.com.cn,Chinedu.Ezeigweneme@mtn.com,#RFOptimization.NG@mtn.com,Esther.Igbinakenzua@mtn.com,frederick.kpam@ericsson.com,stephen.caoguodong@huawei.com,Abayomi.Onafuye@mtn.com,nnamdi.osuji@huawei.com,stephen.seyi.ademoloye@ericsson.com,tosin.adedapo@ericsson.com,adebayo.sulaiman.oshijirin@ericsson.com,samuel.ola1@huawei.com,irorere.lawrence.osakhienuwa@huawei.com,ejiofor.asogwa@ericsson.com,satish.satish@zte.com.cn,tang.mingxin1@zte.com.cn,Olufunso.Oluwapojuwomi@mtn.com,amah.Jackson@mtn.com,#NIDRAI.NG@mtn.com,tola.daramola@ericsson.com,ragavendra.kumar@ericsson.com,oluwadare.awe@ericsson.com,abimbola.nwankwonta@ericsson.com,eddie.zhangfan@huawei.com,Olayemi.Awofisoye@huawei.com,olawale.aminu@huawei.com,femi.ajayi@zte.com.cn,oghenekevwe.kofi@ericsson.com,kehinde.akingbagbohun@huawei.com,patrick.okaka@huawei.com,ekene.anthony.ibedu@huawei.com,#RFPlanningEngr.NG@mtn.com,uzoma.joenkamuke@huawei.com,Oluwaseun.Onabajo@huawei.com,duqisheng@huawei.com,imoh.umobong@ericsson.com,asuku.aliu.mohammed@ericsson.com,nokia-opt@list.nokia.com,Titilayo.Oguntokun@mtn.com,maxz.chooi@huawei.com,osukoya.ayodele@huawei.com,bola.badie.zaki@huawei.com,alabi.shukrat.mopelola@huawei.com,Young.Omereonye@mtn.com,yakubu.oke.ext@nokia.com,muhammad.t.khan.ext@nokia.com,MohammadReza.Rajabi@mtn.com,oyebode.olumide.temitayo@huawei.com,Rasheed.Bello@mtn.com,Kayode.Olufuwa@mtn.com,Ayodeji.Oni@mtn.com,Peter.Erin@mtn.com,ogunbiyi.timilehin.oladapo@huawei.com,Albert.Chukwuma@mtn.com,augustine.solomon@huawei.com,xue.ningyi@zte.com.cn,hu.shaodong@zte.com.cn,Iyilary@zte.com.cn,wang.huiwen30@zte.com.cn,peng.weidong@zte.com.cn,liu.gang5@zte.com.cn,mohd.zuheb.shakeel@ericsson.com,adeboye.dayo@huawei.com,zhanghaitao11@huawei.com,wangguangxi@huawei.com,pengzhenxing@huawei.com,hazem.amaher@huawei.com,Adeniran.Adepoju@mtn.com,Muhammad.Ashraf@mtn.com,Adeel.Ahmed1@mtn.com,#TxAccessPlanningHQ.NG@mtn.com,jesse.obuotor@nokia.com,adebanji.adeyemi@nokia.com,yuguda.hamisu.ext@nokia.com,fehintola.olayemi.ext@nokia.com,waqar.mehmood@nokia.com,chijioke.okoli@nokia.com,huzhili@huawei.com,Tochukwu.Alaekee@mtn.com,nasiru.hayatu@mtn.com,#networkaccessplanning&optimizationhq.ng@mtn.com";
        //private readonly string RecipientsCSV = "Anthony.Nwosu@mtn.com";

        private readonly List<string> tableColumnNames = new() { "S/N", "TECH", "Spectrum", "SiteID", "Region", "Vendor", "Submission Date", "Acceptance Date", "Scope", "State" };

        private readonly Dictionary<string, string> VendorRecipientsCSV = new()
        {
         { "ERICSSON", "oghenekevwe.kofi@ericsson.com,tola.daramola@ericsson.com,asuku.aliu.mohammed@ericsson.com,adebayo.sulaiman.oshijirin@ericsson.com,chinedu.obi@ericsson.com,esther.igbinakenzua@ericsson.com,emmanuel.ekpendu@ericsson.com,chinyere.tina.ejiofor@ericsson.com,simeon.oladipo@ericsson.com,abiodun.abimbola.kayode@ericsson.com,joseph.ogundiran@ericsson.com,david.aweh@ericsson.com,opeyemi.tokoya.oluwadamilare@ericsson.com,ejiofor.asogwa@ericsson.com,kikelomo.sofolahan@ericsson.com,stephen.seyi.ademoloye@ericsson.com,yusuf.adejumo.salau@ericsson.com,paul.ajayi@ericsson.com,junaid.omotade@ericsson.com,oluwafunmiso.inaolaji@ericsson.com,victoria.nsiamuna@ericsson.com,chiamaka.ohaji@ericsson.com,elmer.ambrose@ericsson.com,adaugo.okezie@ericsson.com,joseph.nwokeafor@ericsson.com,mirian.nnanyere@ericsson.com,lilian.onyedim@ericsson.com,echezona.madu@ericsson.com,john.nnoli@ericsson.com,precious.nwaorgu@ericsson.com,salvation.peter@ericsson.com,ajirioghene.manawa@ericsson.com,sydney.onukwugha@ericsson.com,oluwatoba.abe@ericsson.com,oluwatobi.allen@ericsson.com,frederick.kpam@ericsson.com,damilola.adeyemi@ericsson.com,ifeanyichukwu.oparaeke@ericsson.com" }
         //{ "ERICSSON", "Anthony.Nwosu@mtn.com" }
        
        };
        private Dictionary<string, int> TotalRow = new();
        private readonly Dictionary<string, int> TotalRowInit = new()
        {
            { "2G", 0 },
            { "3G", 0 },
            { "U900", 0 },
            { "Multi Sector", 0 },
            { "700M", 0 },
            { "800M", 0 },
            { "1800M", 0 },
            { "2600M", 0 },
            { "N3500", 0 }


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
            GC.SuppressFinalize(this);
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


                var summaryTableHeader = TotalRowInit.Select(x => x.Key).Distinct().ToList();
                var MonthlyRequests = RequestSummary.GetVendorRequests(MinDateTime, MaxDateTime);
                var MonthlyProjectTypeRequests = RequestSummary.GetProjectTypeRequests(MinDateTime, MaxDateTime);

                var AllVendorMthRequest = MonthlyRequests.GroupBy(x => x.Vendor).ToList();
                var AllProjectMthRequest = MonthlyProjectTypeRequests.GroupBy(x => x.ProjectType).ToList();

                var tableMonthlyAll = GenerateSummaryTable(AllProjectMthRequest, summaryTableHeader, yesterDay);
                tableMonthlyAll += GenerateSummaryTable(AllVendorMthRequest, summaryTableHeader, yesterDay, "vendor");

                foreach (var region in regions)
                {
                    var engineers = regionUsers.Where(x => x.Key.Contains(region)).SelectMany(x => x.ToList()).Select(x => x.Email);
                    var engineerRecipientCSV = string.Join(",", engineers);
                    var VendorMthRequest = MonthlyRequests.Where(x => x.Region == region).GroupBy(x => x.Vendor).ToList();
                    var ProjectMthRequest = MonthlyProjectTypeRequests.Where(x => x.Region == region).GroupBy(x => x.ProjectType).ToList();

                    var table = GenerateSummaryTable(ProjectMthRequest, summaryTableHeader, yesterDay);
                    table += GenerateSummaryTable(VendorMthRequest, summaryTableHeader, yesterDay, "vendor");
                    table += "<p></p><p></p>";

                    try
                    {
                        var regionMonthRequests = (await _request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                        && x.EngineerAssigned.DateApproved.Date >= MinDateTime.Date && x.EngineerAssigned.DateApproved.Date < MaxDateTime.Date
                                        //&& !x.Spectrum.Name.Contains("MOD")
                                        && x.Region.Name.ToUpper() == region.ToUpper() && x.Status == "Accepted"
                                        //&& x.ProjectType.Name != "Layer Expansion" && x.ProjectType.Name != "Small Cell"
                                        , null, "EngineerAssigned,Region,Requester.Vendor,ProjectType,TechType,Spectrum"))
                                         .Select(x => new AcceptanceDTO
                                         {
                                             SiteId = x.SiteId,
                                             Region = x.Region.Name,
                                             State = x.State,
                                             Vendor = x.Requester.Vendor.Name,
                                             ProjectType = RequestSummary.GetProjectTypeName(x.ProjectType.Name),
                                             ProjectTypeName = x.ProjectType.Name,
                                             TechType = x.TechType.Name,
                                             Spectrum = x.Spectrum.Name,
                                             AcceptanceCount = 1,

                                             // DateSubmitted = x.DateSubmitted
                                                DateSubmitted = DateTimeOffset.ParseExact(x.DateSubmitted.ToString("dd/MM/yyyy HH:mm:ss zzz"), "dd/MM/yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture)

                                                ,
                                             // DateAccepted = x.EngineerAssigned.DateApproved
                                                DateAccepted = DateTimeOffset.ParseExact(x.EngineerAssigned.DateApproved.ToString("dd/MM/yyyy HH:mm:ss zzz"), "dd/MM/yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture)


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

                      // SendNotification(yesterDay, table, "anthony.nwosu@mtn.com", $"{region} Monthly: Tuning Acceptance {yesterDay:yyyy-MMMM}");

                        SendNotification(yesterDay, table, engineerRecipientCSV, $"{region} Monthly: Tuning Acceptance {yesterDay:yyyy-MMMM}");
                    }
                    catch (Exception ex)
                    {
                        string errEX = ex.Message;
                        string errStack = ex.StackTrace;
                    }                                

                }

                ///
                try
                {
                    var lastMonthRequests = (await _request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                    && x.EngineerAssigned.DateApproved.Date >= MinDateTime.Date && x.EngineerAssigned.DateApproved.Date < MaxDateTime.Date
                                    && !x.Spectrum.Name.Contains("MOD")
                                    && x.ProjectType.Name != "Layer Expansion" && x.ProjectType.Name != "Small Cell", null, "EngineerAssigned,Region,Requester.Vendor,ProjectType,TechType,Spectrum"))
                                     .Select(x => new AcceptanceDTO
                                     {
                                         SiteId = x.SiteId,
                                         Region = x.Region.Name,
                                         State = x.State,
                                         Vendor = x.Requester.Vendor.Name,
                                         ProjectType = RequestSummary.GetProjectTypeName(x.ProjectType.Name),
                                         ProjectTypeName = x.ProjectType.Name,
                                         TechType = x.TechType.Name,
                                         Spectrum = x.Spectrum.Name,
                                         AcceptanceCount = 1,
                                                                                    
                                        // DateSubmitted = x.DateSubmitted,

                                         DateSubmitted = DateTimeOffset.ParseExact(x.DateSubmitted.ToString("dd/MM/yyyy HH:mm:ss zzz"), "dd/MM/yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture)                                                                                
                                        ,
                                         // DateAccepted = x.EngineerAssigned.DateApproved
                                         DateAccepted = DateTimeOffset.ParseExact(x.EngineerAssigned.DateApproved.ToString("dd/MM/yyyy HH:mm:ss zzz"), "dd/MM/yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture)



                                     }).ToList();

                    SendNotification(yesterDay, tableMonthlyAll, RecipientsCSV, $"Tuning Acceptance {yesterDay:yyyy-MMMM}");
                    SendVendorSpecificAccecptanceNotification(yesterDay, lastMonthRequests);

                }
                catch (Exception ex)
                {
                    string errEX2 = ex.Message;
                    string errStack2 = ex.StackTrace;
                }


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
                mvm.Greetings += $"<p>In {yesterDay:MMMM, yyyy}, <b>total of {TotalRow["2G"]} 2G, {TotalRow["3G"]} 3G, {TotalRow["Multi Sector"]} Multi-Sector, {TotalRow["U900"]} U900,  {TotalRow["700M"]} LTE7, {TotalRow["800M"]} LTE8, {TotalRow["1800M"]} LTE18, {TotalRow["2600M"]} LTE26 , {TotalRow["N3500"]} N3500 </b> ARE RF ATP-ed:</p>";
                mvm.Greetings += $"<p> &nbsp; </p>";
            }

            var devRecipient = new List<SenderBody>
            {
                new SenderBody { Name = "", Address = "Anthony.Nwosu@mtn.com" },
            };

            var recipientList = (ENV == "Development") ? devRecipient : HelperFunctions.ConvertMailStringToList(recipients);

            mvm.To = recipientList;
            mvm.CC = new List<SenderBody>
            {
                //new SenderBody { Name = "", Address = "Adekunle.Adeyemi@mtn.com" },
                new SenderBody { Name = "", Address = "Kehinde.Ayoola-Oni@mtn.com" },
                new SenderBody { Name = "", Address = "Anthony.Nwosu@mtn.com" },
               
            };

            mvm.Subject = subject;
            mvm.From = new SenderBody { Name = "SMP Portal", Address = "smp_request@mtn.com" };

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
                    { "2600M", 0 },
                    { "N3500", 0 }
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
                        rowBody.AddCell((request.DateAccepted.HasValue) ? request.DateAccepted.Value.ToString("d") : null, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.ProjectTypeName, null, null, null, CellTDProperties);
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
