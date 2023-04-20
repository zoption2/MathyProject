using System.Data;
using Cysharp.Threading.Tasks;


namespace Mathy.Services
{
    public interface IDataProvider
    {
        UniTask TryCreateTable(IDbConnection connection);
    }

}

