using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.IO;
using System.Linq;

namespace Project.V1.DLL.Helpers
{
    public class SerilogLoggingActionFilter : IActionFilter
    {
        private readonly IDiagnosticContext _diagnosticContext;
        public SerilogLoggingActionFilter(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST")
            {
                context.HttpContext.Request.EnableBuffering();
                string body = new StreamReader(context.HttpContext.Request.Body)
                                                 .ReadToEndAsync().GetAwaiter().GetResult();
                context.HttpContext.Request.Body.Position = 0;

                //_diagnosticContext.Set("Request Body Stream", $"{HelperFunctions.ReadStreamInChunks(context.HttpContext.Request.Body)}");
                _diagnosticContext.Set("Request Body", context.HttpContext.Request.Form);
            }

            if (context.HttpContext.User.Identity!.IsAuthenticated)
            {
                _diagnosticContext.Set("UserClaims", string.Join(",", context.HttpContext.User.Claims.Select(x => x.Value).ToList()));
                _diagnosticContext.Set("UserName", context.HttpContext.User.Identity.Name);
                _diagnosticContext.Set("UserAction", $"{context.HttpContext.User.Identity.Name} accessed ({context.Controller}).({context.ActionDescriptor.DisplayName}) with data: {context.ActionDescriptor.RouteValues}");
            }

            _diagnosticContext.Set("RouteData", context.ActionDescriptor.RouteValues);
            _diagnosticContext.Set("ActionName", context.ActionDescriptor.DisplayName);
            _diagnosticContext.Set("ActionId", context.ActionDescriptor.Id);
            _diagnosticContext.Set("ValidationState", context.ModelState.IsValid);
        }

        // Required by the interface
        public void OnActionExecuted(ActionExecutedContext context) { }
    }

    public class SerilogLoggingPageFilter : IPageFilter
    {
        private readonly IDiagnosticContext _diagnosticContext;
        public SerilogLoggingPageFilter(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            string name = context.HandlerMethod?.Name ?? context.HandlerMethod?.MethodInfo.Name;
            if (name != null)
            {
                _diagnosticContext.Set("RazorPageHandler", name);
            }
        }

        // Required by the interface
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST")
            {
                context.HttpContext.Request.EnableBuffering();
                string body = new StreamReader(context.HttpContext.Request.Body)
                                                 .ReadToEndAsync().GetAwaiter().GetResult();
                context.HttpContext.Request.Body.Position = 0;

                //_diagnosticContext.Set("Request Body Stream", $"{HelperFunctions.ReadStreamInChunks(context.HttpContext.Request.Body)}");
                if (context.HttpContext.Request.HasFormContentType)
                {
                    _diagnosticContext.Set("Request Body", context.HttpContext.Request.Form);
                }
            }

            if (context.HttpContext.User.Identity!.IsAuthenticated)
            {
                _diagnosticContext.Set("UserClaims", string.Join(",", context.HttpContext.User.Claims.Select(x => x.Value).ToList()));
                _diagnosticContext.Set("UserName", context.HttpContext.User.Identity.Name);
                _diagnosticContext.Set("PageRouteData", context.RouteData.DataTokens);
                _diagnosticContext.Set("PageRouteResult", context.Result);
                _diagnosticContext.Set("AreaName", context.ActionDescriptor.AreaName);
                _diagnosticContext.Set("ModelTypes", context.ActionDescriptor.ModelTypeInfo);
                _diagnosticContext.Set("PageTypes", context.ActionDescriptor.PageTypeInfo);
                _diagnosticContext.Set("UserAction", $"{context.HttpContext.User.Identity.Name} accessed ({context.HandlerInstance}).({context.ActionDescriptor.DisplayName}) with data: {context.ActionDescriptor.RouteValues}");
            }

            _diagnosticContext.Set("RouteData", context.ActionDescriptor.RouteValues);
            _diagnosticContext.Set("ActionName", context.ActionDescriptor.DisplayName);
            _diagnosticContext.Set("ActionId", context.ActionDescriptor.Id);
            _diagnosticContext.Set("ValidationState", context.ModelState.IsValid);
        }
    }
}
