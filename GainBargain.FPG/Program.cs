using GainBargain.DAL.Repositories;

namespace GainBargain.FPG
{
    class Program
    {
        static void Main(string[] args)
        {
            FavoriteProductRepository repo = new FavoriteProductRepository();
            var r = repo.GetAllTransactions();
            //Tree<int> tree = new Tree<int>(repo.GetAllTransactions(), );
        }
    }
}
