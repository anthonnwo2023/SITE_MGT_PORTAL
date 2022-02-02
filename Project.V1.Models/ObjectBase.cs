namespace Project.V1.Models;

public class ObjectBase
{
    [Key]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    public bool IsActive { get; set; }

    public DateTime DateCreated { get; set; }
}
