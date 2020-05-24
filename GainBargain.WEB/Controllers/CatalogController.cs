using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.WEB.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class CatalogController : Controller
    {
        ISuperCategoryRepository superCategoryRepo;
        IRepository<Category> categoryRepo;
        IProductRepository productRepo;
        IMarketRepository marketRepo;

        public CatalogController()
        {
            superCategoryRepo = new SuperCategoryRepository(new GainBargainContext());
            categoryRepo = new Repository<Category>(new GainBargainContext());
            productRepo = new ProductRepository(new GainBargainContext());
            marketRepo = new MarketRepository(new GainBargainContext());
        }

        public ActionResult Categories()
        {
            return View(superCategoryRepo.GetAllSuperCategoriesWithCategories());
        }

        public ActionResult Catalog(int? superCategoryId, int? categoryId)
        {
            if (superCategoryId == null)
            {
                return HttpNotFound();
            }

            var sc = superCategoryRepo.GetSuperCategoryWithCategories((int)superCategoryId);

            if (sc == null) 
            {
                return HttpNotFound();
            }

            CatalogVM catalog = new CatalogVM();
            catalog.SuperCategoryId = sc.Id;
            catalog.SuperCategoryName = sc.Name;
            catalog.Categories = sc.Categories;
            catalog.Markets = marketRepo.FindMarketsBySuperCategory(sc.Id);

            ViewBag.CategoryId = categoryId;


            return View(catalog);
        }

        public ActionResult Products(int? superCategoryId, int? categoryId)
        {
            if (superCategoryId == null)
                return HttpNotFound();
            if (categoryId != null) {
                return PartialView(productRepo.Find(p => p.CategoryId == categoryId));
            }
            return PartialView(productRepo.ProductsWithSameSuperCategory((int)superCategoryId));
        }

    }
}