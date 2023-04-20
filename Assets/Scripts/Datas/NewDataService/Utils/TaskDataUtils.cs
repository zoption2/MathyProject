using Mathy.Data;

namespace Mathy.Services
{
    public static class TaskResultsTableRequests
    {
        private const string kTasksTable = "TasksResults";

        private const string kId = "id";
        private const string kDate = "Date";
        private const string kMode = "Mode";
        private const string kModeIndex = "ModeIndex";
        private const string kTaskType = "TaskType";
        private const string kTaskTypeIndex = "TypeIndex";
        private const string kElements = "Elements";
        private const string kOperators = "Operators";
        private const string kVariants = "Variants";
        private const string kSelectedAnswersIndexes = "SelectedIndexes";
        private const string kCorrectAnswersIndexes = "CorrectIndexes";
        private const string kIsCorrect = "IsCorrect";
        private const string kDuration = "Duration";
        private const string kMaxValue = "MaxValue";


        public static readonly string TryCreateTasksDataTableQuery = $@"create table if not exists {kTasksTable}
            (
            {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
            {kDate} STRING NOT NULL,
            {kMode} STRING NOT NULL,
            {kModeIndex} INTEGER NOT NULL,
            {kTaskType} STRING NOT NULL,
            {kTaskTypeIndex} INTEGER NOT NULL,
            {kElements} STRING NOT NULL,
            {kOperators} STRING NOT NULL,
            {kVariants} STRING NOT NULL,
            {kSelectedAnswersIndexes} STRING NOT NULL,
            {kCorrectAnswersIndexes} STRING NOT NULL,
            {kIsCorrect} BOOLEAN NOT NULL,
            {kDuration} DOUBLE NOT NULL,
            {kMaxValue} INTEGER NOT NULL
            )";

        private static string _selectableTaskTableContent = $@"
            {kId} as {nameof(TaskDataTableModel.ID)},
            {kDate} as {nameof(TaskDataTableModel.Date)},
            {kMode} as {nameof(TaskDataTableModel.Mode)},
            {kModeIndex} as {nameof(TaskDataTableModel.TaskModeIndex)},
            {kTaskType} as {nameof(TaskDataTableModel.TaskType)},
            {kTaskTypeIndex} as {nameof(TaskDataTableModel.TaskTypeIndex)},
            {kElements} as {nameof(TaskDataTableModel.ElementValues)},
            {kOperators} as {nameof(TaskDataTableModel.OperatorValues)},
            {kVariants} as {nameof(TaskDataTableModel.VariantValues)},
            {kSelectedAnswersIndexes} as {nameof(TaskDataTableModel.SelectedAnswerIndexes)},
            {kCorrectAnswersIndexes} as {nameof(TaskDataTableModel.CorrectAnswerIndexes)},
            {kIsCorrect} as {nameof(TaskDataTableModel.IsAnswerCorrect)},
            {kDuration} as {nameof(TaskDataTableModel.Duration)},
            {kMaxValue} as {nameof(TaskDataTableModel.MaxValue)}";


        public static readonly string InsertTaskQuery = $@"insert into {kTasksTable}
            ({kDate}, {kMode}, {kModeIndex}, {kTaskType}, {kTaskTypeIndex}, {kElements}
            , {kOperators}, {kVariants}, {kSelectedAnswersIndexes}, {kCorrectAnswersIndexes}
            , {kIsCorrect}, {kDuration}, {kMaxValue})
            values( 
                @{nameof(TaskDataTableModel.Date)}, 
                @{nameof(TaskDataTableModel.Mode)},
                @{nameof(TaskDataTableModel.TaskModeIndex)},
                @{nameof(TaskDataTableModel.TaskType)},
                @{nameof(TaskDataTableModel.TaskTypeIndex)},
                @{nameof(TaskDataTableModel.ElementValues)},
                @{nameof(TaskDataTableModel.OperatorValues)},
                @{nameof(TaskDataTableModel.VariantValues)},
                @{nameof(TaskDataTableModel.SelectedAnswerIndexes)},
                @{nameof(TaskDataTableModel.CorrectAnswerIndexes)},
                @{nameof(TaskDataTableModel.IsAnswerCorrect)},
                @{nameof(TaskDataTableModel.Duration)},
                @{nameof(TaskDataTableModel.MaxValue)})
            returning {kId}";


        public static readonly string SelectTaskByModeAndDateQuery = $@"select
            {_selectableTaskTableContent}
            from {kTasksTable} 
            where {kMode} = @{nameof(TaskDataTableModel.Mode)}
            and {kDate} = @{nameof(TaskDataTableModel.Date)}
            ;";
    }


    public static class DailyModeTableRequests
    {
        private const string kDailyModeTable = "DailyMode";

        private const string kId = "id";
        private const string kDate = "Date";
        private const string kMode = "Mode";
        private const string kModeIndex = "ModeIndex";
        private const string kIsModeDone = "IsDone";
        private const string kPlayedCount = "PlayedCount";


