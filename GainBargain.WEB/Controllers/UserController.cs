using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.WEB.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace GainBargain.WEB.Controllers
{
    public class UserController : Controller
    {
        private IFavoriteCategoriesRepository favCatRepository;
        private IRepository<FavoriteProduct> favProductRepository;
        private IRepository<User> userRepository;
        private IRepository<Category> categoryRepository;
        private IProductCacheRepository productRepository;
        private GainBargainContext db;
        public UserController()
        {
            favCatRepository = new FavoriteCategoriesRepository();
            favProductRepository = new Repository<FavoriteProduct>(new GainBargainContext());
            userRepository = new Repository<User>(new GainBargainContext());
            productRepository = new ProductsCacheRepository(new GainBargainContext());
            categoryRepository = new Repository<Category>(new GainBargainContext());
            db = new GainBargainContext();
        }
        // GET: User
        public ActionResult Index()
        {
            var model = new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                FavoriteCategories = favCatRepository.FindByUserName(User.Identity.Name)
                //FavoriteProducts = favCatRepository.Find(x => x.UserId == int.Parse(User.Identity.GetUserId()))
            };

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult FavoriteCategoriesPartial()
        {
            SelectList categories = new SelectList(categoryRepository.GetAll(), "Id", "Name");
            ViewBag.Categories = categories;
            return PartialView("FavoriteCategories", favCatRepository.FindByUserName(User.Identity.Name));
        }

        public void RemoveFromFavoriteCategory(int id)
        {
            favCatRepository.RemoveFromFavoriteCategory(id, User.Identity.Name);
        }

        [HttpPost]
        public void AddFavoriteCategory(string id)
        {
            if (id == null)
            {
                return;
            }

            var userId = userRepository.Find(x=>x.Email == User.Identity.Name).FirstOrDefault().Id;
            var favCat = new FavoriteCategory()
            {
                UserId = userId,
                CategoryId = int.Parse(id)
            };

            favCatRepository.AddFavoriteCategory(favCat);
        }

        [ChildActionOnly]
        public ActionResult FavoriteProductsPartial(int subCategoryId)
        {
            var products = productRepository.GetTopProducts(4, subCategoryId);
            return PartialView("FavoriteProduct", products);
        }

    }
}