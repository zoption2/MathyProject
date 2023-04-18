using Mathy.Data;
using Mathy.Services;

public static class DatabaseExtensions
{
    public static TaskDataTableModel ToTaskTableData(this TaskData data)
    {
        var result = new TaskDataTableModel();
        result.ID = data.ID;
        result.Date = data.Date.ToString("yyyy-MM-dd");
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


    public static DailyModeTableModel ToDailyModeTableData(this DailyModeData data)
    {
        var result = new DailyModeTableModel();
        result.Id = data.Id;
        result.Date = data.Date.ToString("yyyy-MM-dd");
        result.Mode = data.Mode.ToString();
        result.ModeIndex = (int)data.Mode;
        result.IsComplete = data.IsComplete;
        result.LastIndex = data.LastIndex;

        return result;
    }
}
