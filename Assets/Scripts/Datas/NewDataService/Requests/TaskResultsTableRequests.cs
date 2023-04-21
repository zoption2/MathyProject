namespace Mathy.Services.Data
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
}

