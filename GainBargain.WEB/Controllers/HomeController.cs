using GainBargain.DAL.EF;
using System.Web.Mvc;
using System.Linq;

namespace GainBargain.WEB.Controllers
{
    public class HomeController : Controller
    {
        GainBargainContext db = new GainBargainContext();

        public ActionResult Index()
        {
            //var db = new GainBargainContext();

            var productList = db.Products
                .OrderBy(p => p.Name)
                .Take(30)
                .ToList();

            return View(productList);
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