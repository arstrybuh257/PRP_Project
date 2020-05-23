using System.Web.Mvc;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;

namespace GainBargain.WEB.Controllers
{
    public class UserController : Controller
    {
        private IRepository<FavoriteCategory> favCatRepository;
        private IRepository<FavoriteProduct> favProductRepository;
        public UserController()
        {
            favCatRepository = new Repository<FavoriteCategory>(new GainBargainContext());
            favProductRepository = new Repository<FavoriteProduct>(new GainBargainContext());
        }
        // GET: User
        public ActionResult Index()
        {
            

            return View();
        }

        
    }
}