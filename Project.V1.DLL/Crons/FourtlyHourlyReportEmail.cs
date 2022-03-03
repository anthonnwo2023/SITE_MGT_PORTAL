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
    public class FourtlyHourlyReportEmail : IJob, IDisposable
    {
        private readonly IRequest _request;
        private readonly ICLogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly List<string> tableColumnNames = new() { "S/N", "TECH", "Spectrum", "SiteID", "Region", "Vendor", "Submission Date", "Acceptance Date", "Scope", "Status", "State" };

        private readonly Dictionary<string, int> TotalRow = new();
        private readonly Dictionary<string, int> TotalRowInit = new()
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

        private readonly List<string> Statuses = new()
        {
            "Pending",
            "Reworked",
            "Restarted"
        };

        public FourtlyHourlyReportEmail(IRequest request, IVendor vendor, IProjectType projectType,
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
                var table = string.Empty;
                var engineerRecipientCSV = string.Empty;

                var regionUsers = (await _userManager.GetUsersInRoleAsync("Engineer"))
                    .GroupBy(x => x.Regions.Select(y => y.Name)).ToList();

                var regions = regionUsers.SelectMany(x => x.Key).Distinct().ToList();

                var summaryTableHeader = TotalRowInit.Select(x => x.Key).Distinct().ToList();

                var regionMonthRequests = (await _request.Get(x => !string.IsNullOrEmpty(x.EngineerAssigned.Fullname.Trim())
                                        && Statuses.Contains(x.Status)))
                                         .Select(x => new AcceptanceDTO
                                         {
                                             SiteId = x.SiteId,
                                             Region = x.Region.Name,
                                             State = x.State,
                                             Status = x.Status,
                                             Vendor = x.Requester.Vendor.Name,
                                             ProjectType = RequestSummary.GetProjectTypeName(x.ProjectType.Name),
                                             ProjectTypeName = x.ProjectType.Name,
                                             TechType = x.TechType.Name,
                                             Spectrum = x.Spectrum.Name,
                                             AcceptanceCount = 1,
                                             DateSubmitted = x.DateSubmitted,
                                             DateAccepted = x.EngineerAssigned.DateApproved
                                         }).ToList();

                if (regionMonthRequests.Any())
                {
                    var requestsByRegion = regionMonthRequests.GroupBy(x => x.Region).ToList();

                    foreach (var requests in requestsByRegion)
                    {
                        var engineers = regionUsers.Where(x => x.Key.Contains(requests.Key)).SelectMany(x => x.ToList()).Select(x => x.Email);
                        engineerRecipientCSV += string.Join(",", engineers);

                        table += $"<p><b><br><br>Sites awaiting acceptance as at {DateTime.UtcNow:dd MMMM,yyyy H:mm} in {requests.Key} region are as follows: </b></p>";

                        table += GenerateTable(requests, tableColumnNames) + "<br><br>";
                    }
                }


                SendNotification(table, engineerRecipientCSV, $"Awaiting Acceptance as at {DateTime.UtcNow:dd MMMM, yyyy HH:mm}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, new { }, ex);
            }
        }

        private async void SendNotification(string table, string recipients, string subject)
        {
            var mvm = new MailerViewModel<DailyReportEmail>
            {
                Name = $"<p>Dear All,</p>",
                //Greetings = $"<p>Kindly find attached the updated tracker for all accepted sites and their details/summary.</p>"
                Greetings = $"<p>The following sites are waiting to be accepted by MTNN RF.</p>"
            };
            mvm.Greetings += $"<p>For all site details, kindly refer to the <a href='https://ojtssapp1/smp/' target='_blank'>SMP Portal</a>. In case of any further queries please feel free to contact MTN RF.</p>";

            var devRecipient = new List<SenderBody>
            {
                new SenderBody { Name = "", Address = "Adekunle.Adeyemi@mtn.com" },
            };
            var devCCRecipient = new List<SenderBody>
            {
                new SenderBody { Name = "", Address = "Adekunle.Adeyemi@mtn.com" },
                new SenderBody { Name = "", Address = "Anthony.Nwosu@mtn.com" },
                new SenderBody { Name = "", Address = "Kehinde.Ayoola-Oni@mtn.com" },
            };

            var recipientList = (ENV == "Development") ? devRecipient : HelperFunctions.ConvertMailStringToList(recipients);
            var recipientCCList = (ENV == "Development") ? devCCRecipient : HelperFunctions.ConvertMailStringToList("#RFPlanningEngr.NG@mtn.com");

            mvm.To = recipientList;
            mvm.CC = recipientCCList;

            mvm.Subject = subject;
            mvm.From = new SenderBody { Name = "SMP Portal", Address = "smp_request@mtn.com" };

            CancellationTokenSource cts = new();
            HelperFunctionFactory<DailyReportEmail> hff = new(cts);

            mvm.Request = table;
            mvm.DailyReportMailBody = mvm.DailyReportMailBody;
            mvm.MailBody = mvm.DailyReportMailBody;

            await hff.SendRequestMessage(mvm);
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
                        rowBody.AddCell($"{request.DateSubmitted:dd.MM.yyyy HH:mm:ss}", null, null, null, CellTDProperties);
                        rowBody.AddCell(((request.DateAccepted.HasValue) && request.DateAccepted != DateTime.MinValue) ? request.DateAccepted.Value.ToString("d") : null, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.ProjectTypeName, null, null, null, CellTDProperties);
                        rowBody.AddCell(request.Status.Trim(), null, null, null, CellTDProperties);
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
