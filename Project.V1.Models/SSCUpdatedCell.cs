namespace Project.V1.Models;

[Table("SSC_CELLS_UPDATED")]
public class SSCUpdatedCell
{
    [Key]
    public double ID { get; set; }
    public string SITE_ID { get; set; }
    public string CELL_ID { get; set; }
    public string PREV_ADMIN_STATUS { get; set; }
    public string ADMIN_STATUS { get; set; }
    public string UPDATED_BY { get; set; }
    public string UNIQUE_ID { get; set; }
    public string SITE_ACCEPTANCE { get; set; }
    public string TECHNOLOGY { get; set; }
    public string BAND { get; set; }
    public DateTime DATECREATED { get; set; }
}
