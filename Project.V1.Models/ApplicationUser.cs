using Microsoft.Extensions.DependencyInjection;

namespace Project.V1.Models;

public class ApplicationUser : IdentityUser, IDisposable
{
    private UserManager<ApplicationUser> UserManager => ServiceActivator.GetScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();

    [NotMapped]
    private bool disposed = false;

    [Display(Name = "Job Title")]
    public string JobTitle { get; set; }

    public string Department { get; set; }

    [Required]
    public string Fullname { get; set; }

    public string UserType { get; set; } = "Internal";

    [Display(Name = "Last Logged In")]
    public DateTime LastLoginDate { get; set; }

    [Display(Name = "Date Created")]
    public DateTime DateCreated { get; set; }

    public bool IsActive { get; set; }

    [NotMapped]
    public bool IsMTNUser { get; set; }

    public bool ShowAllRegionReport { get; set; }

    public bool IsADLoaded { get; set; }

    public bool IsNewPassword { get; set; }

    public Task<IList<string>> UserRoles => UserManager.GetRolesAsync(this);

    [NotMapped]
    public string[] Roles { get; set; }

    [NotMapped]
    public List<ClaimViewModel> Projects { get; set; }

    [Required]
    public string VendorId { get; set; }

    [ForeignKey(nameof(VendorId))]
    public virtual VendorModel Vendor { get; set; }

    public virtual List<RegionViewModel> Regions { get; set; }

    protected void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                //await this.DisposeAsync();
            }
        }
        this.disposed = true;
    }

    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
