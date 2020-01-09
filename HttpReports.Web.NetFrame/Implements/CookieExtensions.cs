using System;
using System.Web;

namespace HttpReports.Web.Implements
{
    public static class CookieExtensions
    {
        public static void SetCookie(this HttpContextBase context, string key, string value, double minutes)
        {
            context.Response.Cookies.Add(new HttpCookie(key, value)
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }

        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        public static void DeleteCookie(this HttpContextBase context, string key)
        {
            context.Response.Cookies.Remove(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        public static string GetCookie(this HttpContextBase context, string key)
        {
            var cookie = context.Request.Cookies.Get(key);
            if (string.IsNullOrEmpty(cookie.Value))
                return string.Empty;
            return cookie.Value;
        }

    }
}
