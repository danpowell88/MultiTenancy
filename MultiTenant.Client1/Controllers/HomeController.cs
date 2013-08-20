using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MultiTenant.Client1.Controllers
{
    public class HomeController : MultiTenant.Base.Controllers.HomeController
    {
        public override ActionResult Index()
        {
            ViewBag.Message = "Client 1 Index";

            return View();
        }

        public override ActionResult About()
        {
            ViewBag.Message = "Client 1 About";

            return View();
        }

        public override ActionResult Contact()
        {
            ViewBag.Message = "Client 1 Contact";

            return View();
        }
    }
}
