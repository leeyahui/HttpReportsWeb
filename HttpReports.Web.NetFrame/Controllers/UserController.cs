using HttpReports.Web.Implements;
using HttpReports.Web.Models;
using System.Web.Mvc;

namespace HttpReports.Web.Controllers
{

    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly HttpReportsConfig _config;

        public UserController(HttpReportsConfig config)
        {
            _config = config;
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CheckLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new Result(-1, "账号或密码错误"));
            }

            if (_config.UserName == username && _config.Password == password)
            {
                HttpContext.SetCookie("login_info", username, 60 * 30 * 10);

                return Json(new Result(1, "登录成功"));
            }

            return Json(new Result(-1, "账号或密码错误"));
        }

        public ActionResult Logout()
        {
            HttpContext.DeleteCookie("login_info");

            return new RedirectResult("/User/Login");
        }



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

    }
}