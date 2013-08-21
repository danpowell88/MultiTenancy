using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MultiTenant.Base.Areas.Area1.Controllers
{
    public class MyAreaController : Controller
    {
        public virtual ActionResult Index()
        {
            ViewBag.Message = "Base Area 1 My Area Controller";
            return View();
        }

    }
}
