﻿using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;
using GainBargain.WEB.Models;
using System.Web.Mvc;

namespace GainBargain.WEB.Controllers
{
    public class CatalogController : Controller
    {
        ISuperCategoryRepository superCategoryRepo;
        IRepository<Category> categoryRepo;
        IProductRepository productRepo;
        IMarketRepository marketRepo;

        const int pageSize = 32;
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

            catalog.AvailableCategories = sc.Categories;


            catalog.Pager = new Pager(countProducts, page, pageSize, 3);

            //catalog.PageInfo = new PageInfo()
            //{
            //    PageNumber = page,
            //    PageSize = pageSize,
            //    TotalItems = countProducts
            //};

            if (categoryId != null)
                catalog.SelectedCategories.Add(categoryId.Value);

            return View(catalog);
        }

        [HttpPost]
        public ActionResult Catalog(CatalogVM model, int page = 1)
        {
            var sc = superCategoryRepo.GetSuperCategoryWithCategories(model.SuperCategoryId);

            int countProducts;

            model.Products = productRepo.GetProductsPerPage
                (page, pageSize, model.SuperCategoryId, model.SelectedCategories, out countProducts);

            model.AvailableCategories = sc.Categories;

            model.Pager = new Pager(countProducts, page, pageSize, 3);

            return View(model);

        }

        //public ActionResult Products(int? superCategoryId, int? categoryId)
        //{
        //    if (superCategoryId == null)
        //        return HttpNotFound();
        //    if (categoryId != null) {
        //        return PartialView(productRepo.Find(p => p.CategoryId == categoryId));
        //    }
        //    return PartialView(productRepo.ProductsWithSameSuperCategory((int)superCategoryId));
        //}

    }
}