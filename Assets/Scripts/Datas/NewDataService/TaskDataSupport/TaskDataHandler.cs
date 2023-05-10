using Cysharp.Threading.Tasks;
using Mathy.Data;
using System;
using System.Collections.Generic;



namespace Mathy.Services.Data
{
    public interface ITaskDataHandler
    {
        UniTask<List<TaskResultData>> GetResultsByModeAndDate(TaskMode mode, DateTime date);
        UniTask<List<string>> GetTaskResultsFormatted(TaskMode mode, DateTime date);
        UniTask<int> SaveTask(TaskResultData task);
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
        UniTask<List<DailyModeData>> GetMonthData(DateTime date);
        UniTask<SkillStatisticData> GetSkillStatistic(SkillType skillType, int grade);
        UniTask<DayResultData> GetDayResult(DateTime date);
    }


    public class TaskDataHandler : ITaskDataHandler
    {
        private const int kModesToDayComplete = 4;

        private readonly ITaskResultsProvider _taskProvider;
        private readonly IDailyModeProvider _dailyModeProvider;
        private readonly ISkillStatisticProvider _skillStatisticProvider;
        private readonly IDayResultProvider _dayResultsProvider;
        private readonly ITaskResultFormatProcessor _resultFormatProcessor;
        private readonly DataService _dataService;


        public TaskDataHandler(DataService dataService)
        {
            _dataService = dataService;
            var filePath = dataService.DatabasePath;

            _taskProvider = new TaskResultsProvider(filePath);
            _dailyModeProvider = new DailyModeProvider(filePath);
            _skillStatisticProvider = new SkillStatisticProvider(filePath);
            _dayResultsProvider = new DayResultProvider(filePath);
            _resultFormatProcessor = new TaskResultFormatProcessor();
        }


        public async UniTask<List<TaskResultData>> GetResultsByModeAndDate(TaskMode mode, DateTime date)
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

        public async UniTask<int> SaveTask(TaskResultData task)
        {
            var idKey = KeyValueIntegerKeys.TotalTasksIndexer.ToString();
            var uniqueId = await _dataService.KeyValueStorage.GetIntValue(idKey, 0);
            uniqueId++;
            task.ID = uniqueId;
            await _taskProvider.SaveTask(task);
            await _dataService.KeyValueStorage.SaveIntValue(idKey, uniqueId);

            var answer = task.IsAnswerCorrect
                ? KeyValueIntegerKeys.TotalCorrectAnswers
                : KeyValueIntegerKeys.TotalWrongAnswers;
            var answerKey = answer.ToString();
            await _dataService.KeyValueStorage.IncrementIntValue(answerKey);

            var skillKey = task.SkillType.ToString();
            await _dataService.KeyValueStorage.IncrementIntValue(skillKey);

            await UpdateSkillStatistic(task);

            return uniqueId;
        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            await _dailyModeProvider.UpdateDailyMode(data);
            await UpdateDayResultData(data);
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {
            var result = await _dailyModeProvider.GetDailyModeData(date, mode);
            return result;
        }

        public async UniTask<List<DailyModeData>> GetMonthData(DateTime date)
        {
            return await _dailyModeProvider.GetMonthData(date);
        }

        public async UniTask<SkillStatisticData> GetSkillStatistic(SkillType skillType, int grade)
        {
            var result = await _skillStatisticProvider.GetSkillStatistic(skillType, grade);
            return result;
        }

        public async UniTask<DayResultData> GetDayResult(DateTime date)
        {
            return await _dayResultsProvider.GetDayResult(date);
        }

        public async UniTask Init()
        {
            await TryCreateTables();
        }

        public async UniTask ClearData()
        {
            await _taskProvider.DeleteTable();
            await _dailyModeProvider.DeleteTable();
            await _skillStatisticProvider.DeleteTable();
            await _dayResultsProvider.DeleteTable();
        }

        protected async UniTask TryCreateTables()
        {
            await _taskProvider.TryCreateTable();
            await _dailyModeProvider.TryCreateTable();
            await _skillStatisticProvider.TryCreateTable();
            await _dayResultsProvider.TryCreateTable();
        }

        private async UniTask UpdateSkillStatistic(TaskResultData task)
        {
            var stat = await _skillStatisticProvider.GetSkillStatistic(task.SkillType, task.Grade);
            var updatedTotal = ++stat.Total;
            var updatedCorrect = task.IsAnswerCorrect ? ++stat.Correct : stat.Correct;

            stat.Total = updatedTotal;
            stat.Correct = updatedCorrect;
            stat.Rate = updatedTotal > 0
                ? ((updatedCorrect * 100) / updatedTotal)
                : 0;
            stat.Duration += task.Duration;
            await _skillStatisticProvider.UpdateSkillStatistic(stat);
        }

        private async UniTask UpdateDayResultData(DailyModeData data)
        {
            var dayResult = await _dayResultsProvider.GetDayResult(data.Date);
            dayResult.Date = data.Date;
            if (data.IsComplete && !dayResult.CompletedModes.Contains(data.Mode))
            {
                dayResult.CompletedModes.Add(data.Mode);
                dayResult.TotalTasks += data.TotalTasks;
                dayResult.CorrectTasks += data.CorrectAnswers;
                dayResult.MiddleRate = (dayResult.CorrectTasks * 100) / dayResult.TotalTasks;
                dayResult.Duration += data.Duration;
            }

            if(dayResult.CompletedModes.Count == kModesToDayComplete)
            {
                dayResult.IsCompleted = true;
            }

            await _dayResultsProvider.UpdateDayResult(dayResult);
        }
    }
}

