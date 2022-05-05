using ExcelDataReader;
using Project.V1.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Project.V1.Lib.Helpers.Excel
{
    public static class ExcelProcessor
    {
        public static (System.Data.DataTable DT, ExcelTransactionError Error) ToDataTable<T>(T RequestObj, string excelExcelFilePath, string userFullname) where T : class
        {
            ExcelTransactionError ete = new();
            System.Data.DataTable dt = new();

            try
            {

                if (!ValidateExcelDocument(RequestObj, excelExcelFilePath))
                {
                    ete = new ExcelTransactionError
                    {
                        ErrorType = "Invalid Document Type",
                        ErrorDesc = "Uploaded file type not supported. Please upload only .xls and .xlsx files.",
                        CreatedBy = userFullname,
                        DateCreated = DateTimeOffset.UtcNow.DateTime
                    };

                    return (dt, ete);
                }

                // System.Text.Encoding.CodePages
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using FileStream fileStream = new(excelExcelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using StreamReader streamReader = new(fileStream, Encoding.UTF8);
                using IExcelDataReader reader = ExcelReaderFactory.CreateReader(fileStream);

                dt = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true,
                        EmptyColumnNamePrefix = "Column"
                    }
                }).Tables[0];

                if (!IsAllowedHeaders(((dynamic)RequestObj).Headers, GetDataTableHeaders(dt)))
                {
                    dt = new System.Data.DataTable();
                    ete = new ExcelTransactionError
                    {
                        ErrorType = "Excel Header Mismatch",
                        ErrorDesc = "The column headers in the excel does not match the expected columns",
                        CreatedBy = userFullname,
                        DateCreated = DateTimeOffset.UtcNow.DateTime
                    };

                    return (dt, ete);
                }

                ete = new ExcelTransactionError
                {
                    ErrorType = "",
                    ErrorDesc = "All good!",
                    CreatedBy = userFullname,
                    DateCreated = DateTimeOffset.UtcNow.DateTime
                };

                dt.ConvertColumnTypeTo("Site Id", raw => raw.ToString());
                dt.ConvertColumnTypeTo("Longitude", raw => Convert.ToDouble(raw));
                dt.ConvertColumnTypeTo("Latitude", raw => Convert.ToDouble(raw));
                dt.ConvertColumnTypeTo("Antenna Height", raw => raw.ToString());
                dt.ConvertColumnTypeTo("Tower Height - (M)", raw => Convert.ToDouble(raw));
                dt.ConvertColumnTypeTo("State", raw => raw.ToString()?.UpperCaseFirst());
                dt.ConvertColumnTypeTo("Antenna Azimuth", raw => raw.ToString());
                dt.ConvertColumnTypeTo("Power - (w)", raw => raw.ToString());
                dt.ConvertColumnTypeTo("RRU Power - (w)", raw => raw.ToString());
                dt.ConvertColumnTypeTo("Bandwidth (MHz)", raw => raw.ToString());
                dt.ConvertColumnTypeTo("M Tilt", raw => raw.ToString());
                dt.ConvertColumnTypeTo("E Tilt", raw => raw.ToString());
                dt.ConvertColumnTypeTo("Summer Config", raw => raw.ToString());
                dt.ConvertColumnTypeTo("Software", raw => raw.ToString());
                dt.ConvertColumnTypeTo("Project Year", raw => Convert.ToDouble(raw));
                dt.ConvertColumnTypeTo("Integrated Date", raw => Convert.ToDateTime(raw.ToString()));

                return (dt, ete);
            }
            catch (Exception ex)
            {
                dt = new System.Data.DataTable();
                ete = new ExcelTransactionError
                {
                    ErrorType = "Invalid Datatype!",
                    ErrorDesc = ex.Message,
                    CreatedBy = userFullname,
                    DateCreated = DateTimeOffset.UtcNow.DateTime
                };

                return (dt, ete);
            }
        }

        private static ExcelDataSetConfiguration GetExcelDataSetConfig()
        {
            return new ExcelDataSetConfiguration()
            {
                // Gets or sets a value indicating whether to set the DataColumn.DataType 
                // property in a second pass.
                UseColumnDataType = true,

                // Gets or sets a callback to determine whether to include the current sheet
                // in the DataSet. Called once per sheet before ConfigureDataTable.
                FilterSheet = (tableReader, sheetIndex) => true,

                // Gets or sets a callback to obtain configuration options for a DataTable. 
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    // Gets or sets a value indicating the prefix of generated column names.
                    EmptyColumnNamePrefix = "Column",

                    // Gets or sets a value indicating whether to use a row from the 
                    // data as column names.
                    UseHeaderRow = false,

                    // Gets or sets a callback to determine which row is the header row. 
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = (rowReader) =>
                    {
                        // F.ex skip the first row and use the 2nd row as column headers:
                        rowReader.Read();
                    },

                    // Gets or sets a callback to determine whether to include the 
                    // current row in the DataTable.
                    FilterRow = (rowReader) =>
                    {
                        return true;
                    },

                    // Gets or sets a callback to determine whether to include the specific
                    // column in the DataTable. Called once per column after reading the 
                    // headers.
                    FilterColumn = (rowReader, columnIndex) =>
                    {
                        return true;
                    }
                }
            };
        }

        private static bool ValidateExcelDocument<T>(T RequestObj, string excelExcelFilePath) where T : class
        {
            string ext = Path.GetExtension(excelExcelFilePath).ToLower();

            return (IsAllowedExt(ext, false) && NoNullHeaders(((dynamic)RequestObj).Headers));
        }

        public static bool IsAllowedExt(string ext, bool isWaiver)
        {
            if (isWaiver)
            {
                return (ext == ".msg" || ext == ".eml");
            }

            return (ext == ".xlsx" || ext == ".xls" || ext == ".ppt" || ext == ".pptx");
        }

        public static bool IsAllowedExtReject(string ext, bool isWaiver)
        {
            if (isWaiver)
            {
                return (ext == ".msg" || ext == ".eml");
            }

            return (ext == ".xlsx" || ext == ".xls" || ext == ".ppt" || ext == ".pptx" || ext == ".jpeg" || ext == ".jpg" || ext == ".png");
        }

        private static List<string> GetDataTableHeaders(System.Data.DataTable dt)
        {
            return dt.Columns.Cast<DataColumn>()
                                         .Select(x => x.ColumnName)
                                         .ToList();
        }

        // function that creates an object from the given data row
        //public static T CreateItemFromRow<T>(DataRow row) where T : new()
        //{
        //    // create a new object
        //    T item = new();

        //    // set the item
        //    SetItemFromRow(item, row);

        //    // return 
        //    return item;
        //}

        // function that creates an object from the given data row

        public static T CreateItemFromRowMapper<T>(DataRow row) where T : new()
        {
            // create a new object
            T item = new();

            // set the item
            SetItemFromRowMapper(item, row);

            // return 
            return item;
        }

        //public static void SetItemFromRow<T1>(T1 item, DataRow row) where T1 : new()
        //{
        //    // go through each column
        //    foreach (DataColumn c in row.Table.Columns)
        //    {
        //        // find the property for the column
        //        PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

        //        // if exists, set the value
        //        if (p != null && row[c] != DBNull.Value)
        //        {
        //            p.SetValue(item, row[c], null);
        //        }
        //    }
        //}

        private static string GetExcelColumnName(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName).GetCustomAttributes(false).Where(x => x.GetType() == typeof(ExcelColumnNameAttribute)).FirstOrDefault();

            if (property != null)
            {
                return ((ExcelColumnNameAttribute)property).ColumnName;
            }
            return "";
        }

        //private static void ParsePrimitive(PropertyInfo prop, object entity, object value)
        //{
        //    if (prop.PropertyType == typeof(string))
        //    {
        //        prop.SetValue(entity, value.ToString().Trim(), null);
        //    }
        //    if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
        //    {
        //        if (value == null)
        //        {
        //            prop.SetValue(entity, null, null);
        //        }
        //        else
        //        {
        //            prop.SetValue(entity, int.Parse(value.ToString()), null);
        //        }
        //    }
        //    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<DateTime>))
        //    {
        //        bool isValid = DateTime.TryParse(value.ToString(), out DateTime date);

        //        if (isValid)
        //        {
        //            prop.SetValue(entity, date, null);
        //        }
        //        else
        //        {
        //            //Making an assumption here about the format of dates in the source data.
        //            isValid = DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd", new CultureInfo("en-US"), DateTimeStyles.AssumeLocal, out date);
        //            if (isValid)
        //            {
        //                prop.SetValue(entity, date, null);
        //            }
        //        }
        //    }
        //    if (prop.PropertyType == typeof(decimal))
        //    {
        //        prop.SetValue(entity, decimal.Parse(value.ToString()), null);
        //    }
        //    if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
        //    {
        //        bool isValid = double.TryParse(value.ToString(), out _);

        //        if (isValid)
        //        {
        //            prop.SetValue(entity, double.Parse(value.ToString()), null);
        //        }
        //    }
        //}

        public static void SetItemFromRowMapper<T1>(T1 item, DataRow row) where T1 : new()
        {
            try
            {
                var properties = item.GetType()
                   .GetProperties()
                   .Where(q => q.GetCustomAttributes(typeof(ExcelColumnNameAttribute), true).Any()).ToList();

                // go through each column
                //foreach (DataColumn c in row.Table.Columns)
                //{
                //    // find the property for the column
                //    //PropertyInfo p = item.GetType().GetProperty(c.ColumnName);


                //    // if exists, set the value
                //    //if (p != null && row[c] != DBNull.Value)
                //    //{
                //    //    p.SetValue(item, row[c], null);
                //    //}
                //}
                foreach (var property in properties)
                {
                    string columnName = GetExcelColumnName(item.GetType(), property.Name);

                    var attributes = property.GetCustomAttributes(false);
                    var columnMapping = attributes.FirstOrDefault(a => a.GetType() == typeof(ExcelColumnNameAttribute));

                    if (columnMapping != null && row[columnName] != DBNull.Value)
                    {
                        var mapsto = columnMapping as ExcelColumnNameAttribute;
                        //Console.WriteLine(msg, property.Name, mapsto.ColumnName);

                        property.SetValue(item, (row[columnName] is string) ? row[columnName].ToString().Trim() : row[columnName]);
                    }

                    //var propertyValue = row[columnName];

                    //if (propertyValue != DBNull.Value)
                    //{
                    //    Map(row, property, item, columnName);
                    //    break; //Assumes that the first matching column contains the source data
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        //public static void Map(DataRow row, PropertyInfo prop, object entity, string columnName)
        //{
        //    if (!String.IsNullOrWhiteSpace(columnName) && row.Table.Columns.Contains(columnName))
        //    {
        //        var propertyValue = row[columnName];

        //        if (propertyValue != DBNull.Value)
        //        {
        //            ParsePrimitive(prop, entity, row[columnName]);
        //        }
        //    }
        //}

        private static bool IsAllowedHeaders(List<string> ExpectedHeaders, List<string> dtHeaders)
        {
            ExpectedHeaders.Sort();
            dtHeaders.Sort();

            return (ExpectedHeaders.SequenceEqual(dtHeaders) && ExpectedHeaders.Count == dtHeaders.Count);
        }

        private static bool NoNullHeaders(List<string> Headers)
        {
            return Headers != null;
        }
    }

    public class ExcelColumns
    {
        public string TaskDone { get; set; }

        public string TaskOwner { get; set; }
    }

    public class ExcelTransactionError
    {
        public string ErrorType { get; set; }
        public string ErrorDesc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
