using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;

namespace GainBargain.DAL.Repositories
{
    public class AssociationRepository
    {
        private GainBargainContext db;
        public AssociationRepository()
        {
            db = new GainBargainContext();
        }

        public void CreateAssociation(int productId, List<int> associations)
        {
            foreach (var ass in associations)
            {
                if (ass != 0)
                {
                    var p = new Association() { AccosiationId = ass, ProductId = productId };
                    db.Associations.Add(p);
                }
                
                db.SaveChanges();
            }
        }

        public IEnumerable<Association> GetAssociations(int productId)
        {
            return db.Associations.Include("ProductCache").Where(x=>x.ProductId == productId).ToList();
        }
    }
}
