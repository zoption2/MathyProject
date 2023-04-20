using Dapper;
using System.Data;
using Cysharp.Threading.Tasks;


namespace Mathy.Services
{
    public class GeneralResultsProvider
    {
        public async UniTask<GeneralResultsData> GetDataAsync(IDbConnection connection)
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

        public async UniTask SaveAsync(GeneralResultsData data, IDbConnection connection)
        {
            var requestModel = data.ConvertToModel();
            await connection.ExecuteAsync(GeneralResultsTableRequests.UpdateQuery, requestModel);
        }

        public async UniTask TryCreateTable(IDbConnection connection)
        {
            await connection.ExecuteAsync(GeneralResultsTableRequests.TryCreateTableQuery);
        }
    }

}

