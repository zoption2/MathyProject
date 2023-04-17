namespace Mathy.Services
{
    public static class TaskDataUtils
    {
        private const string kTableName = "TasksData";

        public static readonly string SelectByModeQuery = $@"select
            Day as {nameof(NewTaskData.Date)},
            Mode as {nameof(NewTaskData.Mode)},
            ModeIndex as {nameof(NewTaskData.TaskModeIndex)},
            TaskType as {nameof(NewTaskData.TaskType)},
            TaskIndex as {nameof(NewTaskData.TaskTypeIndex)},
            Answer as {nameof(NewTaskData.Answer)},
            IsCorrect as {nameof(NewTaskData.IsAnswerCorrect)},
            Duration as {nameof(NewTaskData.Duration)}
            from {kTableName} 
            where Mode = @Mode";

        public static readonly string InsertQuery = $@"insert into {kTableName}
            (Day, Mode, ModeIndex, TaskType, TaskIndex, Answer, IsCorrect, Duration)
            values
            @{nameof(NewTaskData.Date)},
            @{nameof(NewTaskData.Mode)},
            @{nameof(NewTaskData.TaskModeIndex)},
            @{nameof(NewTaskData.TaskType)},
            @{nameof(NewTaskData.TaskTypeIndex)},
            @{nameof(NewTaskData.Answer)},
            @{nameof(NewTaskData.IsAnswerCorrect)},
            @{nameof(NewTaskData.Duration)}
            returning Id";


        public static readonly string CreateTableQuery = $@"create table if not exists {kTableName}
            (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Day STRING NOT NULL,
            Mode STRING NOT NULL,
            ModeIndex INTEGER NOT NULL,
            TaskType STRING NOT NULL,
            TaskIndex INTEGER NOT NULL,
            Answer STRING NOT NULL,
            IsCorrect BOOLEAN NOT NULL,
            Duration DOUBLE NOT NULL
            );";
    }
}

