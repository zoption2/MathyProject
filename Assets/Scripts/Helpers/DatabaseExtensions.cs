using Cysharp.Threading.Tasks;
using Mathy.Data;
using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;

namespace Mathy.Services.Data
{
    public static class DatabaseExtensions
    {
        private const string kDataFormat = "yyyy-MM-dd";

        #region TaskResultsTable
        public static TaskDataTableModel ConvertToModel(this TaskResultData data)
        {
            var result = new TaskDataTableModel();
            result.ID = data.ID;
            result.Date = data.Date.ToString(kDataFormat);
            result.Mode = data.Mode.ToString();
            result.TaskModeIndex = (int)data.Mode;
            result.TaskType = data.TaskType.ToString();
            result.TaskTypeIndex = (int)data.TaskType;
            result.ElementValues = string.Join(",", data.ElementValues);
            result.OperatorValues = string.Join(",", data.OperatorValues);
            result.VariantValues = string.Join(",", data.VariantValues);
            result.SelectedAnswerIndexes = string.Join(",", data.SelectedAnswerIndexes);
            result.CorrectAnswerIndexes = string.Join(",", data.CorrectAnswerIndexes);
            result.IsAnswerCorrect = data.IsAnswerCorrect;
            result.Duration = data.Duration;
            result.MaxValue = data.MaxValue;

            return result;
        }

        public static TaskResultData ConvertToData(this TaskDataTableModel model)
        {
            var result = new TaskResultData();
            result.ID = model.ID;
            result.Date = DateTime.ParseExact(model.Date, kDataFormat, CultureInfo.InvariantCulture);
            result.Mode = Enum.Parse<TaskMode>(model.Mode);
            result.TaskType = Enum.Parse<TaskType>(model.TaskType);
            result.ElementValues = model.ElementValues.Split(',').ToList();
            result.OperatorValues = model.OperatorValues.Split(',').ToList();
            result.VariantValues = model.VariantValues.Split(',').ToList();
            result.SelectedAnswerIndexes = model.SelectedAnswerIndexes.Split(',').Select(int.Parse).ToList();
            result.CorrectAnswerIndexes = model.CorrectAnswerIndexes.Split(',').Select(int.Parse).ToList();
            result.IsAnswerCorrect = model.IsAnswerCorrect;
            result.Duration = model.Duration;
            result.MaxValue = model.MaxValue;

            return result;
        }
        #endregion

        #region DailyModeTable
        public static DailyModeTableModel ConvertToModel(this DailyModeData data)
        {
            var result = new DailyModeTableModel();
            result.Id = data.Id;
            result.Date = data.Date.ToString(kDataFormat);
            result.Mode = data.Mode.ToString();
            result.ModeIndex = (int)data.Mode;
            result.IsComplete = data.IsComplete;
            result.PlayedTasks = data.PlayedCount;
            result.CorrectAnswers = data.CorrectAnswers;
            result.CorrectRate = data.CorrectRate;
            result.Duration = data.Duration;
            result.TotalTasks = data.TotalTasks;

            return result;
        }

        public static DailyModeData ConvertToData(this DailyModeTableModel model)
        {
            var result = new DailyModeData();
            result.Id = model.Id;
            result.Date = DateTime.ParseExact(model.Date, kDataFormat, CultureInfo.InvariantCulture);
            result.Mode = Enum.Parse<TaskMode>(model.Mode);
            result.IsComplete = model.IsComplete;
            result.PlayedCount = model.PlayedTasks;
            result.CorrectAnswers = model.CorrectAnswers;
            result.CorrectRate = model.CorrectRate;
            result.Duration = model.Duration;
            result.TotalTasks = model.TotalTasks;

            return result;
        }
        #endregion

        #region GeneralResultsTable
        public static GeneralTasksViewModel ConvertToModel(this GeneralTasksViewData data)
        {
            var result = new GeneralTasksViewModel();
            result.TotalTasksPlayed = data.TotalTasksPlayed;
            result.TotalCorrectAnswers = data.TotalCorrectAnswers;
            result.MiddleRate = data.MiddleRate;
            result.TotalPlayedTime = data.TotalPlayedTime;

            return result;
        }

