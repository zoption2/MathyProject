using Dapper;
using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mono.Data.Sqlite;

namespace Mathy.Services.Data
{
    public interface IGeneralResultsProvider : IDataProvider
    {
        UniTask<GeneralTasksViewData> GetGeneralTasksDataAsync();
        UniTask<DetailedTasksViewData> GetDetailedTasksDataAsync(TaskType taskType);
        UniTask<DailyModeViewData> GetDailyModeDataAsync(TaskMode mode);
    }


    public class GeneralResultsProvider : BaseDataProvider, IGeneralResultsProvider
    {
        public GeneralResultsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<GeneralTasksViewData> GetGeneralTasksDataAsync()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var data = new GeneralTasksViewData();
                var requestModel = data.ConvertToModel();
                var model = await connection.QueryFirstOrDefaultAsync<GeneralTasksViewModel>
                        (GeneralResultsTableRequests.SelectFromGeneralTasksViewQuery, requestModel);
                if (model == null)
                {
                    return data;
                }
                var result = model.ConvertToData();
                return result;
            }
        }

        public async UniTask<DetailedTasksViewData> GetDetailedTasksDataAsync(TaskType taskType)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var data = new DetailedTasksViewData() { TaskType = taskType };
                var requestModel = data.ConvertToModel();
                var model = await connection.QueryFirstOrDefaultAsync<DetailedTasksViewModel>
                        (GeneralResultsTableRequests.SelectFromGeneralTasksViewQuery, requestModel);
                if (model == null)
                {
                    return data;
                }
                var result = model.ConvertToData();
                return result;
            }
        }

        public async UniTask<DailyModeViewData> GetDailyModeDataAsync(TaskMode mode)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var data = new DailyModeViewData() { Mode = mode };
                var requestModel = data.ConvertToModel();
                var model = await connection.QueryFirstOrDefaultAsync<DailyModeViewModel>
                        (GeneralResultsTableRequests.SelectFromGeneralTasksViewQuery, requestModel);
                if (model == null)
                {
                    return data;
                }
                var result = model.ConvertToData();
                return result;
            }
        }


        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                //We can delete view on game start, if potentially they will changed often,
                //and then create new ones. It's fast and save methods with no data lost.
                await connection.ExecuteAsync(GeneralResultsTableRequests.DropGeneralTasksViewQuery);
                await connection.ExecuteAsync(GeneralResultsTableRequests.DropDetailedTasksViewQuery);
                await connection.ExecuteAsync(GeneralResultsTableRequests.DropDailyModeViewQuery);

                await connection.ExecuteAsync(GeneralResultsTableRequests.CreateGeneralView);
                await connection.ExecuteAsync(GeneralResultsTableRequests.CreateDetailedView);
                await connection.ExecuteAsync(GeneralResultsTableRequests.CreateModeView);
            }
        }
    }

}

