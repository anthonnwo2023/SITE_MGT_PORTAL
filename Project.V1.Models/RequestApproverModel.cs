using Microsoft.Extensions.DependencyInjection;

namespace Project.V1.Models;

[Table("TBL_RFACCEPT_APPROVER")]
public class RequestApproverModel : PersonModel
{
    private UserManager<ApplicationUser> _userManager { get; set; } = ServiceActivator.GetScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();

    public string RequestId { get; set; }

    public string ApproverType { get; set; }

    [Display(Name = "Comment")]
    public string ApproverComment { get; set; }

    [Display(Name = "Request Actioned?")]
    public bool IsActioned { get; set; }

    public bool IsApproved { get; set; }

    public DateTime DateAssigned { get; set; }

    public DateTime DateActioned { get; set; }

    [Required]
    [Display(Name = "Date Actioned")]
    public DateTime DateApproved { get; set; }

    public string UserId => _userManager.FindByNameAsync(Username).Result?.Id;
}
