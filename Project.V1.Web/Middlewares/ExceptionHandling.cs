namespace Project.V1.Web.Middlewares;

public class ErrorResponse
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

public class ExceptionHandling
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandling> _logger;

    public ExceptionHandling(RequestDelegate next, ILogger<ExceptionHandling> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var errorResponse = new ErrorResponse
        {
            Success = false
        };

        switch (exception)
        {
            case ApplicationException ex:
                if (ex.Message.Contains("Invalid token"))
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.ErrorMessage = ex.Message;
                    break;
                }

                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ErrorMessage = ex.Message;
                break;

            case KeyNotFoundException ex:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.ErrorMessage = ex.Message;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.ErrorMessage = "Internal Server Errors. Check Logs!";
                break;
        }

        _logger.LogError(exception.Message);
        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}
