using System;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Linq;
using Dapper;
using System.Data;
using System.IO;
using Mathy.Data;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor.Search;
using ModestTree;

namespace Mathy.Services
{
    public interface IDataService
    { 
        ITaskDataProvider TaskData { get; }
    }


    public class DataService : IDataService
    {
        private TaskDataProvider _task;

        private readonly string dataPath = Application.persistentDataPath;
        private string databasePath;

        public ITaskDataProvider TaskData => _task;


        public DataService()
        {
            databasePath = dataPath + "/Saves/";
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
            }

            _task = new TaskDataProvider(databasePath);
        }
    }


    public interface ITaskDataProvider
    {
        UniTask<TaskData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date);
        UniTask<int> SaveTask(TaskData task);
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
    }

    public class TaskDataProvider : ITaskDataProvider
    {
        private const string kFileFormat = "tasks_results_save_{0}.db";

        private string _databasePath;
        private string _directoryPath;
        private int _currentYear;
        private static IDbConnection _dbConnection;


        public TaskDataProvider(string directoryPath)
        {
            _directoryPath = directoryPath;
            _currentYear = DateTime.UtcNow.Year;
            var fileName = string.Format(kFileFormat, _currentYear);
            var saveFilePath = directoryPath + fileName;
            _databasePath = $"Data Source={saveFilePath}";
            TryCreateTables();
        }

        public async UniTask<TaskData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date)
        {
            var databasePath = GetPathToDatabase(date);
            if(databasePath.IsEmpty())
            {
                return new TaskData[0];
            }

            using (_dbConnection = new SqliteConnection(databasePath))
            {
                var requestData = new TaskData()
                {
                    Date = date,
                    Mode = mode
                };
                var requestModel = requestData.ToTaskTableData();
                var tableModels = await _dbConnection.QueryAsync<TaskDataTableModel>(TaskDataUtils.SelectTaskByModeAndDateQuery, requestModel);
                var result = tableModels.Select(x => x.ToTaskData()).ToArray();
                return result;
            }
        }

        public async UniTask<int> SaveTask(TaskData task)
        {
            using (_dbConnection = new SqliteConnection(_databasePath))
            {
                var dataModel = task.ToTaskTableData();
                var query = TaskDataUtils.InsertTaskQuery;
                var id = await _dbConnection.QueryFirstOrDefaultAsync<int>(query, dataModel);
                return id;
            }
        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            using (_dbConnection = new SqliteConnection(_databasePath))
            {
                var dataModel = data.ToDailyModeTableData();
                var exists = await _dbConnection.QueryFirstOrDefaultAsync<DailyModeTableModel>(TaskDataUtils.SelectDailyQuery, dataModel);
                var query = exists != null ? TaskDataUtils.UpdateDailyQuery : TaskDataUtils.InsertDailyQuery;
                await _dbConnection.ExecuteAsync(query, dataModel);
            }
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {
            var requestData = new DailyModeData() { Date = date, Mode = mode };

            var databasePath = GetPathToDatabase(date);
            if (databasePath.IsEmpty())
            {
                return requestData;
            }

            using (_dbConnection = new SqliteConnection(_databasePath))
            {
                
                var requestModel = requestData.ToDailyModeTableData();
                var model = await _dbConnection.QueryFirstOrDefaultAsync<DailyModeTableModel>
                    (TaskDataUtils.SelectDailyQuery, requestModel);
                if (model == null)
                {
                    await _dbConnection.ExecuteAsync(TaskDataUtils.InsertDailyQuery, requestModel);
                    return requestData;
                }
                var result = model.ToDailyModeData();
                return result;
            }
        }

        private void TryCreateTables()
        {
            using (_dbConnection = new SqliteConnection(_databasePath))
            {
                _dbConnection.Open();
                var transaction = _dbConnection.BeginTransaction();
                _dbConnection.Execute(TaskDataUtils.CreateTasksDataTableQuery, transaction);
                _dbConnection.Execute(TaskDataUtils.CreateDailyModeTableQuery, transaction);
                transaction.Commit();
            }
        }

        private string GetPathToDatabase(DateTime date)
        {
            if (date.Year == _currentYear)
            {
                return _databasePath;
            }
            else
            {
                var fileName = string.Format(kFileFormat, date.Year);
                var saveFilePath = _directoryPath + fileName;
                if (File.Exists(saveFilePath))
                {
                    var selectedDatabasePath = $"Data Source={fileName}";
                    return selectedDatabasePath;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}

