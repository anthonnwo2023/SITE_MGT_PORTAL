using Microsoft.Extensions.Configuration;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Runtime.Versioning;

namespace Project.V1.DLL.Helpers
{
    public class ADHelper
    {
        private static LdapConnection _connection;

        public ADHelper()
        {
        }

        public static string Auth_user(string uname, string pword, string domain)
        {
            try
            {
                _connection = new LdapConnection(domain);
                NetworkCredential credential = new(uname, pword);
                _connection.Credential = credential;
                _connection.Bind();
                return "true";
            }
            catch (LdapException lexc)
            {
                //APP_BLL.WriteLog(uname + ":" + pword + ": " +lexc.StackTrace);
                return lexc.Message;
            }
            catch (Exception exc)
            {
                //APP_BLL.WriteLog("General Login issue with AD");
                return "General Err:" + exc;
            }
        }

        [SupportedOSPlatform("Windows")]
        public static List<Dictionary<string, string>> FindADUsers(string term, IConfiguration configuration)
        {
            List<Dictionary<string, string>> FoundUsers = new();
            string connectionStr = configuration.GetConnectionString("ADConnectionString");

            DirectorySearcher dsearch = new(connectionStr)
            {
                Filter = string.Format("(&(objectCategory=person)(objectClass=user)(mail={0}))", term.ToLower() + "*")
            };

            SearchResultCollection results1 = dsearch.FindAll();

            foreach (SearchResult sResultSet in results1)
            {
                Dictionary<string, string> b = new()
                {
                    { "Id", GetProperty(sResultSet, "sAMAccountName") },
                    { "Name", GetProperty(sResultSet, "cn") },
                    { "Value", GetProperty(sResultSet, "mail") },
                    { "Title", GetProperty(sResultSet, "title") },
                };

                if (!b["Name"].Trim().Equals(string.Empty) && !b["Value"].Trim().Equals(string.Empty))
                {
                    FoundUsers.Add(b);
                }
            }

            return FoundUsers;
        }

        [SupportedOSPlatform("Windows")]
        public static ADUserDomainModel GetADUserData(string username)
        {
            ADUserDomainModel aDUserDomain = new();

            string connection = LoginObject.Configuration.GetConnectionString("ADConnectionString");
            DirectorySearcher dsearch = new(connection)
            {
                Filter = "(sAMAccountName=" + username.ToLower() + ")"
            };

            try
            {
                SearchResult sresult = dsearch.FindOne();

                if (sresult != null)
                {
                    DirectoryEntry dsresult = sresult.GetDirectoryEntry();

                    dsresult.Properties["sn"][0].ToString();
                    foreach (SearchResult sResultSet in dsearch.FindAll())
                    {
                        aDUserDomain.Username = username;
                        // Login Name
                        aDUserDomain.Fullname = GetProperty(sResultSet, "cn"); //Fullname
                                                                               // First Name
                        aDUserDomain.Firstname = GetProperty(sResultSet, "givenName");
                        // Middle Initials
                        aDUserDomain.Middlename = GetProperty(sResultSet, "initials");
                        // Last Name
                        aDUserDomain.Lastname = GetProperty(sResultSet, "sn");
                        // telephonenumber
                        aDUserDomain.PhoneNo = GetProperty(sResultSet, "telephoneNumber");
                        // email address
                        aDUserDomain.Email = GetProperty(sResultSet, "mail");
                        // title
                        aDUserDomain.Title = GetProperty(sResultSet, "title");
                        // department
                        aDUserDomain.Department = GetProperty(sResultSet, "department");
                    }
                }
                else
                {
                    aDUserDomain.ErrorMsg = "The supplied credential is invalid.";
                }

                return aDUserDomain;
            }
            catch
            {
                throw;
            }
        }

        [SupportedOSPlatform("Windows")]
        public static string GetProperty(SearchResult searchResult, string PropertyName)
        {
            if (searchResult.Properties.Contains(PropertyName))
            {
                return (string)searchResult.Properties[PropertyName][0];
            }

            return string.Empty;
        }
    }
}
