
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Final_project.Utilities.Extensions
{
    public static class ExtensionMethods
    {
        public static string IsSelected(this IHtmlHelper htmlHelper, string controllers, string actions, string cssClass = "active")
        {
            string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            string[] acceptedActions = actions.Trim().Split(',');
            string[] acceptedControllers = controllers.Trim().Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : string.Empty;
        }
    }
}
