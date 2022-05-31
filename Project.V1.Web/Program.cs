using System.Reflection;

namespace Project.V1.Web;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            Log.Information("Application starting up");
            IHost host = CreateHostBuilder(args).Build();
            ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
            host.Run();
            Log.Information("The application has started");
        }
        catch (Exception ex)
        {
            Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
            Log.Fatal(ex, "The application failed to start correctly");
        }
        finally
        {
            Log.Information("Shutdown: ", new { AppStatus = $"Closed: {DateTime.Now} - Application powered down + CloseAndFlush()" });
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureLogging()
    {
        Log.Logger = HelperFunctions.GetSerilogLogger();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureLogging((context, logging) =>
            {
                logging.AddDebug();
                logging.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configuration.AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true);
            });
}
