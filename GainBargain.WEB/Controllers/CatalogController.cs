using AutoMapper;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.WEB.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class CatalogController : Controller
    {
        ISuperCategoryRepository superCategoryRepo;
        IRepository<Category> categoryRepo;
        IProductCacheRepository productRepo;
        IMarketRepository marketRepo;

        const int pageSize = 32;
        public CatalogController()
        {
            superCategoryRepo = new SuperCategoryRepository(new GainBargainContext());
            categoryRepo = new Repository<Category>(new GainBargainContext());
            productRepo = new ProductsCacheRepository(new GainBargainContext());
            marketRepo = new MarketRepository(new GainBargainContext());
        }

        public ActionResult Categories()
        {
            return View(superCategoryRepo.GetAllSuperCategoriesWithCategories());
        }

        [HttpGet]
        public ActionResult Search(string text, int page = 1)
        {
            if (text == null)
            {
                return HttpNotFound();
            }
            CatalogVM catalog = new CatalogVM();
            int countProducts = 0;
            if (!String.IsNullOrEmpty(text))
            {
                catalog.Products = productRepo.SearchProduct(page, pageSize, text, out countProducts);
            }
            catalog.CountProducts = countProducts;
            catalog.Pager = new Pager(countProducts, page, pageSize, 3);
            catalog.Search = text;
            return View("Catalog", catalog);
        }

        [HttpPost]
        public ActionResult Search(CatalogVM catalog, int page = 1)
        {
            int countProducts;
            catalog.Products = productRepo.SearchProduct(page, pageSize, catalog.Search, out countProducts);
            catalog.CountProducts = countProducts;
            if (catalog.SortOrder != null)
            {
                catalog.Products = catalog.SortOrder.Value ? catalog.Products.OrderBy(p => p.Price)
                    : catalog.Products.OrderByDescending(p => p.Price);
            }
            catalog.Pager = new Pager(countProducts, page, pageSize, 3);
            return View("Catalog", catalog);
        }

        public ActionResult Catalog(int? superCategoryId, int? categoryId, int page = 1)
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

            int countProducts;

            catalog.Products = productRepo.GetProductsPerPage
                (page, pageSize, superCategoryId.Value, categoryId ?? null, out countProducts);

            catalog.CountProducts = countProducts;

            catalog.AvailableCategories = sc.Categories;


            catalog.Pager = new Pager(countProducts, page, pageSize, 3);

            if (categoryId != null)
                catalog.SelectedCategories.Add(categoryId.Value);

            return View(catalog);
        }

        [HttpPost]
        public ActionResult Catalog(CatalogVM model, int page = 1)
        {
            var sc = superCategoryRepo.GetSuperCategoryWithCategories(model.SuperCategoryId);

            model.SuperCategoryName = sc.Name;

            int countProducts;

            model.Products = productRepo.GetProductsPerPage
                (page, pageSize, model.SuperCategoryId, model.SelectedCategories, out countProducts);

            model.CountProducts = countProducts;

            model.AvailableCategories = sc.Categories;

            model.Pager = new Pager(countProducts, page, pageSize, 3);

            if (model.SortOrder != null)
            {
                model.Products = model.SortOrder.Value ? model.Products.OrderBy(p => p.Price) 
                    : model.Products.OrderByDescending(p => p.Price);
            }

            return View(model);

        }

        public ActionResult Product(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var product = productRepo.Get(id.Value);
            var market = marketRepo.Get(product.MarketId);
            ProductVM productVM = new ProductVM(id.Value, product.Name, product.ImageUrl, product.Price, product.PrevPrice,
                market.Id, market.Name, market.MarketLogoUrl);
            return View(productVM);
        }

        public ActionResult Market(int? id)
        {
            if (id == null)
                return HttpNotFound();
            
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Market, MarketVM>()));
            var marketVM = mapper.Map<Market, MarketVM>(marketRepo.Get(id.Value));

            return View(marketVM);
        }
    }
}