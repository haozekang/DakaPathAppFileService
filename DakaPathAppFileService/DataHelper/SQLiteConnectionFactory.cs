using Chloe.Infrastructure;
using System.Data;
using System.Data.SQLite;

namespace DakaPathAppFileService.DataHelper
{
    public class SQLiteConnectionFactory : IDbConnectionFactory
    {
        string _connString = null;

        public SQLiteConnectionFactory(string connString)
        {
            this._connString = connString;
        }

        public IDbConnection CreateConnection()
        {
            IDbConnection conn = new SQLiteConnection(this._connString);
            return conn;

        }
    }
}