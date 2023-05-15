namespace Mathy.Services.Data
{
    public static class TaskResultsTableRequests
    {
        public const string kTasksTable = "TasksResults";

        public const string kId = "id";
        private const string kDate = "Date";
        private const string kMode = "Mode";
        private const string kModeIndex = "ModeIndex";
        public const string kTaskType = "TaskType";
        public const string kTaskTypeIndex = "TypeIndex";
        private const string kSkillType = "Skill";
        private const string kSkillIndex = "SkillIndex";
        private const string kElements = "Elements";
        private const string kOperators = "Operators";
        private const string kVariants = "Variants";
        private const string kSelectedAnswersIndexes = "SelectedIndexes";
        private const string kCorrectAnswersIndexes = "CorrectIndexes";
        public const string kIsCorrect = "IsCorrect";
        public const string kDuration = "Duration";
        private const string kMaxValue = "MaxValue";


        public static readonly string CreatingTableColumns = $@"
        (
            {kId} INTEGER PRIMARY KEY NOT NULL UNIQUE,
            {kDate} STRING NOT NULL,
            {kMode} STRING NOT NULL,
            {kModeIndex} INTEGER NOT NULL,
            {kTaskType} STRING NOT NULL,
            {kTaskTypeIndex} INTEGER NOT NULL,
            {kSkillType} STRING NOT NULL,
            {kSkillIndex} INTEGER NOT NULL,
            {kElements} STRING NOT NULL,
            {kOperators} STRING NOT NULL,
            {kVariants} STRING NOT NULL,
            {kSelectedAnswersIndexes} STRING NOT NULL,
            {kCorrectAnswersIndexes} STRING NOT NULL,
            {kIsCorrect} BOOLEAN NOT NULL,
            {kDuration} DOUBLE NOT NULL,
            {kMaxValue} INTEGER NOT NULL
        )
        ";


        public static readonly string TryCreateTasksDataTableQuery = $@"create table if not exists {kTasksTable}
            {CreatingTableColumns}
            ";




        public static string SelectableContentQuery = $@"
            {kId} as {nameof(TaskDataTableModel.ID)},
            {kDate} as {nameof(TaskDataTableModel.Date)},
            {kMode} as {nameof(TaskDataTableModel.Mode)},
            {kModeIndex} as {nameof(TaskDataTableModel.TaskModeIndex)},
            {kTaskType} as {nameof(TaskDataTableModel.TaskType)},
            {kTaskTypeIndex} as {nameof(TaskDataTableModel.TaskTypeIndex)},
            {kSkillType} as {nameof(TaskDataTableModel.SkillType)},
            {kSkillIndex} as {nameof(TaskDataTableModel.SkillIndex)},
            {kElements} as {nameof(TaskDataTableModel.ElementValues)},
            {kOperators} as {nameof(TaskDataTableModel.OperatorValues)},
            {kVariants} as {nameof(TaskDataTableModel.VariantValues)},
            {kSelectedAnswersIndexes} as {nameof(TaskDataTableModel.SelectedAnswerIndexes)},
            {kCorrectAnswersIndexes} as {nameof(TaskDataTableModel.CorrectAnswerIndexes)},
            {kIsCorrect} as {nameof(TaskDataTableModel.IsAnswerCorrect)},
            {kDuration} as {nameof(TaskDataTableModel.Duration)},
            {kMaxValue} as {nameof(TaskDataTableModel.MaxValue)}";


        public static readonly string InsertableContentQuery = $@"
            ({kId}, {kDate}, {kMode}, {kModeIndex}, {kTaskType}, {kTaskTypeIndex}, {kSkillType}
            , {kSkillIndex}, {kElements}, {kOperators}, {kVariants}, {kSelectedAnswersIndexes}
            , {kCorrectAnswersIndexes}, {kIsCorrect}, {kDuration}, {kMaxValue})
            values( 
                @{nameof(TaskDataTableModel.ID)}, 
                @{nameof(TaskDataTableModel.Date)}, 
                @{nameof(TaskDataTableModel.Mode)},
                @{nameof(TaskDataTableModel.TaskModeIndex)},
                @{nameof(TaskDataTableModel.TaskType)},
                @{nameof(TaskDataTableModel.TaskTypeIndex)},
                @{nameof(TaskDataTableModel.SkillType)},
                @{nameof(TaskDataTableModel.SkillIndex)},
                @{nameof(TaskDataTableModel.ElementValues)},
                @{nameof(TaskDataTableModel.OperatorValues)},
                @{nameof(TaskDataTableModel.VariantValues)},
                @{nameof(TaskDataTableModel.SelectedAnswerIndexes)},
                @{nameof(TaskDataTableModel.CorrectAnswerIndexes)},
                @{nameof(TaskDataTableModel.IsAnswerCorrect)},
                @{nameof(TaskDataTableModel.Duration)},
                @{nameof(TaskDataTableModel.MaxValue)}
            )";


        public static readonly string InsertTaskQuery = $@"insert into {kTasksTable}
            {InsertableContentQuery}
            ";


        //public static readonly string SelectTaskByModeAndDateQuery = $@"select
        //    {SelectableContentQuery}
        //    from {kTasksTable} 
        //    where {kMode} = @{nameof(TaskDataTableModel.Mode)}
        //    and {kDate} = @{nameof(TaskDataTableModel.Date)}
        //    ;";


        private static readonly string SelectTaskFromQuery = $@"select
            {SelectableContentQuery}
            from";

        private static readonly string SelectByIdSufix = $@"
            where {kId} = @{nameof(TaskDataTableModel.ID)}
            ";

        public static string GetSelectQueryByIdAndTable(string tableName)
        {
            string format = "{0} {1} {2}";
            return string.Format(format, SelectTaskFromQuery, tableName, SelectByIdSufix);
        }


        private static readonly string SelectByModeAndDateSufix = $@"
            where {kModeIndex} = @{nameof(TaskDataTableModel.TaskModeIndex)}
            and {kDate} = @{nameof(TaskDataTableModel.Date)}
            ";

        public static string GetSelectQueryModeAndDate(string tableName)
        {
            string format = "{0} {1} {2}";
            return string.Format(format, SelectTaskFromQuery, tableName, SelectByModeAndDateSufix);
        }


        public static readonly string DeleteTable = $@"
            drop table if exists {kTasksTable}
            ";
    }
}

