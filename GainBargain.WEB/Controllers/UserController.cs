using System.Linq;
using System.Web.Mvc;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.WEB.Models;

namespace GainBargain.WEB.Controllers
{
    public class UserController : Controller
    {
        private IFavoriteCategoriesRepository favCatRepository;
        private IFavoriteProductRepository favProdRepository;
        private IRepository<User> userRepository;
        private IRepository<Category> categoryRepository;
        private IProductCacheRepository productRepository;
        private GainBargainContext db;
        public UserController()
        {
            favCatRepository = new FavoriteCategoriesRepository();
            favProdRepository = new FavoriteProductRepository();
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
            var cat = categoryRepository.GetAll()
                .Where(x => favCatRepository.FindByUserName(User.Identity.Name).All(y => y.CategoryId != x.Id));
            SelectList categories = new SelectList(cat, "Id", "Name");
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

        public void AddToFavoriteProduct(string productId)
        {
            var userId = userRepository.Find(x => x.Email == User.Identity.Name).FirstOrDefault().Id;
            var favProduct = new FavoriteProduct()
            {
                UserId = userId,
                ProductId = int.Parse(productId)
            };

            favProdRepository.AddFavoriteProduct(favProduct);
        }


        public ActionResult FavoriteProducts()
        {
            var products = favProdRepository.FindByUserName(User.Identity.Name);
            return PartialView("FavoriteProducts2", products);
        }

        public void RemoveFromFavoriteProduct(int id)
        {
            favProdRepository.RemoveFromFavoriteProducts(id, User.Identity.Name);
        }

    }
}