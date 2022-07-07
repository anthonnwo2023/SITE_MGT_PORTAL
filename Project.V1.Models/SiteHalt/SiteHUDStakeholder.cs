namespace Project.V1.Models;

[Table("TBL_RFACCEPT_STAKEHOLDERS")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class SiteHUDStakeholder : ObjectBase
{
}
