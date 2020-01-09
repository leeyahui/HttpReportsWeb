using HttpReports.Web.Models;
using HttpReports.Web.Services;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HttpReports.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataService _dataService;

        public HomeController(DataService dataService)
        {
            _dataService = dataService;
        }

        public ActionResult Index()
        {
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            return View();
        }

        public ActionResult Trend()
        {
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            return View();
        }

        public ActionResult Monitor()
        {
            var jobs = _dataService.GetJobs();

            List<JobRequest> list = new List<JobRequest>();

            foreach (var item in jobs)
            {
                list.Add(new JobRequest
                {
                    Id = item.Id,
                    Title = item.Title,
                    CronLike = _dataService.ParseJobCronString(item.CronLike),
                    Email = item.Emails,
                    Mobiles = item.Mobiles,
                    Status = item.Status,
                    Node = item.Servers

                });
            }

            ViewBag.list = list;

            return View();
        }

        public ActionResult AddMonitor(int Id = 0)
        {
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            ViewBag.Id = Id;

            return View();
        }


        public ActionResult Detail()
        {
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

    }
}