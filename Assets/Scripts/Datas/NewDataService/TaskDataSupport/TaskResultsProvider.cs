using System;
using System.Linq;
using Dapper;
using System.Data;
using Mathy.Data;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;

namespace Mathy.Services.Data
{
    public interface ITaskResultsProvider : IDataProvider
    {
        UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date);
        UniTask<int> SaveTask(TaskResultData task);
    }

    public class TaskResultsProvider : BaseDataProvider, ITaskResultsProvider
    {
        public TaskResultsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date)
        {
            using(var connection = new SqliteConnection(_dbFilePath))
            {
                var requestData = new TaskResultData() {Date = date, Mode = mode};
                var requestModel = requestData.ConvertToModel();
                var tableModels = await connection.QueryAsync<TaskDataTableModel>(TaskResultsTableRequests.SelectTaskByModeAndDateQuery, requestModel);
                var result = tableModels.Select(x => x.ConvertToData()).ToArray();
                return result;
            }
        }

        public async UniTask<int> SaveTask(TaskResultData task)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var dataModel = task.ConvertToModel();
                var query = TaskResultsTableRequests.InsertTaskQuery;
                var id = await connection.QueryFirstOrDefaultAsync<int>(query, dataModel);
                return id;
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(TaskResultsTableRequests.TryCreateTasksDataTableQuery);
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(TaskResultsTableRequests.DeleteTable);
            }
        }
    }

}

