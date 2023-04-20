using System;
using System.Linq;
using Dapper;
using System.Data;
using Mathy.Data;
using Cysharp.Threading.Tasks;


namespace Mathy.Services
{
    public interface ITaskResultsProvider : IDataProvider
    {
        UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date, IDbConnection connection);
        UniTask<int> SaveTask(TaskResultData task, IDbConnection connection);
    }

    public class TaskResultsProvider : ITaskResultsProvider
    {
        public async UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date, IDbConnection connection)
        {
            var requestData = new TaskResultData()
            {
                Date = date,
                Mode = mode
            };
            var requestModel = requestData.ConvertToModel();
            var tableModels = await connection.QueryAsync<TaskDataTableModel>(TaskResultsTableRequests.SelectTaskByModeAndDateQuery, requestModel);
            var result = tableModels.Select(x => x.ConvertToData()).ToArray();
            return result;
        }

        public async UniTask<int> SaveTask(TaskResultData task, IDbConnection connection)
        {
            var dataModel = task.ConvertToModel();
            var query = TaskResultsTableRequests.InsertTaskQuery;
            var id = await connection.QueryFirstOrDefaultAsync<int>(query, dataModel);
            return id;
        }

        public async UniTask TryCreateTable(IDbConnection connection)
        {
            await connection.ExecuteAsync(TaskResultsTableRequests.TryCreateTasksDataTableQuery);
        }
    }

}

