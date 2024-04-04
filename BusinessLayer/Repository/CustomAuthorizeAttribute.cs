using BusinessLayer.InterFace;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using DataAccessLayer.DataModels;
using System.Linq;

public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _role;
    private readonly string _menuid;
    public CustomAuthorizeAttribute(string role = "", string menuid = "")
    {
        this._role = role;
        this._menuid = menuid;
        
    }



    public void OnAuthorization(AuthorizationFilterContext context)
    {
            var jwtServices = context.HttpContext.RequestServices.GetService<IJwtAuth>();
        var admin = context.HttpContext.RequestServices.GetService<IAdmin>();

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
        var userPermissions = admin.GetUserPermissions(roleType);
      
        if (string.IsNullOrWhiteSpace(_role) || !userPermissions.Contains(int.Parse(_menuid)))
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
