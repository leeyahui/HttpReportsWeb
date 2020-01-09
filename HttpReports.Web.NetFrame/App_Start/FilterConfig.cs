using System.Linq;
using System.Web.Mvc;

namespace HttpReports.Web.NetFrame
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalAuthorizeFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }

    internal class GlobalAuthorizeFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext context)
        {
            // 判断是否跳过授权过滤器 
            if (context.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            var cookies = context.HttpContext.Request.Cookies;
            if (cookies.AllKeys.ToList().Contains("login_info"))
            {
                string cookie = context.HttpContext.Request.Cookies["login_info"].Value;

                if (string.IsNullOrEmpty(cookie))
                {
                    context.Result = new RedirectResult("/User/Login");
                    return;
                }
            }
            else
            {
                context.Result = new RedirectResult("/User/Login");
                return;
            }

        }
    }
}
