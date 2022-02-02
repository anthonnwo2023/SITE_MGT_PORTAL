namespace Project.V1.Models;

[Table("TBL_RFACCEPT_REQUEST_REQUESTER")]
public class RequesterData
{
    [Key]
    public string Id { get; set; }
    public string Username { get; set; }
    [Required]
    [Display(Name = "Fullname")]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; }
    [Required]
    [Phone]
    [Display(Name = "Phone Number")]
    public string Phone { get; set; }
    [Required]
    [Display(Name = "Designation")]
    public string Title { get; set; }
    [Required]
    [Display(Name = "Department/Unit")]
    public string Department { get; set; }

    public string VendorId { get; set; }

    public virtual VendorModel Vendor { get; set; }
}
