using System.Data;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;

namespace Mathy.Services
{
    public interface IDataProvider
    {
        UniTask TryCreateTable();
    }

    public abstract class BaseDataProvider : IDataProvider
    {
        protected string _dbFilePath;
        public BaseDataProvider(string dbFilePath)
        {
            _dbFilePath = dbFilePath;
        }

        public abstract UniTask TryCreateTable();

        protected void OpenConnection(out IDbConnection connection)
        {
            connection = new SqliteConnection(_dbFilePath);
        }
    }
}

