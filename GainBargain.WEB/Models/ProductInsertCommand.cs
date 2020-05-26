using GainBargain.DAL.EF;
using GainBargain.DAL.Entities;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace GainBargain.WEB.Models
{
    internal class ProductInsertCommand : IDisposable
    {
        private DbConnection connection;
        private DbTransaction transaction;
        private DbCommand command;

        private bool madeRollback = false;

        public ProductInsertCommand(GainBargainContext db)
        {
            connection = db.Database.Connection;
            connection.Open();
            transaction = connection.BeginTransaction();
            command = connection.CreateCommand();

            command.Transaction = transaction;
            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO Products(Name, Price, UploadTime, ImageUrl, CategoryId, MarketId)"
                                + "VALUES(@name, @price, @uploadTime, @imageUrl, @categoryId, @marketId);";

            command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar));
            command.Parameters.Add(new SqlParameter("@price", SqlDbType.Real));
            command.Parameters.Add(new SqlParameter("@uploadTime", SqlDbType.DateTime));
            command.Parameters.Add(new SqlParameter("@imageUrl", SqlDbType.NVarChar));
            command.Parameters.Add(new SqlParameter("@categoryId", SqlDbType.Int));
            command.Parameters.Add(new SqlParameter("@marketId", SqlDbType.Int));
        }

        public void ExecuteOn(Product p)
        {
            try
            {
                command.Parameters["@name"].Value = p.Name;
                command.Parameters["@price"].Value = p.Price;
                command.Parameters["@uploadTime"].Value = p.UploadTime;
                command.Parameters["@imageUrl"].Value = p.ImageUrl;
                command.Parameters["@categoryId"].Value = p.CategoryId;
                command.Parameters["@marketId"].Value = p.MarketId;

                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                connection.Close();
                madeRollback = true;

                throw ex;
            }
        }

        public void Dispose()
        {
            if (!madeRollback)
            {
                transaction.Commit();
                connection.Close();
            }
        }
    }
}