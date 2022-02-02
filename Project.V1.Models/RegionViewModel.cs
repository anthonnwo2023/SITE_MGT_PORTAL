namespace Project.V1.Models;

[Table("TBL_RFACCEPT_REGIONS")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class RegionViewModel : ObjectBase
{
    [Required]
    public string Abbr { get; set; }

    public string MailList { get; set; }

    public bool IsRegular { get; set; }

    public bool IsRural { get; set; }

    public virtual List<ApplicationUser> Users { get; set; }
}

