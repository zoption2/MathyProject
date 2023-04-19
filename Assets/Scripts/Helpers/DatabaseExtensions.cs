using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mathy.Services;
using System;
using System.Globalization;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V20;

public static class DatabaseExtensions
{
    private const string kDataFormat = "yyyy-MM-dd";

    public static TaskDataTableModel ToTaskTableData(this TaskData data)
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

    //result.ElementValues = data.ElementValues != null 
    //        ? string.Join(",", data.ElementValues)
    //        : string.Empty;
    //    result.OperatorValues = data.OperatorValues != null
    //        ? string.Join(",", data.OperatorValues)
    //        : string.Empty;
    //    result.VariantValues = data.VariantValues != null
    //        ? string.Join(",", data.VariantValues)
    //        : string.Empty;

    public static TaskData ToTaskData(this TaskDataTableModel model)
    {
        var result = new TaskData();
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


    public static DailyModeTableModel ToDailyModeTableData(this DailyModeData data)
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

    public static DailyModeData ToDailyModeData(this DailyModeTableModel model)
    {
        var result = new DailyModeData();
        result.Id = model.Id;
        result.Date = DateTime.ParseExact(model.Date, kDataFormat, CultureInfo.InvariantCulture);
        result.Mode = Enum.Parse<TaskMode>(model.Mode);
        result.IsComplete = model.IsComplete;
        result.PlayedCount = model.PlayedTasks;

        return result;
    }
}
