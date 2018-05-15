using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using Npgsql;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Web;
using System.Configuration;

namespace PrototypeWebBlockchain.Repository
{
    public class TransactionRepository : ITransactionRepository<TransactionData>
    {

        private string connectionString;
        public TransactionRepository()
        {
            connectionString = ConfigurationManager.AppSettings["PostgresConnection"];
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }

        public void Add(TransactionData transaction)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO transaction_t (id, member_id, filename, filepath, date) VALUES(@id, @member_id, @filename, @filepath,@date)", transaction);
            }
        }

        public IEnumerable<TransactionData> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var list = dbConnection.Query<TransactionData>("SELECT * FROM transaction_t");
                return list;
            }
        }

        public IEnumerable<int> NextID()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var id = dbConnection.Query<int>("SELECT nextval('transaction_t_id_seq')");
                return id;
            }
        }

        public IEnumerable<TransactionData> FindByID(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var list = dbConnection.Query<TransactionData>("SELECT * FROM transaction_t WHERE member_id = @Id", new { Id = id });
                return list;
            }
        } 

    }
}