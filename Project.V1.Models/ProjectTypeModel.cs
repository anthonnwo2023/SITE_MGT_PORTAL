namespace Project.V1.Models;

[Table("TBL_RFACCEPT_PROJECTTYPES")]
[Index(new string[] { nameof(Name), nameof(SpectrumId) }, IsUnique = true)]
public class ProjectTypeModel : ObjectBase
{
    public string SpectrumId { get; set; }

    [ForeignKey(nameof(SpectrumId))]
    public virtual SpectrumViewModel Spectrum { get; set; }
}
