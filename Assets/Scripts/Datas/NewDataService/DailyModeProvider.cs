using System;
using Dapper;
using System.Data;
using Cysharp.Threading.Tasks;


namespace Mathy.Services
{
    public class DailyModeProvider
    {
        public async UniTask UpdateDailyMode(DailyModeData data, IDbConnection connection)
        {
            var dataModel = data.ConvertToModel();
            var exists = await connection.QueryFirstOrDefaultAsync<DailyModeTableModel>(DailyModeTableRequests.SelectDailyQuery, dataModel);
            var query = exists != null ? DailyModeTableRequests.UpdateDailyQuery : DailyModeTableRequests.InsertDailyQuery;
            await connection.ExecuteAsync(query, dataModel);
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode, IDbConnection connection)
        {
            var requestData = new DailyModeData() { Date = date, Mode = mode };
            var requestModel = requestData.ConvertToModel();
            var model = await connection.QueryFirstOrDefaultAsync<DailyModeTableModel>
                (DailyModeTableRequests.SelectDailyQuery, requestModel);
            if (model == null)
            {
                await connection.ExecuteAsync(DailyModeTableRequests.InsertDailyQuery, requestModel);
                return requestData;
            }
            var result = model.ConvertToData();
            return result;
        }

        public async UniTask TryCreateTable(IDbConnection connection)
        {
            await connection.ExecuteAsync(DailyModeTableRequests.TryCreateDailyModeTableQuery);
        }
    }

}

