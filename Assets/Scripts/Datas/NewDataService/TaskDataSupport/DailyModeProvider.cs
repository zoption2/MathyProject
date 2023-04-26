using System;
using Dapper;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace Mathy.Services.Data
{
    public interface IDailyModeProvider : IDataProvider
    {
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
        UniTask<List<DailyModeData>> GetDailyData(DateTime date);
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
                var exists = await connection.QueryFirstOrDefaultAsync<DailyModeTableModel>(DailyModeTableRequests.SelectByDateAndModeQuery, dataModel);
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
                    (DailyModeTableRequests.SelectByDateAndModeQuery, requestModel);
                if (model == null)
                {
                    return requestData;
                }
                var result = model.ConvertToData();
                return result;
            }
        }

        public async UniTask<List<DailyModeData>> GetDailyData(DateTime date)
        {
            List<DailyModeData> results = new List<DailyModeData>();
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var requestData = new DailyModeData() { Date = date };
                var requestModel = requestData.ConvertToModel();
                var models = await connection.QueryAsync<DailyModeTableModel>
                    (DailyModeTableRequests.SelectByDateQuery, requestModel);
                var modelsArray = models.ToArray();
                for (int i = 0, j = modelsArray.Length; i < j; i++)
                {
                    var data = modelsArray[i].ConvertToData();
                    results.Add(data);
                }
                return results;
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(DailyModeTableRequests.TryCreateDailyModeTableQuery);
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(DailyModeTableRequests.DeleteTable);
            }
        }
    }

}

