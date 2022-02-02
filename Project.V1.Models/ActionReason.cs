namespace Project.V1.Models;

[Table("TBL_RFACCEPT_REQUEST_ACTIONCOMMENTS")]
public class ActionReason
{
    [Key]
    public string Id { get; set; }

    public string Comment { get; set; }

    public string CommentBy { get; set; }

    public DateTime DateCreated { get; set; }
}
