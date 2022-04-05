namespace Project.V1.Models;

[Table("TBL_RFACCEPT_TECHTYPES")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class TechTypeModel : ObjectBase
{
    public virtual List<SiteHUDRequestModel> HUDRequests { get; set; }
}
