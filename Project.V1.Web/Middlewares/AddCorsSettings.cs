namespace Project.V1.Web.Middlewares;

public static class AddCorsSettings
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
        });
    }
}
