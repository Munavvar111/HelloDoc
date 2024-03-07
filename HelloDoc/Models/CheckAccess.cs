//using BusinessLayer.InterFace;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System.IdentityModel.Tokens.Jwt;

//namespace HelloDoc.Models
//{
//    public class CheckAccess : IAuthorizationFilter
//    {
//        public void OnAuthorization(AuthorizationFilterContext context)
//        {
//            string sessionData = HttpContext.Session.GetString("token");
//            if (sessionData == null)
//            {
//                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));

//            }
//        }
//    }
//}