        public static GeneralTasksViewData ConvertToData(this GeneralTasksViewModel model)
        {
            var result = new GeneralTasksViewData();
            result.TotalTasksPlayed = model.TotalTasksPlayed;
            result.MiddleRate = model.MiddleRate;
            result.TotalCorrectAnswers = model.TotalCorrectAnswers;
            result.TotalPlayedTime = model.TotalPlayedTime;

            return result;
        }

        public static DetailedTasksViewModel ConvertToModel(this DetailedTasksViewData data)
        {
            var result = new DetailedTasksViewModel();
            result.TaskType = data.TaskType.ToString();
            result.TaskTypeIndex = (int)data.TaskType;
            result.TotalTasksPlayed = data.TotalTasksPlayed;
            result.TotalCorrectAnswers = data.TotalCorrectAnswers;
            result.MiddleRate = data.MiddleRate;
            result.TotalPlayedTime = data.TotalPlayedTime;

            return result;
        }

        public static DetailedTasksViewData ConvertToData(this DetailedTasksViewModel model)
        {
            var result = new DetailedTasksViewData();
            result.TaskType = Enum.Parse<TaskType>(model.TaskType);
            result.TotalTasksPlayed = model.TotalTasksPlayed;
            result.TotalCorrectAnswers = model.TotalCorrectAnswers;
            result.MiddleRate = model.MiddleRate;
            result.TotalPlayedTime = model.TotalPlayedTime;

            return result;
        }

        public static DailyModeViewModel ConvertToModel(this DailyModeViewData data)
        {
            var result = new DailyModeViewModel();
            result.Mode = data.Mode.ToString();
            result.ModeIndex = (int)data.Mode;
            result.TotalCompleted = data.TotalCompleted;

            return result;
        }

        public static DailyModeViewData ConvertToData(this DailyModeViewModel model)
        {
            var result = new DailyModeViewData();
            result.Mode = Enum.Parse<TaskMode>(model.Mode);
            result.TotalCompleted = model.TotalCompleted;

            return result;
        }
        #endregion

        #region SkillPlanTable
        public static SkillPlanTableModel ConvertToTableModel(this SkillSettingsData data)
        {
            var result = new SkillPlanTableModel();
            result.Grade = data.Grade;
            result.Skill = data.Skill.ToString();
            result.IsEnabled = data.IsEnabled;
            result.Value = data.Value;
            result.MinValue = data.MinValue;
            result.MaxValue = data.MaxValue;

            return result;
        }

        public static SkillSettingsData ConvertToData(this SkillPlanTableModel model)
        {
            var result = new SkillSettingsData();
            result.Grade = model.Grade;
            result.Skill = Enum.Parse<SkillType>(model.Skill);
            result.IsEnabled = model.IsEnabled;
            result.Value = model.Value;
            result.MinValue = model.MinValue;
            result.MaxValue = model.MaxValue;

            return result;
        }
        #endregion



        private static string SerializeDictionary<TEnum, TValue>(Dictionary<TEnum, TValue> dic) where TEnum : Enum
        {
            int count = dic.Count;
            Dictionary<string, TValue> convertedDic = new(count);
            foreach (var key in dic.Keys)
            {
                convertedDic.Add(key.ToString(), dic[key]);
            }
            var result = JsonConvert.SerializeObject(convertedDic);
            return result;
        }

        private static Dictionary<TEnum, TValue> DeserializeDictionaryJson<TEnum, TValue>(string json) where TEnum : Enum
        {
            Dictionary<string, TValue> convertedDic = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            Dictionary<TEnum, TValue> result = new();

            foreach (var key in convertedDic.Keys)
            {
                try
                {
                    var parsedKey = Enum.Parse<TEnum>(key);
                    result.Add(parsedKey, convertedDic[key]);
                }
                catch (Exception)
                {
                    UnityEngine.Debug.LogFormat($"Current version of {nameof(TEnum)} does not contains {key}, " +
                        $"so it will not be included at general results, getted from database");
                    throw;
                }
            }

            return result;
        }
    }
}

