using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mathy.Services;
using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class DatabaseExtensions
{
    private const string kDataFormat = "yyyy-MM-dd";

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
        result.SelectedAnswerIndexes= model.SelectedAnswerIndexes.Split(',').Select(int.Parse).ToList();
        result.CorrectAnswerIndexes= model.CorrectAnswerIndexes.Split(',').Select(int.Parse).ToList();
        result.IsAnswerCorrect= model.IsAnswerCorrect;
        result.Duration = model.Duration;
        result.MaxValue = model.MaxValue;

        return result;
    }


    public static DailyModeTableModel ConvertToModel(this DailyModeData data)
    {
        var result = new DailyModeTableModel();
        result.Id = data.Id;
        result.Date = data.Date.ToString(kDataFormat);
        result.Mode = data.Mode.ToString();
        result.ModeIndex = (int)data.Mode;
        result.IsComplete = data.IsComplete;
        result.PlayedTasks = data.PlayedCount;

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

        return result;
    }

    public static GeneralResultsTableModel ConvertToModel(this GeneralResultsData data)
    {
        var result = new GeneralResultsTableModel();
        result.TotalTasksPlayed = data.TotalTasksPlayed;
        result.TotalCorrectAnswers = data.TotalCorrectAnswers;
        result.TotalPlayedTime = data.TotalPlayedTime;
        result.EachTaskPlayedJson = JsonConvert.SerializeObject(data.EachTaskPlayed);
        result.TaskMiddleRatingJson = JsonConvert.SerializeObject(data.TaskMiddleRating);
        result.EachModePlayedJson = JsonConvert.SerializeObject(data.EachModePlayed);
        result.ModeMiddleRatingJson = JsonConvert.SerializeObject(data.ModeMiddleRating);
        result.ModeCompletedJson = JsonConvert.SerializeObject(data.ModeCompleted);
        result.SkillTypeMiddleRatingJson = JsonConvert.SerializeObject(data.SkillTypeMiddleRating);

        return result;
    }

    public static GeneralResultsData ConvertToData(this GeneralResultsTableModel model)
    {
        var result = new GeneralResultsData();
        result.TotalTasksPlayed = model.TotalTasksPlayed;
        result.TotalCorrectAnswers = model.TotalCorrectAnswers;
        result.TotalPlayedTime = model.TotalPlayedTime;
        result.EachTaskPlayed = JsonConvert.DeserializeObject<Dictionary<TaskType, int>>(model.EachTaskPlayedJson);
        result.TaskMiddleRating = JsonConvert.DeserializeObject<Dictionary<TaskType, int>>(model.TaskMiddleRatingJson);
        result.EachModePlayed = JsonConvert.DeserializeObject<Dictionary<TaskMode, int>>(model.EachModePlayedJson);
        result.ModeMiddleRating = JsonConvert.DeserializeObject<Dictionary<TaskMode, int>>(model.ModeMiddleRatingJson);
        result.ModeCompleted = JsonConvert.DeserializeObject<Dictionary<TaskMode, int>>(model.ModeCompletedJson);
        result.SkillTypeMiddleRating = JsonConvert.DeserializeObject<Dictionary<SkillType, int>>(model.SkillTypeMiddleRatingJson);

        return result;
    }
}
