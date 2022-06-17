using Project.V1.Web.MappingProfiles;

namespace Project.V1.Web;

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
        services.AddDbContext<ApplicationDbContext>();
        services.AddDbContext<MTNISDbContext>();

        //set ASPNETCORE_ENVIRONMENT=Production
        //if (_env.IsProduction())
        //    services.AddDbContext<ApplicationDbContext>();
        //else
        //    services.AddDbContext<ApplicationDbContext, StageAppDBContext>();

        services.AddAutoMapper(typeof(RequestMapProfiles));

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

            jobKey = new JobKey("Monthly Report job", "SMP group");

            q.AddJob<MonthlyReportEmail>(j => j
                .StoreDurably()
                .WithIdentity(jobKey)
                .WithDescription("Monthly Report Reminder")
            );

            q.AddTrigger(t => t
                .WithIdentity("Monthly Report Reminder Trigger")
                .ForJob(jobKey)
                .StartNow()
                .WithCronSchedule(Configuration.GetValue<string>("Scheduler:MonthlyReport"))
                //.WithCronSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 7, 30))
                .WithDescription("Daily Report Reminder Trigger")
            );

            jobKey = new JobKey("4 Hourly Report job", "SMP group");

            q.AddJob<FourtlyHourlyReportEmail>(j => j
                .StoreDurably()
                .WithIdentity(jobKey)
                .WithDescription("4 Hourly Report Reminder")
            );

            q.AddTrigger(t => t
                .WithIdentity("4 Hourly Report Reminder Trigger")
                .ForJob(jobKey)
                .StartNow()
                .WithCronSchedule(Configuration.GetValue<string>("Scheduler:Hourly4Report"))
                //.WithCronSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(1, 7, 30))
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

        services.AddHttpClient("RequestClient")
            .ConfigureHttpMessageHandlerBuilder(builder =>
            {
                var a = new HttpClientHandler()
                {
                    UseDefaultCredentials = true
                };
                a.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                builder.PrimaryHandler = a;

                builder.Build();
            });
        services.AddHttpContextAccessor();

        services.AddScoped<IUserAuthentication, UserAuthentication>();
        services.AddScoped<ICLogger, CLogger>();
        services.AddScoped<IRequest, SMPRequest>();
        services.AddScoped<ISSCRequestUpdate, SSCRequestUpdate>();
        services.AddScoped<IHUDRequest, DLL.Services.SiteHalt.HUDRequest>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<IClaimCategory, ClaimCategory>();
        services.AddScoped<IVendor, Vendor>();
        services.AddScoped<IUser, UserService>();
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
        services.AddScoped<AcceptanceAdaptor>();

        services.ConfigureBlazorizeOptions();
        services.ConfigureCors();
        services.ConfigureBasicAuthenticationHandler();
        services.ConfigureIdentityOption();
        services.ConfigureValidators();
        services.AddSyncfusionBlazor(option => option.IgnoreScriptIsolation = false);

        // register the scope authorization handler
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
        services.AddSingleton<AppState>();

        services.AddControllers()
        .AddMvcOptions(opts =>
        {
            opts.Filters.Add<SerilogLoggingActionFilter>();
            opts.Filters.Add<SerilogLoggingPageFilter>();
        })
        .AddOData(option => option.AddRouteComponents("odata", GetEdmModel())
        .Count().Filter().OrderBy().Expand().SetMaxTop(null).Select())
        //.AddODataNewtonsoftJson()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
        .AddRazorRuntimeCompilation();

        services.AddRazorPages()
            .AddMvcOptions(opts =>
            {
                opts.Filters.Add<SerilogLoggingActionFilter>();
                opts.Filters.Add<SerilogLoggingPageFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddRazorRuntimeCompilation()
            ;

        services.AddAuthorization();
        services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

        services.AddServerSideBlazor();
        services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
        services.AddScoped<HUDPDFExportService>();
        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    static IEdmModel GetEdmModel()
    {
        ODataConventionModelBuilder builder = new();

        //ComplexTypeConfiguration<RequestApproverModel> approverType = builder.ComplexType<RequestApproverModel>();
        //approverType.Property(p => p.Id);
        //approverType.Property(p => p.IsApproved);
        //approverType.Property(p => p.DateApproved);
        //approverType.Property(p => p.Fullname);
        //approverType.Property(p => p.ApproverComment);

        ComplexTypeConfiguration<VendorModel> vendorType = builder.ComplexType<VendorModel>();
        vendorType.Property(p => p.Id);
        vendorType.Property(p => p.Name);
        vendorType.Property(p => p.IsActive);
        vendorType.Property(p => p.DateCreated);

        //ComplexTypeConfiguration<RequesterData> requesterType = builder.ComplexType<RequesterData>();
        //requesterType.Property(p => p.Id);
        //requesterType.Property(p => p.Name);
        //requesterType.Property(p => p.Department);
        //requesterType.Property(p => p.Email);
        //requesterType.Property(p => p.Username);
        //requesterType.Property(p => p.VendorId);
        //requesterType.Property(p => p.Phone);
        //requesterType.ComplexProperty(p => p.Vendor);

        //EntityTypeConfiguration<RequestViewModel> acceptanceType = builder.EntityType<RequestViewModel>();
        //acceptanceType.HasKey(p => p.Id);
        //acceptanceType.ComplexProperty(p => p.EngineerAssigned);
        //acceptanceType.ComplexProperty(p => p.Requester);

        //builder.EntitySet<RequestApproverModel>("EngineerAssigned");
        //builder.EntitySet<VendorModel>("Vendors");
        //builder.EntitySet<RequesterData>("Requester");
        builder.EntitySet<RequestViewModel>("Acceptance");
        builder.EntitySet<RequestViewModelDTO>("AcceptanceDTO");
        builder.EntitySet<SSCUpdatedCell>("SSCRequest");

        //BuildFunction(builder);
        return builder.GetEdmModel();
    }

    static void BuildFunction(ODataConventionModelBuilder builder)
    {
        var function = builder.EntityType<RequestViewModel>().Collection.Function("GetAcceptances").Returns<IQueryable<RequestViewModel>>();
        function.Parameter<string>("userName");
        function.Parameter<string>("vendorName");
        function.Parameter<bool>("showAllRegionReport");
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjUyNzQ0QDMyMzAyZTMxMmUzMENHemJ6dTk1RFdkWXE2Y2wvVGwvb3dERDdNNzlUUDNyUklTNGNaWk9lbDg9");

        ServiceActivator.Configure(app.ApplicationServices);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            // migrate any database changes on startup (includes initial db creation)
            context.Database.Migrate();

            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);

        //app.UseMiddleware<ExceptionHandling>();

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

        app.UseCors("AllowAll");

        //app.UseSession();

        app.UseAuthentication();

        app.Use(async (ctx, next) =>
        {
            using (LogContext.PushProperty("IPAddress", ctx.Connection.RemoteIpAddress))
            {
                await next();
            }
        });

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
