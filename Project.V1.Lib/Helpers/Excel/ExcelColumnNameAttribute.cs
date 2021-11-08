using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Lib.Helpers.Excel
{
    public class ExcelColumnNameAttribute : Attribute
    {
        public readonly string ColumnName;

        public ExcelColumnNameAttribute(string name)
        {
            this.ColumnName = name;
        }
    }
}
