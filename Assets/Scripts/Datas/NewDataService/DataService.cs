using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Linq;
using Dapper;
using System.Data;
using System.IO;

namespace Mathy.Services
{
    public interface IDataService
    { 
        ITaskDataProvider Task { get; }
    }


    public class DataService : IDataService
    {
        private const string kFileName = "Save.db";
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
            saveFilePath = saveDirectoryPath + kFileName;
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
        NewTaskData[] GetTasksByMode(TaskMode mode);
        void InsertTask(NewTaskData task);
    }

    public class TaskDataProvider : ITaskDataProvider
    {
        private string databasePath;

        private static readonly IDbConnection _dbConnection;


        public TaskDataProvider(string databasePath)
        {
            this.databasePath = databasePath;
            _dbConnection.ConnectionString = databasePath;
            TryCreateTable();
        }

        public NewTaskData[] GetTasksByMode(TaskMode mode)
        {
            using (_dbConnection)
            {
                var query = TaskDataUtils.SelectByModeQuery;
                return _dbConnection.Query<NewTaskData>(query, mode).ToArray();
            }
        }

        public void InsertTask(NewTaskData task)
        {
            using (_dbConnection)
            {
                var query = TaskDataUtils.InsertQuery;
            }
        }

        private void TryCreateTable()
        {
            using(_dbConnection)
            {
                var query = TaskDataUtils.CreateTableQuery;
                _dbConnection.Execute(query);
            }
        }
    }


    [Serializable]
    public class NewTaskData
    {
        public string Date { get; set; }
        public TaskMode Mode { get; set; }
        public int TaskModeIndex { get; set; }
        public TaskType TaskType { get; set; }
        public int TaskTypeIndex { get; set; }
        public string Answer { get; set; }
        public bool IsAnswerCorrect { get; set; }
        public double Duration { get; set; }
    }
}

