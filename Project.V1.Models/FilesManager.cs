namespace Project.V1.Models;

public class FilesManager
{
    public int Index { get; set; }
    public bool UploadIsSSV { get; set; } = true;
    public bool UploadIsWaiver { get; set; }
    public string UploadType { get; set; }
    public string Filename { get; set; }
    public string UploadPath { get; set; }
    public FileStream Filestream { get; set; }
    public UploadFiles UploadFile { get; set; }
}
