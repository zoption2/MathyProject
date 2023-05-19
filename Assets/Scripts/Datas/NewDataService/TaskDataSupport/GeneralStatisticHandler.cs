using Cysharp.Threading.Tasks;
using Mathy.Data;
using System;

namespace Mathy.Services.Data
{
    public interface IGeneralStatisticHandler
    {
        UniTask<GeneralTasksViewData> GetGeneralTasksDataAsync();
        UniTask<DetailedTasksViewData> GetDetailedTasksDataAsync(TaskType taskType);
        UniTask<DailyModeViewData> GetDailyModeDataAsync(TaskMode mode);
    }


    public class GeneralStatisticHandler : IGeneralStatisticHandler
    {
        private readonly IDetailedTaskStatisticProvider _detailedTaskStatisticProvider;
        private readonly IDailyModeStatisticProvider _dailyModeStatisticProvider;
        private readonly DataService _dataService;
        private readonly string _filePath;

        public GeneralStatisticHandler(DataService dataService)
        {
            _dataService = dataService;
            _filePath = dataService.DatabasePath;

            _detailedTaskStatisticProvider = new DetailedTaskStatisticProvider(_filePath);
            _dailyModeStatisticProvider = new DailyModeStatisticProvider(_filePath);
        }

        public async UniTask<GeneralTasksViewData> GetGeneralTasksDataAsync()
        {
            var data = new GeneralTasksViewData();
            var totalTasks = await _dataService.KeyValueStorage.GetIntValueAsync(KeyValueIntegerKeys.TotalTasksIndexer);
            var totalCorrect = await _dataService.KeyValueStorage.GetIntValueAsync(KeyValueIntegerKeys.TotalCorrectAnswers);
            data.TotalTasksPlayed = totalTasks;
            data.TotalCorrectAnswers = totalCorrect;
            data.MiddleRate = (int)((totalCorrect * 100) / totalTasks);
            data.TotalPlayedTime = 0;

            return data;
        }

        public async UniTask<DetailedTasksViewData> GetDetailedTasksDataAsync(TaskType taskType)
        {
            return await _detailedTaskStatisticProvider.GetDataAsync(taskType);
        }

        public async UniTask<DailyModeViewData> GetDailyModeDataAsync(TaskMode mode)
        {
            return await _dailyModeStatisticProvider.GetDailyModeDataAsync(mode);
        }

        public async UniTask Init()
        {
            await TryCreateTables();
        }

        public async UniTask ClearData()
        {
            await _detailedTaskStatisticProvider.DeleteTable();
            await _dailyModeStatisticProvider.DeleteTable();
        }

        protected async UniTask TryCreateTables()
        {
            await _detailedTaskStatisticProvider.TryCreateTable();
            await _dailyModeStatisticProvider.TryCreateTable();
        }
    }
}