        public static readonly string TryCreateDailyModeTableQuery = $@"create table if not exists {kDailyModeTable}
            (
            {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
            {kDate} STRING NOT NULL,
            {kMode} STRING NOT NULL,
            {kModeIndex} INTEGER NOT NULL,
            {kIsModeDone} BOOLEAN NOT NULL,
            {kPlayedCount} INTEGER NOT NULL
            )";

        private static string _selectableDailyModeTableContent = $@"
            {kId} as {nameof(DailyModeTableModel.Id)},
            {kDate} as {nameof(DailyModeTableModel.Date)},
            {kMode} as {nameof(DailyModeTableModel.Mode)},
            {kModeIndex} as {nameof(DailyModeTableModel.ModeIndex)},
            {kIsModeDone} as {nameof(DailyModeTableModel.IsComplete)},
            {kPlayedCount} as {nameof(DailyModeTableModel.PlayedTasks)}";


        public static readonly string InsertDailyQuery = $@"insert into {kDailyModeTable}
            ({kDate}, {kMode}, {kModeIndex}, {kIsModeDone}, {kPlayedCount})
            values(
            @{nameof(DailyModeTableModel.Date)},
            @{nameof(DailyModeTableModel.Mode)},
            @{nameof(DailyModeTableModel.ModeIndex)},
            @{nameof(DailyModeTableModel.IsComplete)},
            @{nameof(DailyModeTableModel.PlayedTasks)})";

        public static readonly string UpdateDailyQuery = $@"update {kDailyModeTable}
            SET {kIsModeDone} = @{nameof(DailyModeTableModel.IsComplete)},
                {kPlayedCount} = @{nameof(DailyModeTableModel.PlayedTasks)}
            WHERE {kDate} = @{nameof(DailyModeTableModel.Date)}
            AND {kModeIndex} = @{nameof(DailyModeTableModel.ModeIndex)}";


        public static readonly string SelectDailyQuery = $@"select
            {_selectableDailyModeTableContent}
            from {kDailyModeTable}
            where {kDate} = @{nameof(DailyModeTableModel.Date)}
            and {kModeIndex} = @{nameof(DailyModeTableModel.ModeIndex)}
            ;";
    }


    public static class GeneralResultsTableRequests
    {
        private const string kGeneralTable = "GeneralTasksResults";

        private const string kTotalTasks = "TasksPlayed";
        private const string kTotalCorrect = "CorrectAnswers";
        private const string kTasksTime = "TasksTime";
        private const string kEachTaskPlayed = "EachTaskPlayed";
        private const string kTaskMiddleRating = "TasksMiddleRating";
        private const string kEachModePlayed = "EachModePlayed";
        private const string kModeMiddleRating = "ModeMiddleRating";
        private const string kModeCompleted = "ModeCompleted";
        private const string kSkillMiddleRating = "SkillMiddleRating";



        public static readonly string TryCreateTableQuery = $@"create table if not exists {kGeneralTable}
            (
            {kTotalTasks} INTEGER NOT NULL,
            {kTotalCorrect} INTEGER NOT NULL,
            {kTasksTime} DOUBLE NOT NULL,
            {kEachTaskPlayed} STRING NOT NULL,
            {kTaskMiddleRating} STRING NOT NULL,
            {kEachModePlayed} STRING NOT NULL,
            {kModeMiddleRating} STRING NOT NULL,
            {kModeCompleted} STRING NOT NULL,
            {kSkillMiddleRating} STRING NOT NULL
            )";

        public static readonly string SelectQuery = $@"select
            {kTotalTasks} as {nameof(GeneralResultsTableModel.TotalTasksPlayed)},
            {kTotalCorrect} as {nameof(GeneralResultsTableModel.TotalCorrectAnswers)},
            {kTasksTime} as {nameof(GeneralResultsTableModel.TotalPlayedTime)},
            {kEachTaskPlayed} as {nameof(GeneralResultsTableModel.EachTaskPlayedJson)},
            {kTaskMiddleRating} as {nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
            {kEachModePlayed} as {nameof(GeneralResultsTableModel.EachModePlayedJson)},
            {kModeMiddleRating} as {nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
            {kModeCompleted} as {nameof(GeneralResultsTableModel.ModeCompletedJson)},
            {kSkillMiddleRating} as {nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
            from {kGeneralTable}
            ;";

        public static readonly string InsertQuery = $@"insert into {kGeneralTable}
            ({kTotalTasks}, {kTotalCorrect}, {kTasksTime}, {kEachTaskPlayed}, {kTaskMiddleRating},
            {kEachModePlayed}, {kModeMiddleRating}, {kModeCompleted}, {kSkillMiddleRating})
            values(
                @{nameof(GeneralResultsTableModel.TotalTasksPlayed)},
                @{nameof(GeneralResultsTableModel.TotalCorrectAnswers)},
                @{nameof(GeneralResultsTableModel.TotalPlayedTime)},
                @{nameof(GeneralResultsTableModel.EachTaskPlayedJson)},
                @{nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
                @{nameof(GeneralResultsTableModel.EachModePlayedJson)},
                @{nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
                @{nameof(GeneralResultsTableModel.ModeCompletedJson)},
                @{nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
            )";


        public static readonly string UpdateQuery = $@"update {kGeneralTable}
            set
            {kTotalTasks} = @{nameof(GeneralResultsTableModel.TotalTasksPlayed)},
            {kTotalCorrect} = @{nameof(GeneralResultsTableModel.TotalCorrectAnswers)},
            {kTasksTime} = @{nameof(GeneralResultsTableModel.TotalPlayedTime)},
            {kEachTaskPlayed} = @{nameof(GeneralResultsTableModel.EachTaskPlayedJson)},
            {kTaskMiddleRating} = @{nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
            {kEachModePlayed} = @{nameof(GeneralResultsTableModel.EachModePlayedJson)},
            {kModeMiddleRating} = @{nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
            {kModeCompleted} = @{nameof(GeneralResultsTableModel.ModeCompletedJson)},
            {kSkillMiddleRating} = @{nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
";
    }

}

