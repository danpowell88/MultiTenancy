using System.Web.Mvc;

namespace MutiTenant.Client2.Controllers
{
    public class HomeController : MultiTenant.Base.Controllers.HomeController
    {
        public override ActionResult Index()
        {
            ViewBag.Message = "Client 2 Index";

            return View();
        }

        public override ActionResult About()
        {
            ViewBag.Message = "Client 2 About";

            return View();
        }

        public override ActionResult Contact()
        {
            ViewBag.Message = "Client 2 Contact";

            return View();
        }
    }
}
