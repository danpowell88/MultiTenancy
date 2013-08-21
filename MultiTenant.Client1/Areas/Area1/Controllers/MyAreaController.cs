using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MultiTenant.Client1.Areas.Area1.Controllers
{
    public class MyAreaController : MultiTenant.Base.Areas.Area1.Controllers.MyAreaController
    {
        public override ActionResult Index()
        {
            ViewBag.Message = "Client 1 - Area 1 - Index action";
            return View();
        }

    }
}
