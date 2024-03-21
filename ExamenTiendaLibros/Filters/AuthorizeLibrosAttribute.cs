using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace ExamenTiendaLibros.Filters
{
    public class AuthorizeLibrosAttribute : AuthorizeAttribute
        , IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            Debug.WriteLine("Controller: " + controller);
            Debug.WriteLine("Action: " + action);
            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
            var TempData = provider.LoadTempData(context.HttpContext);

            TempData["controller"] = controller;
            TempData["action"] = action;

            provider.SaveTempData(context.HttpContext, TempData);


            if (user.Identity.IsAuthenticated == false)
            {
                //ENVIAMOS A LA VISTA LOGIN
                context.Result = this.GetRoute("Managed", "Login");
            }
        }

        private RedirectToRouteResult GetRoute
            (string controller, string action)
        {
            RouteValueDictionary ruta =
                new RouteValueDictionary(
                    new { controller = controller, action = action });
            RedirectToRouteResult result =
                new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
