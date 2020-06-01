using System.Collections.Generic;
using GainBargain.DAL.EF;
using System.Web.Mvc;
using System.Linq;
using GainBargain.DAL.Interfaces;
using GainBargain.DAL.Repositories;

namespace GainBargain.WEB.Controllers
{
    public class HomeController : Controller
    {
        GainBargainContext db = new GainBargainContext();

        public ActionResult Index()
        {
            //var db = new GainBargainContext();

            IProductCacheRepository rep = new ProductsCacheRepository(db);

            var productList = rep.GetTopProducts(12)
                .ToList();

            ///////////////////////////////////
            //FavoriteProductRepository repo = new FavoriteProductRepository();
            //AssociationRepository ass = new AssociationRepository();
            //TreeBuilder.Build(repo.GetAllTransactions());

            //Tree<int> conditionalTree = TreeBuilder.Tree.GetConditionalTree(95510);
            //List<int> result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95510, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(591);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(591, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(596);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(596, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(1754);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(1754, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(55497);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(55497, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(66665);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(66665, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(67002);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(67002, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(67011);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(67011, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(67000);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(67000, result);


            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(89784);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(89784, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94614);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94614, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94629);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94629, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94635);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94635, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94638);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94638, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94647);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94647, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94662);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94662, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94663);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94663, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94666);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94666, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(94833);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(94833, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95051);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95051, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95066);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95066, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95081);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95081, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95131);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95131, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95145);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95145, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95166);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95166, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95248);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95248, result);

            //conditionalTree = TreeBuilder.Tree.GetConditionalTree(95281);
            //result = conditionalTree.GetAssociations(conditionalTree.Root);
            //ass.CreateAssociation(95281, result);


            ///////////////////////////////////////////

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