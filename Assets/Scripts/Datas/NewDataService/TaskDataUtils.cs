namespace Mathy.Services
{
    public static class TaskDataUtils
    {
        private const string kTableName = "TasksData";

        public static readonly string SelectByModeQuery = $@"select
            date as {nameof(NewTaskData.Date)}
            mode as {nameof(NewTaskData.Mode)},
            mode_index as {nameof(NewTaskData.TaskModeIndex)},
            task_type as {nameof(NewTaskData.TaskType)},
            task_index as {nameof(NewTaskData.TaskTypeIndex)},
            answer as {nameof(NewTaskData.Answer)},
            is_correct as {nameof(NewTaskData.IsAnswerCorrect)},
            duration as {nameof(NewTaskData.Duration)}
            from {nameof(kTableName)} 
            where mode = @Mode";

        public static readonly string InsertQuery = $@"insert into {nameof(kTableName)}
            (date, mode, mode_index, task_type, task_index, answer, is_correct, duration)
            values
            @{nameof(NewTaskData.Date)},
            @{nameof(NewTaskData.Mode)},
            @{nameof(NewTaskData.TaskModeIndex)},
            @{nameof(NewTaskData.TaskType)},
            @{nameof(NewTaskData.TaskTypeIndex)},
            @{nameof(NewTaskData.Answer)},
            @{nameof(NewTaskData.IsAnswerCorrect)},
            @{nameof(NewTaskData.Duration)}";

        public static readonly string CreateTableQuery = $@"create table if not exists {nameof(kTableName)}
            (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Date TEXT NOT NULL,
            Mode INTEGER NOT NULL,
            TaskModeIndex INTEGER NOT NULL,
            TaskType INTEGER NOT NULL,
            TaskTypeIndex INTEGER NOT NULL,
            Answer TEXT NOT NULL,
            IsAnswerCorrect INTEGER NOT NULL,
            Duration REAL NOT NULL
            );";
    }
}

