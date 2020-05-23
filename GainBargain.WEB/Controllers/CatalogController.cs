using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class CatalogController : Controller
    {
        ISuperCategoryRepository superCategoryRepo;
        IRepository<Category> categoryRepo;
        IProductRepository productRepo;

        public CatalogController()
        {
            superCategoryRepo = new SuperCategoryRepository(new GainBargainContext());
            categoryRepo = new Repository<Category>(new GainBargainContext());
            productRepo = new ProductRepository(new GainBargainContext());
        }

        public ActionResult Categories()
        {
            return View(superCategoryRepo.GetSuperCategoriesWithCategories());
        }

        public ActionResult Catalog(string type, int? categoryId)
        {
            if (type == null || categoryId == null)
            {
                return HttpNotFound();
            }
            if (type == "superCategory")
            {
                return View(productRepo.ProductsWithSameSuperCategory((int)categoryId));
            }
            return View(productRepo.Find(p => p.CategoryId == categoryId));
        }
    }
}