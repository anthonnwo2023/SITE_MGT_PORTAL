using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.DLL.Helpers.Excel
{
    public class ExcelColumnNameAttribute : Attribute
    {
        private readonly string Name;

        public ExcelColumnNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
