using System;
using Dapper;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;

namespace Mathy.Services.Data
{
    public interface IDailyModeProvider : IDataProvider
    {
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
    }


    public class DailyModeProvider : BaseDataProvider, IDailyModeProvider
    {
        public DailyModeProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var dataModel = data.ConvertToModel();
                var exists = await connection.QueryFirstOrDefaultAsync<DailyModeTableModel>(DailyModeTableRequests.SelectDailyQuery, dataModel);
                var query = exists != null ? DailyModeTableRequests.UpdateDailyQuery : DailyModeTableRequests.InsertDailyQuery;
                await connection.ExecuteAsync(query, dataModel);
            }
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
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
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(DailyModeTableRequests.TryCreateDailyModeTableQuery);
            }
        }
    }

}

