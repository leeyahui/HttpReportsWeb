using HttpReports.Web.Services;
using System.Web.Mvc;

namespace HttpReports.Web.Controllers
{

    [AllowAnonymous]
    public class TestController : Controller
    {
        private readonly DataService _dataService;

        public TestController(DataService dataService)
        {
            _dataService = dataService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult A1()
        {
            System.Threading.Thread.Sleep(1000);

            return Json(new { code = 1 });
        }

        public ActionResult A2()
        {
            System.Threading.Thread.Sleep(2000);

            return Json(new { code = 1 });
        }

        public ActionResult A3()
        {
            System.Threading.Thread.Sleep(3000);

            return Json(new { code = 1 });
        }

        public ActionResult A4()
        {
            System.Threading.Thread.Sleep(4000);

            return Json(new { code = 1 });
        }

        public ActionResult A5()
        {
            System.Threading.Thread.Sleep(5000);

            return Json(new { code = 1 });
        }

        public ActionResult A6()
        {
            System.Threading.Thread.Sleep(6000);

            return Json(new { code = 1 });
        }

        public ActionResult A7()
        {
            System.Threading.Thread.Sleep(7000);

            return Json(new { code = 1 });
        }

        public ActionResult A8()
        {
            System.Threading.Thread.Sleep(8000);

            return Json(new { code = 1 });
        }

        public ActionResult A9()
        {
            System.Threading.Thread.Sleep(9000);

            return Json(new { code = 1 });
        }

        public ActionResult A10()
        {
            System.Threading.Thread.Sleep(10000);

            return Json(new { code = 1 });
        }
    }
}