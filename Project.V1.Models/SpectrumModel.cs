namespace Project.V1.Models;

[Table("TBL_RFACCEPT_SPECTRUM")]
[Index(new string[] { nameof(Name), nameof(TechTypeId) }, IsUnique = true)]
public class SpectrumViewModel : ObjectBase
{
    public string TechTypeId { get; set; }

    [ForeignKey(nameof(TechTypeId))]
    public virtual TechTypeModel TechType { get; set; }
}
