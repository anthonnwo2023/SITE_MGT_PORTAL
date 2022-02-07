using Project.V1.Web.Request;

namespace Project.V1.Web
{
    public class Startup
    {
        //private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration/*, IWebHostEnvironment env*/)
        {
            Configuration = configuration;
            ///_env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>();

            //set ASPNETCORE_ENVIRONMENT=Production
            //if (_env.IsProduction())
            //    services.AddDbContext<ApplicationDbContext>();
            //else
            //    services.AddDbContext<ApplicationDbContext, StageAppDBContext>();

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddQuartz(q =>
            {
                // base quartz scheduler, job and trigger configuration
                q.SchedulerId = "Scheduler-Core";
                q.SchedulerName = "Custom Notification Scheduler";
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                // these are the defaults
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 20;
                });

                // configure jobs with code
                var jobKey = new JobKey("Daily Report job", "SMP group");

                q.AddJob<DailyReportEmail>(j => j
                    .StoreDurably()
                    .WithIdentity(jobKey)
                    .WithDescription("Daily Report Reminder")
                );

                q.AddTrigger(t => t
                    .WithIdentity("Daily Report Reminder Trigger")
                    .ForJob(jobKey)
                    .StartNow()
                    .WithCronSchedule(Configuration.GetValue<string>("Scheduler:DailyReport"))
                    .WithDescription("Daily Report Reminder Trigger")
                );
            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

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
            services.AddScoped<IRequest, SMPRequest>();
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
            services.AddScoped<IProjects, Projects>();
            services.AddScoped<ISummerConfig, SummerConfig>();
            services.AddScoped<IStaticReport, StaticReport>();
            services.AddScoped<IRequestListObject, RequestListObject>();


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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTQ0NTg3QDMxMzkyZTMzMmUzMFY4MGRvNmFCOG5vdmFxRVcxSTUySllsS2hPcnhjUlRjSUFUbytSNUZ4blk9;NTQ0NTg4QDMxMzkyZTMzMmUzMGc1U1dzaDV4Q0ZxUkZJdE1HUjNJSXB6SDhRM0QyakMzOTlGTWxQNjFuQUU9");

            ServiceActivator.Configure(app.ApplicationServices);

            // migrate any database changes on startup (includes initial db creation)
            context.Database.Migrate();

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

            app.UseMiddleware<ExceptionHandling>();

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
