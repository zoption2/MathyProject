﻿using System;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using Mathy.Data;
using Cysharp.Threading.Tasks;
using ModestTree;


namespace Mathy.Services
{
    public interface ITaskDataHandler
    {
        UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date);
        UniTask SaveTask(TaskResultData task);
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
    }


    public class TaskDataHandler : ITaskDataHandler
    {
        private readonly TaskResultsProvider _taskProvider;
        private readonly GeneralResultsProvider _generalProvider;
        private readonly DailyModeProvider _dailyModeProvider;

        private const string kFileFormat = "tasks_results_save_{0}.db";
        private const string kGeneralFileName = "general_results_save.db";

        private string _taskDBFilePath;
        private string _generalDBFilePath;
        private string _saveDirectoryPath;
        private int _currentYear;
        private GeneralResultsData _generalData;
        private static IDbConnection _taskDBConnection;
        private static IDbConnection _generalDBConnection;

        public TaskDataHandler(string directoryPath)
        {
            _saveDirectoryPath = directoryPath;
            _currentYear = DateTime.UtcNow.Year;
            var fileName = string.Format(kFileFormat, _currentYear);
            var saveFilePath = directoryPath + fileName;
            _taskDBFilePath = $"Data Source={saveFilePath}";

            var generalSavePath = directoryPath + kGeneralFileName;
            _generalDBFilePath = $"Data Source={generalSavePath}";

            _taskProvider = new TaskResultsProvider();
            _dailyModeProvider = new DailyModeProvider();
            _generalProvider = new GeneralResultsProvider();
        }

        public async UniTask Init()
        {
            await TryCreateTables();
            await InitProviders();
        }

        private IDbConnection OpenConnection(string path)
        {
            var connection = new SqliteConnection(path);
            connection.Open();
            return connection;
        }

        private void CloseConnection(IDbConnection connection)
        {
            connection.Close();
            connection.Dispose();
        }

        public async UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date)
        {
            var databasePath = GetPathToTaskResultsDatabase(date);
            if (databasePath.IsEmpty())
            {
                return new TaskResultData[0];
            }
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            var result = await _taskProvider.GetTasksByModeAndDate(mode, date, _taskDBConnection);
            CloseConnection(_taskDBConnection);
            return result;
        }

        public async UniTask SaveTask(TaskResultData task)
        {
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            await _taskProvider.SaveTask(task, _taskDBConnection);
            CloseConnection(_taskDBConnection);
            UpdateGeneralData(task);
            SaveGeneralDataAsync();
        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            await _dailyModeProvider.UpdateDailyMode(data, _taskDBConnection);
            CloseConnection(_taskDBConnection);
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {
            var databasePath = GetPathToTaskResultsDatabase(date);
            if (databasePath.IsEmpty())
            {
                return new DailyModeData() { Date = date, Mode = mode };
            }

            _taskDBConnection = OpenConnection(databasePath);
            var result = await _dailyModeProvider.GetDailyModeData(date, mode, _taskDBConnection);
            CloseConnection(_taskDBConnection);
            return result;
        }

        private async UniTask TryCreateTables()
        {
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            await _taskProvider.TryCreateTable(_taskDBConnection);
            await _dailyModeProvider.TryCreateTable(_taskDBConnection);
            CloseConnection(_taskDBConnection);
            _generalDBConnection = OpenConnection(_generalDBFilePath);
            await _generalProvider.TryCreateTable(_generalDBConnection);
            CloseConnection (_generalDBConnection);
        }

        private async UniTask InitProviders()
        {
            _generalDBConnection = OpenConnection(_generalDBFilePath);
            _generalData = await _generalProvider.GetDataAsync(_generalDBConnection);
            CloseConnection(_generalDBConnection);
        }

        private void UpdateGeneralData(TaskResultData task)
        {
            _generalData.TotalTasksPlayed++;
            _generalData.TotalCorrectAnswers = task.IsAnswerCorrect ? ++_generalData.TotalCorrectAnswers : _generalData.TotalCorrectAnswers;
            _generalData.TotalPlayedTime += task.Duration;
            var taskDictionary = _generalData.EachTaskPlayed;
            if (!taskDictionary.ContainsKey(task.TaskType))
            {
                taskDictionary.Add(task.TaskType, 0);
            }
            _generalData.EachTaskPlayed[task.TaskType]++;

            var modeDictionary = _generalData.EachModePlayed;
            if (!modeDictionary.ContainsKey(task.Mode))
            {
                modeDictionary.Add(task.Mode, 0);
            }
            _generalData.EachModePlayed[task.Mode]++;
        }

        private async void SaveGeneralDataAsync()
        {
            _generalDBConnection = OpenConnection(_generalDBFilePath);
            await _generalProvider.SaveAsync(_generalData, _generalDBConnection);
            CloseConnection(_generalDBConnection);
        }

        //Method return path to database file based on date.Year, as every year has it own file.db
        private string GetPathToTaskResultsDatabase(DateTime date)
        {
            if (date.Year == _currentYear)
            {
                return _taskDBFilePath;
            }
            else
            {
                var fileName = string.Format(kFileFormat, date.Year);
                var saveFilePath = _saveDirectoryPath + fileName;
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

