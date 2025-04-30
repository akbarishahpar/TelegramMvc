using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TelegramMvc.Extensions;

public static class ViewResultExtensions
{
    public static string ToHtml(this ViewResult result, HttpContext httpContext, ControllerContext controllerContext)
    {
        var builder = new StringBuilder();
        using var output = new StringWriter(builder);

        var routeData = controllerContext.RouteData;
        var viewName = (result.ViewName ?? routeData.Values["action"] as string)!;
        var options = httpContext.RequestServices.GetRequiredService<IOptions<MvcViewOptions>>();
        var htmlHelperOptions = options.Value.HtmlHelperOptions;
        var viewEngineResult = result.ViewEngine?.FindView(controllerContext, viewName, true) ??
                               options.Value.ViewEngines.Select(x => x.FindView(controllerContext, viewName, true))
                                   .FirstOrDefault();

        var view = viewEngineResult!.View!;

        var viewContext = new ViewContext(controllerContext, view, result.ViewData, result.TempData, output,
            htmlHelperOptions);

        view
            .RenderAsync(viewContext)
            .GetAwaiter()
            .GetResult();

        return builder.ToString();
    }
}