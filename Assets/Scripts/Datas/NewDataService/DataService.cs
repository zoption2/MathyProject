using System;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Linq;
using Dapper;
using System.Data;
using System.IO;
using Mathy.Data;
using Cysharp.Threading.Tasks;

namespace Mathy.Services
{
    public interface IDataService
    { 
        ITaskDataProvider Task { get; }
    }


    public class DataService : IDataService
    {
        private const string kFileFormat = "TasksSave_{0}.db";
        private const string kVersion = "0.0.1";

        private TaskDataProvider _task;

        private string dataPath = Application.persistentDataPath;
        private string saveDirectoryPath;
        private string saveFilePath;
        private string databasePath;

        public ITaskDataProvider Task => _task;


        public DataService()
        {
            saveDirectoryPath = dataPath + "/Saves/";
            var currentYear = DateTime.Now.Year;
            var fileName = string.Format(kFileFormat, currentYear);
            saveFilePath = saveDirectoryPath + fileName;
            databasePath = $"Data Source={saveFilePath}";
            if (!Directory.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
            }

            _task = new TaskDataProvider(databasePath);
        }
    }


    public interface ITaskDataProvider
    {
        TaskData[] GetTasksByMode(TaskMode mode);
        UniTask SaveTask(TaskData task);
        UniTask UpdateDailyMode(DailyModeData data);
    }

    public class TaskDataProvider : ITaskDataProvider
    {
        private const string kTableName = "TasksData";

        private string databasePath;
        private static IDbConnection _dbConnection;


        public TaskDataProvider(string databasePath)
        {
            this.databasePath = databasePath;
            TryCreateTasksTable();
            //TryCreateDailyModeTable();
        }

        public TaskData[] GetTasksByMode(TaskMode mode)
        {
            try
            {
                using (_dbConnection = new SqliteConnection(databasePath))
                {
                    var query = TaskDataUtils.SelectByModeQuery;
                    return _dbConnection.Query<TaskData>(query, mode).ToArray();
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0} error: " + e.ToString(), nameof(GetTasksByMode));
                return default;
            }

        }

        public async UniTask SaveTask(TaskData task)
        {
            try
            {
                using (_dbConnection = new SqliteConnection(databasePath))
                {
                    var dataModel = task.ToTaskTableData();
                    var query = TaskDataUtils.InsertQuery;
                    var id = await _dbConnection.QueryAsync<int>(query, dataModel);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0} error: " + e.ToString(), nameof(SaveTask));
            }

        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            try
            {
                using (_dbConnection = new SqliteConnection(databasePath))
                {
                    var dataModel = data.ToDailyModeTableData();
                    var exists = _dbConnection.ExecuteScalar<int>(TaskDataUtils.SelectDailyQuery, dataModel);
                    var query = exists > 0 ? TaskDataUtils.UpdateDailyQuery : TaskDataUtils.InsertQuery;
                    await _dbConnection.ExecuteAsync(query, dataModel);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0} error: " + e.ToString(), nameof(UpdateDailyMode));
            }
        }

        private void TryCreateTasksTable()
        {
            try
            {
                using (_dbConnection = new SqliteConnection(databasePath))
                {
                    _dbConnection.Open();
                    var transaction = _dbConnection.BeginTransaction();
                    _dbConnection.Execute(TaskDataUtils.CreateTasksDataTableQuery, transaction);
                    _dbConnection.Execute(TaskDataUtils.CreateDailyModeTableQuery, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0} error: " + e.ToString(), nameof(TryCreateTasksTable));
            }
        }

        private void TryCreateDailyModeTable()
        {
            try
            {
                using (_dbConnection = new SqliteConnection(databasePath))
                {
                    _dbConnection.Execute(TaskDataUtils.CreateDailyModeTableQuery);
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0} error: " + e.ToString(), nameof(TryCreateTasksTable));
            }
        }
    }


    [Serializable]
    public class DailyModeData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TaskMode Mode { get; set; }
        public bool IsComplete { get; set; }
        public int LastIndex { get; set; }
    }

    [Serializable]
    public class DailyModeTableModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Mode { get; set; }
        public int ModeIndex { get; set; }
        public bool IsComplete { get; set; }
        public int LastIndex { get; set; }
    }
}

