namespace Project.V1.Models;

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
