using System.Data;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;

namespace Mathy.Services
{
    public interface IDataProvider
    {
        UniTask TryCreateTable();
        UniTask DeleteTable();
    }

    public abstract class BaseDataProvider : IDataProvider
    {
        protected string _dbFilePath;
        public BaseDataProvider(string dbFilePath)
        {
            _dbFilePath = dbFilePath;
        }

        public abstract UniTask TryCreateTable();

        public async virtual UniTask DeleteTable()
        {
            await UniTask.CompletedTask;
        }
    }
}

