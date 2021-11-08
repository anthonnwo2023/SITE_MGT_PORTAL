using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelColumnNameAttribute : Attribute
    {
        protected string _name;

        public string ColumnName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public ExcelColumnNameAttribute()
        {
            _name = "";
        }

        public ExcelColumnNameAttribute(string name)
        {
            _name = name;
        }
    }
}