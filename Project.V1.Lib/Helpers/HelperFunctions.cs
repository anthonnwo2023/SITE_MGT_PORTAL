using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Project.V1.Models;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Project.V1.Lib.Helpers
{
    public static class HelperFunctions
    {
        //private static async Task SetState<T>(this IRequestAction<T> serviceAction, T requestClass) where T : RequestBase
        //{
        //    RequestStateBase<T> state = Factory.CreateObject<RequestStateBase<T>, T>(requestClass.RequestStatus, requestClass.RequestType);

        //    await Task.Run(() => serviceAction.SetTransitionState(state));
        //}

        public static string GetTypeName(this string typeAbbr)
        {
            return typeAbbr switch
            {
                "SA" => "Site Acceptance",
                "HS" => "Holistic Site",
                "LS" => "Live Site",
                "EM" => "Equipment Matching",
                "EO" => "Equipment Ordering",
                _ => "",
            };
        }

        public static RequestApproverModel ModelApprover(string appType, string requestId)
        {
            return new RequestApproverModel
            {
                Id = Guid.NewGuid().ToString(),
                RequestId = requestId,
                Username = " ",
                Fullname = " ",
                Email = " ",
                ApproverType = appType,
                DateApproved = DateTime.MinValue,
                DateAssigned = DateTime.MinValue
            };
        }

        public static string RemoveSpecialCharacters(this string str, string[] rogueChars = null)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new("[ ]{2,}", options);
            str = regex.Replace(str, " ");

            string[] roqueCharacters = { "`", "¬", "|", "!", "\"", "\\", "#", "^", "&", "*", "_", "(", ")", "[", "]", "{", "}", "+", "=", ";", ":", "<", ">", "/", "?", "~", " " };

            rogueChars ??= roqueCharacters;

            foreach (string rc in rogueChars)
            {
                str = PerformReplace(str, rc, "_");
            }

            return str;
        }

        private static string PerformReplace(string input, string findChar, string replacChar)
        {
            return input.Replace(findChar, replacChar);
        }


        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
        {
            string a = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}";
            string b = configuration["ElasticConfiguration:Uri"];

            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }

        public static List<SenderBody> ConvertMailStringToList(string vendorMailList)
        {
            var ccs = new List<SenderBody>();

            foreach (var mail in vendorMailList.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ccs.Add(new SenderBody { Name = "", Address = mail });
            }

            return ccs;
        }

        public static Logger GetSerilogLogger()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true)
                .Build();

            string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            string path = Path.Combine(pathBuilt, "log-.txt");

            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .WriteTo.Debug()
                .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment!))
                .WriteTo.Console(new ElasticsearchJsonFormatter())
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Configuration(configuration.GetSection("Serilog"))
                //.WriteTo.File("Logs/log-{Date}.txt", fileSizeLimitBytes: 1_000_000, rollOnFileSizeLimit: true, shared: true, flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.File(path, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}({SourceContext}.{Method}){NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine} {Message:lj}{NewLine}{Exception}",
                                rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 20000000, rollOnFileSizeLimit: true)
                .CreateLogger();
        }

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static string UpperCaseFirst(this string str)
        {
            if (str == null || str.Length == 0)
                return "";

            return str[0].ToString().ToUpper() + str[1..].ToString().ToLower();
        }

        public static string GetAgingDateDiff(this DateTime startDate, DateTime endDate)
        {
            endDate = (endDate == default) ? DateTimeOffset.UtcNow.DateTime.AddHours(1) : endDate;
            if (((DateTimeOffset.UtcNow.DateTime.AddHours(1) - startDate).TotalDays < 0) || ((endDate - startDate).TotalDays < 0))
            {
                return "";
            }

            double hours = (endDate == DateTime.MinValue) ? (DateTimeOffset.UtcNow.DateTime.AddHours(1) - startDate).TotalHours : (endDate - startDate).TotalHours;

            if (hours / 24 > 1)
            {
                return (endDate == DateTime.MinValue) ? $"{(DateTimeOffset.UtcNow.DateTime.AddHours(1) - startDate).TotalHours:0.0}" + " days" : $"{(endDate - startDate).TotalDays:0.0}" + " days";
            }

            return (endDate == DateTime.MinValue) ? $"{(DateTimeOffset.UtcNow.DateTime.AddHours(1) - startDate).TotalHours:0.0}" + " hours" : $"{(endDate - startDate).TotalHours:0.0}" + " hours";
        }

        public static string GetMTTRDateDiff(this DateTime startDate, DateTime endDate)
        {
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                return "";
            }

            double hours = (endDate - startDate).TotalHours;

            if (hours / 24 > 1)
            {
                return $"{(endDate - startDate).TotalDays:0.0}" + " days";
            }

            return $"{(endDate - startDate).TotalHours:0.0}" + " hours";
        }

        public static DateTime Convert2Datetime(this string date)
        {
            CultureInfo CultureI = CultureInfo.CurrentCulture;

            if (CultureI.Name != "en-US")
            {
                date = ConvertDate2USFormart(date);
            }

            return DateTime.Parse(date);
        }

        private static string ConvertDate2USFormart(string date)
        {
            DateTime theDate = DateTime.ParseExact(date, "dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo);

            return theDate.ToString("mm/dd/yyyy");
        }

        public static async Task SendEmailAction<T>(T request, SendEmailActionObj emailObj, string role) where T : class, IDisposable
        {
            CancellationTokenSource cts = new();
            HelperFunctionFactory<T> hff = new(cts);
            var rType = ((dynamic)request).RequestType;

            ((dynamic)request).RequestType = (((dynamic)request).RequestType as string).GetTypeName();
            MailerViewModel<T> mvm = BuildMailObject(request, emailObj, role);
            ((dynamic)request).RequestType = rType;

            await hff.SendRequestMessage(mvm);
        }

        public static async Task SendEmailAction<T>(List<T> requests, SendEmailActionObj emailObj, string role, string requestType) where T : class, IDisposable
        {
            CancellationTokenSource cts = new();
            HelperFunctionFactory<T> hff = new(cts);

            MailerViewModel<T> mvm = BuildMailObject(requests, emailObj, role, requestType);

            await hff.SendRequestMessage(mvm);
        }

        /// <summary>
        /// Gets the 12:00:00 instance of a DateTime
        /// </summary>
        public static DateTime Start(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTime End(this DateTime dateTime)
        {
            return Start(dateTime).AddDays(1).AddTicks(-1);
        }

        public static MailerViewModel<T> BuildMailObject<T>(List<T> mObj, SendEmailActionObj emailObj, string role, string requestType = null) where T : class
        {
            var requests = ((dynamic)mObj) as List<RequestViewModel>;
            var request = requests.FirstOrDefault();

            dynamic mailRequestObject = new ExpandoObject();
            mailRequestObject.BatchNumber = request.BulkBatchNumber;
            mailRequestObject.RequestCount = requests.Count;
            mailRequestObject.RequestType = requestType;
            mailRequestObject.DateCreated = request.DateCreated;
            mailRequestObject.Status = request.Status;
            mailRequestObject.Region = request.Region.Name;

            MailerViewModel<T> mvm = new()
            {
                Name = emailObj.Name,
                MessageTitle = emailObj.Title,
                Greetings = emailObj.Greetings,
                To = emailObj.To,
                CC = emailObj.CC,
                Subject = ($"Site Acceptance Bulk Request ({mailRequestObject.Region}) - {mailRequestObject.BatchNumber} {role} Notice").Replace("  ", " "),
                From = new SenderBody { Name = "SMP Portal", Address = "smp_request@mtn.com" },
                Request = mailRequestObject,
                MailToUsername = emailObj.M2Uname,
                Comment = emailObj.Comment,
                RequestLink = emailObj.Link,
                Attachments = new() { emailObj.Attachment }
            };

            mvm.BulkCreateMailBody = mvm.BulkCreateMailBody;
            mvm.MailBody = mvm.BulkCreateMailBody;

            return mvm;
        }

        public static MailerViewModel<T> BuildMailObject<T>(T mObj, SendEmailActionObj emailObj, string role, string requestType = null) where T : class
        {
            var requests = ((dynamic)mObj) as RequestViewModel;

            MailerViewModel<T> mvm = new()
            {
                Name = emailObj.Name,
                MessageTitle = emailObj.Title,
                Greetings = emailObj.Greetings,
                To = emailObj.To,
                CC = emailObj.CC,
                Subject = ($"Site Acceptance Request ({((dynamic)mObj).Region.Name}) - {((dynamic)mObj).BulkBatchNumber} {role} Notice").Replace("  ", " "),
                From = new SenderBody { Name = "SMP Portal", Address = "smp_request@mtn.com" },
                Request = ((dynamic)mObj),
                MailToUsername = emailObj.M2Uname,
                Comment = emailObj.Comment,
                RequestLink = emailObj.Link,
                RequestType = requests.RequestType.GetTypeName()
            };

            mvm.CreateMailBody = mvm.CreateMailBody;
            mvm.MailBody = mvm.BulkCreateMailBody;

            return mvm;
        }

        public static DateTime ToOffset(this DateTime dt, TimeSpan offset)
        {
            return dt.ToUniversalTime().Add(offset);
        }

        public static string GenerateIDUnique(string RequestAbbr)
        {
            int AlphabetCount = 2;
            string Alphabet = "";
            string prefix = "" + RequestAbbr + "-";

            string date = DateTime.Now.ToString("yyMMddHHmmss");
            //ticks = ticks.Substring(ticks.Length - 9);

            for (int i = 1; i <= AlphabetCount; i++)
            {
                string a = GetLetter();
                Alphabet += a;
            }

            string alias = prefix + date + Alphabet;

            return alias;
        }

        private static readonly Random _random = new();
        public static string GetLetter()
        {

            // This method returns a random lowercase letter.
            // ... Between 'a' and 'z' inclusize.
            int num = RandomNumberGenerator.GetInt32(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let.ToString().ToUpper();
        }
    }

    public class HelperFunctionFactory<T> where T : class, IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly SmtpClient _smtpClient;
        private readonly object _smtpLock = new object();

        public HelperFunctionFactory(CancellationTokenSource cts)
        {
            _cts = cts;
            _smtpClient = new SmtpClient();
        }

        public void Dispose()
        {
            _smtpClient.Disconnect(true, _cts.Token);
            _smtpClient.Dispose();
        }

        private void ConnectSMTP(int i)
        {
            try
            {
                if (!_smtpClient.IsConnected)
                {
                    // Note: only needed if the SMTP server requires authentication
                    //client.Authenticate("no-reply@classicholdingscompany.com", "mz8Ql1!5");
                    lock (_smtpLock)
                        _smtpClient.Connect("172.24.32.68", 25, false, _cts.Token);
                }
            }
            catch (Exception)
            {
                i += 1;

                if (i < 60)
                    ConnectSMTP(i++);
            }
        }

        public async Task<bool> SendRequestMessage(MailerViewModel<T> mailObject)
        {
            if (mailObject == null)
            {
                return false;
            }

            var message = new MimeMessage();

            foreach (SenderBody to in mailObject.To.ToList())
            {
                message.To.Add(new MailboxAddress(to.Name, to.Address));
            }

            foreach (SenderBody cc in mailObject.CC.ToList())
            {
                message.Cc.Add(new MailboxAddress(cc.Name, cc.Address));
            }

            message.From.Add(new MailboxAddress(mailObject.From.Name, mailObject.From.Address));
            message.Subject = mailObject.Subject;

            var builder = new BodyBuilder
            {
                // Set the plain-text version of the message text
                TextBody = @"",

                // In order to reference selfie.jpg from the html text, we'll need to add it
                // to builder.LinkedResources and then use its Content-Id value in the img src.
                //var image = builder.LinkedResources.Add(@"C:\Users\Joey\Documents\Selfies\selfie.jpg");
                //image.ContentId = MimeUtils.GenerateMessageId();

                // Set the html version of the message text
                //            builder.HtmlBody = string.Format(@"<p>Hey Alice,<br>
                //<p>What are you up to this weekend? Monica is throwing one of her parties on
                //Saturday and I was hoping you could make it.<br>
                //<p>Will you be my +1?<br>
                //<p>-- Joey<br>
                //<center><img src=""cid:{0}""></center>", image.ContentId);

                HtmlBody = mailObject.MailBody
            };

            // We may also want to attach a calendar event for Monica's party...
            //builder.Attachments.Add(@"C:\Users\Joey\Documents\party.ics");
            if (mailObject.Attachments != null)
            {
                foreach (string attachment in mailObject.Attachments)
                {
                    builder.Attachments.Add(attachment);
                }
            }

            // Now we just need to set the message body and we're done
            message.Body = builder.ToMessageBody();

            try
            {

                ConnectSMTP(0);

                //await _smtpClient.SendAsync(message, _cts.Token);
                var sendMail = _smtpClient.SendAsync(message, _cts.Token);
                Log.Information($"Mail sent to customer to {string.Join(", ", message.To.Select(x => x.Name))}");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ": " + ex.StackTrace);
                return false;
            }
            finally
            {
                //await _smtpClient.DisconnectAsync(true, _cts.Token);
            }
        }
    }

}
