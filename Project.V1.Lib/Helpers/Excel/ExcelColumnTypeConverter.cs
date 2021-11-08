﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Lib.Helpers.Excel
{
    public static class ExcelColumnTypeConverter
    {
        public static void ConvertColumnTypeTo<TTargetType>(this DataTable dt, string columnName, Func<object, TTargetType> valueConverter)
        {
            var newType = typeof(TTargetType);

            DataColumn dc = new(columnName + "_new", newType);

            // Add the new column which has the new type, and move it to the ordinal of the old column
            int ordinal = dt.Columns[columnName].Ordinal;
            dt.Columns.Add(dc);
            dc.SetOrdinal(ordinal);

            // Get and convert the values of the old column, and insert them into the new
            foreach (DataRow dr in dt.Rows)
            {
                dr[dc.ColumnName] = valueConverter(dr[columnName]);
            }

            // Remove the old column
            dt.Columns.Remove(columnName);

            // Give the new column the old column's name
            dc.ColumnName = columnName;
        }
    }
}
