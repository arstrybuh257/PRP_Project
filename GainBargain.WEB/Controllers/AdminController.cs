using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class AdminController : Controller
    {
        // Idk, what it should be and whether it should be
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Sources()
        {


            return View();
        }
    }
}