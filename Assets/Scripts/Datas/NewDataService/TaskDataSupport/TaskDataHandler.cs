using Cysharp.Threading.Tasks;
using Mathy.Data;
using ModestTree;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;


namespace Mathy.Services.Data
{
    public interface ITaskDataHandler
    {
        IReadonlyGeneralResultsData GeneralData { get; }
        UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date);
        UniTask<List<string>> GetTaskResultsFormatted(TaskMode mode, DateTime date);
        UniTask SaveTask(TaskResultData task);
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
    }


    public class TaskDataHandler : ITaskDataHandler
    {
        private readonly ITaskResultsProvider _taskProvider;
        private readonly IGeneralResultsProvider _generalProvider;
        private readonly IDailyModeProvider _dailyModeProvider;
        private readonly ITaskResultFormatProcessor _resultFormatProcessor;

        private string _filePath;
        private GeneralResultsData _generalData;

        public IReadonlyGeneralResultsData GeneralData => _generalData;

        public TaskDataHandler(string filePath)
        {
            _filePath = filePath;

            _taskProvider = new TaskResultsProvider(filePath);
            _dailyModeProvider = new DailyModeProvider(filePath);
            _generalProvider = new GeneralResultsProvider(filePath);
            _resultFormatProcessor = new TaskResultFormatProcessor();
        }


        public async UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date)
        {
            var result = await _taskProvider.GetTasksByModeAndDate(mode, date);
            return result;
        }

        public async UniTask<List<string>> GetTaskResultsFormatted(TaskMode mode, DateTime date)
        {
            var tasks = await GetTasksByModeAndDate(mode, date);
            var result = _resultFormatProcessor.GetTaskResultsFormatted(tasks);
            return result;
        }

        public async UniTask SaveTask(TaskResultData task)
        {
            await _taskProvider.SaveTask(task);
            UpdateGeneralData(task);
            SaveGeneralDataAsync();
        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            await _dailyModeProvider.UpdateDailyMode(data);
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {
            var result = await _dailyModeProvider.GetDailyModeData(date, mode);
            return result;
        }

        protected async UniTask TryCreateTables()
        {
            await _taskProvider.TryCreateTable();
            await _dailyModeProvider.TryCreateTable();
            await _generalProvider.TryCreateTable();
        }

        public async UniTask Init()
        {
            await TryCreateTables();
            _generalData = await _generalProvider.GetDataAsync();
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
            await _generalProvider.SaveAsync(_generalData);
        }
    }
}

