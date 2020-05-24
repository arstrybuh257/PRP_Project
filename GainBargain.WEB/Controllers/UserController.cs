using System.Web.Mvc;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.WEB.Models;
using Microsoft.AspNet.Identity;

namespace GainBargain.WEB.Controllers
{
    public class UserController : Controller
    {
        private IFavoriteCategoriesRepository favCatRepository;
        private IRepository<FavoriteProduct> favProductRepository;
        public UserController()
        {
            favCatRepository = new FavoriteCategoriesRepository(new GainBargainContext());
            favProductRepository = new Repository<FavoriteProduct>(new GainBargainContext());
        }
        // GET: User
        public ActionResult Index()
        {
            var model = new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                //FavoriteCategories = favCatRepository.Find(x=>x.UserId == int.Parse(User.Identity.GetUserId())),
                //FavoriteProducts = favCatRepository.Find(x => x.UserId == int.Parse(User.Identity.GetUserId()))
            };

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult FavoriteCategoriesPartial()
        {
            return PartialView("FavoriteCategories", favCatRepository.FindByUserId(int.Parse(User.Identity.GetUserId())));
        }

        public bool RemoveFromFavoriteCategory(int id)
        {
            //favCatRepository.Get()
            //favCatRepository.Remove();
            return false;
        }
        
    }
}