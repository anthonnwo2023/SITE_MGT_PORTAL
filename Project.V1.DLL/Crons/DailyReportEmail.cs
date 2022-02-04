﻿using Project.V1.DLL.Helpers;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Helpers.HTML.Table;
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
    public class DailyReportEmail : IJob, IDisposable
    {
        private readonly IRequest _request;
        private readonly List<string> tableColumnNames = new() { "S/N", "TECH", "Spectrum", "SiteID", "Region", "Vendor", "Submission Date", "Acceptance Date", "Scope", "State" };
        private readonly string RecipientsCSV = "chinedu.obi@ericsson.com,mtnnocsl@huawei.com,akinola@huawei.com,Joseph.Yakubu@mtn.com,he.jin2@zte.com.cn,Abraham.Uanzekin@mtn.com,Akeem.Alabi@mtn.com,Harold.Obodozie@mtn.com,Olayinka.Esan@mtn.com,Olabode.Aluko@mtn.com,Peter.Okewumi@mtn.com,Oladipo.Bajo@mtn.com,#3G-NOCBackOffice.NG@mtn.com,muhammsal@gmail.com,Nnamdi.Ugochukwu@mtn.com,Babatunde.Ayeni@mtn.com,David.Melaiye@mtn.com,Henry.Obukoadata@mtn.com,lei.yifang@zte.com.cn,Abdul.Ajikobi@mtn.com,#TransmissionBearerServices.NG@mtn.com,#TransmissionAccessPlanningNorth.NG@mtn.com,#TransmissionAccessPlanningWest.NG@mtn.com,#NIDPSOReports.NG@mtn.com,gao.shuang@zte.com.cn,nity.dangwal@zte.com.cn,zengruile@huawei.com,iakhidenor@gmail.com,zhang.yabo111@zte.com.cn,#MTNNigeriaTSSNWG.NG@mtn.com,leey.liyi@huawei.com,#TransmissionAccessPlanningEast.NG@mtn.com,cui.haibo5@zte.com.cn,Chinedu.Ezeigweneme@mtn.com,#RFOptimization.NG@mtn.com,Esther.Igbinakenzua@mtn.com,frederick.kpam@ericsson.com,chido@molcomconcepts.com,stephen.caoguodong@huawei.com,Abayomi.Onafuye@mtn.com,Orieke.Nwosu@mtn.com,femaj2001@yahoo.com,nnamdi.osuji@huawei.com,stephen.seyi.ademoloye@ericsson.com,tosin.adedapo@ericsson.com,adebayo.sulaiman.oshijirin@ericsson.com,Joy.Okpo@mtn.com,Idongesit.Udom@mtn.com,samuel.ola1@huawei.com,irorere.lawrence.osakhienuwa@huawei.com,ejiofor.asogwa@ericsson.com,satish.satish@zte.com.cn,tang.mingxin1@zte.com.cn,Olufunso.Oluwapojuwomi@mtn.com,amah.Jackson@mtn.com,#NIDRAI.NG@mtn.com,tola.daramola@ericsson.com,ragavendra.kumar@ericsson.com,oluwadare.awe@ericsson.com,abimbola.nwankwonta@ericsson.com,eddie.zhangfan@huawei.com,Olayemi.Awofisoye@huawei.com,olawale.aminu@huawei.com,femi.ajayi@zte.com.cn,#NWGUAT_FAT.NG@mtn.com,oghenekevwe.kofi@ericsson.com,kehinde.akingbagbohun@huawei.com,patrick.okaka@huawei.com,ekene.anthony.ibedu@huawei.com,#RFPlanningEngr.NG@mtn.com,uzoma.joenkamuke@huawei.com,Oluwaseun.Onabajo@huawei.com,duqisheng@huawei.com,imoh.umobong@ericsson.com,asuku.aliu.mohammed@ericsson.com,nokia-opt@list.nokia.com,Titilayo.Oguntokun@mtn.com,maxz.chooi@huawei.com,osukoya.ayodele@huawei.com,bola.badie.zaki@huawei.com,alabi.shukrat.mopelola@huawei.com,Young.Omereonye@mtn.com,yakubu.oke.ext@nokia.com,muhammad.t.khan.ext@nokia.com,MohammadReza.Rajabi@mtn.com,oyebode.olumide.temitayo@huawei.com,Rasheed.Bello@mtn.com,Kayode.Olufuwa@mtn.com,Ayodeji.Oni@mtn.com,Peter.Erin@mtn.com,ogunbiyi.timilehin.oladapo@huawei.com,Albert.Chukwuma@mtn.com,augustine.solomon@huawei.com,xue.ningyi@zte.com.cn,hu.shaodong@zte.com.cn,Iyilary@zte.com.cn,wang.huiwen30@zte.com.cn,peng.weidong@zte.com.cn,liu.gang5@zte.com.cn,mohd.zuheb.shakeel@ericsson.com,adeboye.dayo@huawei.com,zhanghaitao11@huawei.com,wangguangxi@huawei.com,pengzhenxing@huawei.com,hazem.amaher@huawei.com,Adeniran.Adepoju@mtn.com,Muhammad.Ashraf@mtn.com,Adeel.Ahmed1@mtn.com,#TxAccessPlanningHQ.NG@mtn.com,idrisolanigan@gmail.com,jesse.obuotor@nokia.com,adebanji.adeyemi@nokia.com,yuguda.hamisu.ext@nokia.com,fehintola.olayemi.ext@nokia.com,waqar.mehmood@nokia.com,chijioke.okoli@nokia.com,huzhili@huawei.com,Tochukwu.Alaekee@mtn.com,#MTNNigeriaTSSNWG.NG@mtn.com,#networkaccessplanning&optimizationhq.ng@mtn.com";
        private Dictionary<string, int> TotalRow = new();
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

        public DailyReportEmail(IRequest request, IVendor vendor, IProjectType projectType)
        {
            _request = request;

            RequestSummary.Initialize(projectType, vendor, request);
        }

        public void Dispose()
        {

        }

        public async Task Execute(IJobExecutionContext context)
        {
            var yesterDay = DateTime.Now.AddDays(-1);
            int lastDayOfMth = DateTime.DaysInMonth(yesterDay.Year, yesterDay.Month);
            var MinDateTime = new DateTime(yesterDay.Year, yesterDay.Month, 1).Date;
            var MaxDateTime = new DateTime(yesterDay.Year, yesterDay.Month, lastDayOfMth).AddDays(1).Date;

            var MonthlyRequests = RequestSummary.GetVendorRequests("Month", yesterDay, MinDateTime, MaxDateTime);
            var DailyRequests = RequestSummary.GetVendorRequests("Day", yesterDay, MinDateTime, MaxDateTime);
            var MonthlyProjectTypeRequests = RequestSummary.GetProjectTypeRequests("Month", yesterDay);

            var VendorMthRequest = MonthlyRequests.GroupBy(x => x.Vendor).ToList();
            var VendorDailyRequest = DailyRequests.GroupBy(x => x.Vendor).ToList();
            var ProjectMthRequest = MonthlyProjectTypeRequests.GroupBy(x => x.ProjectType).ToList();

            var summaryTableHeader = VendorMthRequest.Select(x => x.Select(y => y.Spectrum).Distinct().ToList()).First();

            var table = GenerateSummaryTable(VendorDailyRequest, summaryTableHeader, yesterDay, "vendor", "day");
            table += GenerateSummaryTable(ProjectMthRequest, summaryTableHeader, yesterDay);
            table += GenerateSummaryTable(VendorMthRequest, summaryTableHeader, yesterDay, "vendor");
            table += "<p></p><p></p>";

            var yesterdaysRequests = (await _request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                && x.EngineerAssigned.DateApproved.Date == yesterDay.Date
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

            if (yesterdaysRequests.Any())
            {
                table += $"<p><b><br><br>Sites accepted on {yesterDay:dd.MM.yyyy} are as follows: </b></p>";

                var requestsByFrequency = yesterdaysRequests.GroupBy(x => x.Spectrum).ToList();

                foreach (var request in requestsByFrequency)
                {
                    table += GenerateTable(request, tableColumnNames) + "<br><br>";
                }
            }

            SendNotification(yesterDay, table);
        }

        private async void SendNotification(DateTime yesterDay, string table)
        {
            var mvm = new MailerViewModel<DailyReportEmail>
            {
                Name = $"<p>Dear All,</p>",
                //Greetings = $"<p>Kindly find attached the updated tracker for all accepted sites and their details/summary.</p>"
                Greetings = $"<p>The following sites were accepted as at <strong>{yesterDay:dd.MM.yyyy}</strong>, by MTNN RF.</p>"
            };
            mvm.Greetings += $"<p>For all site details, kindly refer to the SMP Portal. In case of any further queries please feel free to contact MTN RF.</p>";
            mvm.Greetings += $"<p>In {yesterDay:MMMM yyyy}, <b>total of {TotalRow["2G"]} 2G, {TotalRow["3G"]} 3G, {TotalRow["Multi Sector"]} Multi-Sector, {TotalRow["U900"]} U900,  {TotalRow["700M"]} LTE7, {TotalRow["800M"]} LTE8, {TotalRow["1800M"]} LTE18, {TotalRow["2600M"]} LTE26</b> ARE RF ATP-ed:</p>";
            mvm.Greetings += $"<p> &nbsp; </p>";

            var devRecipient = new List<SenderBody>
            {
                new SenderBody { Name = "", Address = "Adekunle.Adeyemi@mtn.com" },
            };

            var recipientList = (ENV == "Development") ? devRecipient : HelperFunctions.ConvertMailStringToList(RecipientsCSV);

            mvm.To = recipientList;
            mvm.CC = new List<SenderBody>
            {
                new SenderBody { Name = "", Address = "Adekunle.Adeyemi@mtn.com" },
                new SenderBody { Name = "", Address = "Anthony.Nwosu@mtn.com" },
                new SenderBody { Name = "", Address = "Kehinde.Ayoola-Oni@mtn.com" },
            };

            mvm.Subject = $"Tuning Acceptance {yesterDay:yyyy-MMMM-dd}";
            mvm.From = new SenderBody { Name = "SMP Portal", Address = "smp_request@mtnnigeria.net" };

            CancellationTokenSource cts = new();
            HelperFunctionFactory<DailyReportEmail> hff = new(cts);

            mvm.Request = table;
            mvm.DailyReportMailBody = mvm.DailyReportMailBody;
            mvm.MailBody = mvm.DailyReportMailBody;

            await hff.SendRequestMessage(mvm);
        }

        private string GenerateSummaryTable(List<IGrouping<string, AcceptanceDTO>> tableData, List<string> tableColumnNames, DateTime date,
            string type = "project", string frequency = "month")
        {
            StringBuilder sb = new();

            using (HTMLTable.Initialize table = new(sb, null, null, TableProperties))
            {
                Dictionary<string, int> totalRow = new();
                table.StartHead();

                using (HTMLTable.Row rowDateHeader = table.AddRow())
                {
                    rowDateHeader.AddCell($"{date:dd/MM/yyyy}", null, null, colSpan: (tableColumnNames.Count + 1).ToString(), CellTDProperties);
                }

                using (HTMLTable.Row rowHeader = table.AddRow())
                {
                    string scenerio = $"Accepted {date.Date:dd/MM/yyyy} (Spectrum by Vendor)";

                    if (type == "project" && frequency == "month")
                    {
                        scenerio = $"Accepted in {date:MMMM} (Spectrum byProject Type) ";
                    }
                    if (type == "vendor" && frequency == "month")
                    {
                        scenerio = $"Accepted in {date:MMMM} (Spectrum by Vendor) ";
                    }

                    rowHeader.AddCell($"<b>Scenerio - {scenerio}</b>", null, null, null, CellTHProperties);
                    foreach (var tData in tableColumnNames)
                    {
                        rowHeader.AddCell($"<b>{tData}</b>", null, null, null, CellTDProperties);
                        totalRow.Add(tData, 0);
                    }
                }
                table.EndHead();

                table.StartBody();

                foreach (var itemByVendor in tableData)
                {
                    using (HTMLTable.Row row = table.AddRow())
                    {
                        if (frequency == "day")
                            row.AddCell($"<b>Accepted Today: ({itemByVendor.Key})</b>", null, null, null, CellTHProperties);
                        else
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

                if (type == "vendor" && frequency == "month")
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