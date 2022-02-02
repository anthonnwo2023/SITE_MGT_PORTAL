namespace Project.V1.Models;

[Table("TBL_RFACCEPT_BASEBANDS")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class BaseBandModel : ObjectBase
{
    public string VendorId { get; set; }

    [ForeignKey(nameof(VendorId))]
    public virtual VendorModel Vendor { get; set; }
}

