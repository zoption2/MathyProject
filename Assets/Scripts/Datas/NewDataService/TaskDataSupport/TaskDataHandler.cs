using Cysharp.Threading.Tasks;
using Mathy.Data;
using System;
using System.Collections.Generic;



namespace Mathy.Services.Data
{
    public interface ITaskDataHandler
    {
        UniTask<TaskResultData[]> GetResultsByModeAndDate(TaskMode mode, DateTime date);
        UniTask<List<string>> GetTaskResultsFormatted(TaskMode mode, DateTime date);
        UniTask SaveTask(TaskResultData task);
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
        UniTask<List<DailyModeData>> GetDailyData(DateTime date);
        UniTask<GeneralTasksViewData> GetGeneralTaskData();
        UniTask<DetailedTasksViewData> GetGeneralTaskTypeDataAsync(TaskType taskType);
        UniTask<DailyModeViewData> GetGeneralTaskModeDataAsync(TaskMode mode);
    }


    public class TaskDataHandler : ITaskDataHandler
    {
        private readonly ITaskResultsProvider _taskProvider;
        private readonly IGeneralResultsProvider _generalProvider;
        private readonly IDailyModeProvider _dailyModeProvider;
        private readonly ITaskResultFormatProcessor _resultFormatProcessor;

        private string _filePath;

        public TaskDataHandler(string filePath)
        {
            _filePath = filePath;

            _taskProvider = new TaskResultsProvider(filePath);
            _dailyModeProvider = new DailyModeProvider(filePath);
            _generalProvider = new GeneralResultsProvider(filePath);
            _resultFormatProcessor = new TaskResultFormatProcessor();
        }


        public async UniTask<TaskResultData[]> GetResultsByModeAndDate(TaskMode mode, DateTime date)
        {
            var result = await _taskProvider.GetTasksByModeAndDate(mode, date);
            return result;
        }

        public async UniTask<List<string>> GetTaskResultsFormatted(TaskMode mode, DateTime date)
        {
            var tasks = await GetResultsByModeAndDate(mode, date);
            var result = _resultFormatProcessor.GetTaskResultsFormatted(tasks);
            return result;
        }

        public async UniTask SaveTask(TaskResultData task)
        {
            await _taskProvider.SaveTask(task);
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

        public async UniTask<List<DailyModeData>> GetDailyData(DateTime date)
        {
            var result = await _dailyModeProvider.GetDailyData(date);
            return result;
        }

        public async UniTask<GeneralTasksViewData> GetGeneralTaskData()
        {
            var result = await _generalProvider.GetGeneralTasksDataAsync();
            return result;
        }

        public async UniTask<DetailedTasksViewData> GetGeneralTaskTypeDataAsync(TaskType taskType)
        {
            var result = await _generalProvider.GetDetailedTasksDataAsync(taskType);
            return result;
        }

        public async UniTask<DailyModeViewData> GetGeneralTaskModeDataAsync(TaskMode mode)
        {
            var result = await _generalProvider.GetDailyModeDataAsync(mode);
            return result;
        }

        public async UniTask Init()
        {
            await TryCreateTables();
        }

        public async UniTask ClearData()
        {
            await _generalProvider.DeleteTable();
            await _taskProvider.DeleteTable();
            await _dailyModeProvider.DeleteTable();
        }

        protected async UniTask TryCreateTables()
        {
            await _taskProvider.TryCreateTable();
            await _dailyModeProvider.TryCreateTable();
            await _generalProvider.TryCreateTable();
        }
    }
}

