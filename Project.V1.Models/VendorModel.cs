namespace Project.V1.Models;

[Table("TBL_RFACCEPT_VENDORS")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class VendorModel : ObjectBase
{
    public string MailList { get; set; }

    public bool ShouldSummerize { get; set; }
}
