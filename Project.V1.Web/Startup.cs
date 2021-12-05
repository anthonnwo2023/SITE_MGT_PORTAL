using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Project.V1.Data;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Extensions;
using Project.V1.Lib.Helpers;
using Project.V1.DLL.Interface;
using Project.V1.Lib.Services;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Models;
using Project.V1.Web.Areas.Identity;
using Project.V1.Web.Middlewares;
using Serilog;
using Serilog.Context;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Navigations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Project.V1.DLL.Services.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Project.V1.DLL.Helpers;

namespace Project.V1.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseOracle(
                    Configuration.GetConnectionString("OracleConnection")
                    );
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddDefaultIdentity<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = false;

            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(10);
                options.OnRefreshingPrincipal = context =>
                {
                    System.Security.Claims.Claim originalUserIdClaim = context.CurrentPrincipal.FindFirst("OriginalUserId");
                    System.Security.Claims.Claim isImpersonatingClaim = context.CurrentPrincipal.FindFirst("IsImpersonating");
                    if (isImpersonatingClaim != null)
                    {
                        if (isImpersonatingClaim.Value == "true" && originalUserIdClaim != null)
                        {
                            context.NewPrincipal.Identities.First().AddClaim(originalUserIdClaim);
                            context.NewPrincipal.Identities.First().AddClaim(isImpersonatingClaim);
                        }
                    }
                    return Task.FromResult(0);
                };
            });

            services.AddScoped<IUserAuthentication, UserAuthentication>();
            services.AddScoped<ICLogger, CLogger>();
            services.AddScoped<IRequest, Request>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<IClaimCategory, ClaimCategory>();
            services.AddScoped<IVendor, Vendor>();
            services.AddScoped<IUser, User>();
            services.AddScoped<IRegion, Region>();
            services.AddScoped<IAntennaType, AntennaType>();
            services.AddScoped<IAntennaMake, AntennaMake>();
            services.AddScoped<IProjectType, ProjectType>();
            services.AddScoped<ITechType, TechType>();
            services.AddScoped<ISpectrum, Spectrum>();
            services.AddScoped<IBaseBand, BaseBand>();
            services.AddScoped<IRRUType, RRUType>();
            services.AddScoped<ISummerConfig, SummerConfig>();


            services.ConfigureBlazorizeOptions();
            services.ConfigureBasicAuthenticationHandler();
            services.ConfigureIdentityOption();
            services.ConfigureValidators();
            services.AddSyncfusionBlazor();
            services.AddHttpContextAccessor();

            services.AddAuthorization();

            // register the scope authorization handler
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddSingleton<AppState>();

            services.AddRazorPages()
                .AddMvcOptions(opts =>
                {
                    opts.Filters.Add<SerilogLoggingActionFilter>();
                    opts.Filters.Add<SerilogLoggingPageFilter>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .AddRazorRuntimeCompilation();

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTE1OTIxQDMxMzkyZTMzMmUzMGhia0ZGMXNFVU54N1dGNWVIb3RFOU5xbmJTMkJocDkxdVN5bEd0UExLUWc9");

            ServiceActivator.Configure(app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (ctx, next) =>
            {
                using (LogContext.PushProperty("IPAddress", ctx.Connection.RemoteIpAddress))
                {
                    await next();
                }
            });

            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);

            app.UseHttpsRedirection();

            FileExtensionContentTypeProvider provider = new();
            provider.Mappings[".msg"] = "application/vnd.ms-outlook";
            provider.Mappings[".eml"] = "application/octet-stream";

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider
                (
                    Path.Combine(Directory.GetCurrentDirectory(), "Documents")
                ),
                RequestPath = "/Documents",
                ContentTypeProvider = provider
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
