namespace Project.V1.Models;

public class SingleRequestObject
{
    public ApplicationUser User { get; set; }
    public List<SpectrumViewModel> Spectrums { get; set; }
    public List<TechTypeModel> TechTypes { get; set; }
    public List<ProjectTypeModel> ProjectTypes { get; set; }
    public string BulkUploadPath { get; set; }
    public bool IsWaiver { get; set; }
}