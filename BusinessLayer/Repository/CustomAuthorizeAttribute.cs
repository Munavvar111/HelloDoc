using BusinessLayer.InterFace;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;

public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _role;
    public CustomAuthorizeAttribute(string role = "")
    {
        this._role = role;
    }



    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var jwtServices = context.HttpContext.RequestServices.GetService<IJwtAuth>();

        if (jwtServices == null)
        {
            return;
        }

        var token = context.HttpContext.Session.GetString("token");

        if (token == null || !jwtServices.ValidateToken(token, out JwtSecurityToken validatedToken))
        {
            context.HttpContext.Session.SetString("returnurl", context.HttpContext.Request.Path);

            if (IsAjaxRequest(context.HttpContext.Request))
            {
                context.Result = new JsonResult(new { error = "Access denied", redirectToLogin = true })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
            else
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }
            
            return;


        }

        var roleClaim = validatedToken.Claims.Where(m => m.Type == "role").FirstOrDefault();
        var roleType = roleClaim.Value;
        if (roleClaim == null)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));

            return;
        }

        if (string.IsNullOrWhiteSpace(_role) || roleType != _role)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
            return;
        }

    }

    private bool IsAjaxRequest(HttpRequest request)
    {
        return string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}
