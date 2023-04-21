using Dapper;
using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mono.Data.Sqlite;

namespace Mathy.Services.Data
{
    public interface IGeneralResultsProvider : IDataProvider
    {
        UniTask<GeneralResultsData> GetDataAsync();
        UniTask SaveAsync(GeneralResultsData data);
    }


    public class GeneralResultsProvider : BaseDataProvider, IGeneralResultsProvider
    {
        public GeneralResultsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<GeneralResultsData> GetDataAsync()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var data = new GeneralResultsData();
                var requestModel = data.ConvertToModel();
                var model = await connection.QueryFirstOrDefaultAsync<GeneralResultsTableModel>
                        (GeneralResultsTableRequests.SelectQuery, requestModel);
                if (model == null)
                {
                    await connection.ExecuteAsync(GeneralResultsTableRequests.InsertQuery, requestModel);
                    return data;
                }
                var result = model.ConvertToData();
                return result;
            }
        }

        public async UniTask SaveAsync(GeneralResultsData data)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var requestModel = data.ConvertToModel();
                await connection.ExecuteAsync(GeneralResultsTableRequests.UpdateQuery, requestModel);
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(GeneralResultsTableRequests.TryCreateTableQuery);
                await connection.ExecuteAsync(GeneralResultsTableRequests.CreateView);
                await connection.ExecuteAsync(GeneralResultsTableRequests.CreateMultiViews);
            }
        }
    }

}

