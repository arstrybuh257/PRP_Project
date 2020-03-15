using GainBargain.DAL.EF;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var db = new GainBargainContext();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}