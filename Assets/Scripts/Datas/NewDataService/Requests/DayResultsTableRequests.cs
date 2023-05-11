namespace Mathy.Services.Data
{
    public static class DayResultsTableRequests
    {
        private const string kTableName = "DayResult";

        private const string kId = "id";
        private const string kDate = "Date";
        private const string kIsComplete = "IsComplete";
        private const string kReward = "Reward";
        private const string kRewardIndex = "RewardIndex";
        private const string kTotalTasks = "PlayedTasks";
        private const string kCorrectTasks = "CorrectTasks";
        private const string kRate = "Rate";
        private const string kCompletedModes = "CompletedModes";
        private const string kDuration = "Duration";


        public static readonly string CreateTableQuery = $@"
            create table if not exists {kTableName}
            (
                {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
                {kDate} STRING NOT NULL,
                {kIsComplete} BOOLEAN NOT NULL,
                {kReward} STRING NOT NULL,
                {kRewardIndex} INTEGER NOT NULL,
                {kTotalTasks} INTEGER NOT NULL,
                {kCorrectTasks} INTEGER NOT NULL,
                {kRate} INTEGER NOT NULL,
                {kCompletedModes} STRING NOT NULL,
                {kDuration} DOUBLE NOT NULL
            )";


        public static readonly string GetCountQyery = $@"SELECT COUNT(*) FROM {kTableName}
            WHERE {kDate} = @{nameof(DayResultTableModel.Date)}
        ;";


        public static readonly string InsertQuery = $@"
            insert into {kTableName}
            (
                {kDate}, {kIsComplete}, {kReward}, {kRewardIndex}, {kTotalTasks}
                , {kCorrectTasks}, {kRate}, {kCompletedModes}, {kDuration}
            )
            values(
                @{nameof(DayResultTableModel.Date)},
                @{nameof(DayResultTableModel.IsComplete)},
                @{nameof(DayResultTableModel.Reward)},
                @{nameof(DayResultTableModel.RewardIndex)},
                @{nameof(DayResultTableModel.TotalTasks)},
                @{nameof(DayResultTableModel.CorrectTasks)},
                @{nameof(DayResultTableModel.MiddleRate)},
                @{nameof(DayResultTableModel.CompletedModes)},
                @{nameof(DayResultTableModel.Duration)}
            )";


        public static readonly string SelectQuery = $@"
            select
                {kId} as {nameof(DayResultTableModel.Id)},
                {kDate} as {nameof(DayResultTableModel.Date)},
                {kIsComplete} as {nameof(DayResultTableModel.IsComplete)},
                {kReward} as {nameof(DayResultTableModel.Reward)},
                {kRewardIndex} as {nameof(DayResultTableModel.RewardIndex)},
                {kTotalTasks} as {nameof(DayResultTableModel.TotalTasks)},
                {kCorrectTasks} as {nameof(DayResultTableModel.CorrectTasks)},
                {kRate} as {nameof(DayResultTableModel.MiddleRate)},
                {kCompletedModes} as {nameof(DayResultTableModel.CompletedModes)},
                {kDuration} as {nameof(DayResultTableModel.Duration)}
            from {kTableName}
            where {kDate} = @{nameof(DayResultTableModel.Date)}
            ;";


        public static readonly string UpdateQuery = $@"
            update {kTableName}
            set
                {kIsComplete} = @{nameof(DayResultTableModel.IsComplete)},
                {kReward} = @{nameof(DayResultTableModel.Reward)},
                {kRewardIndex} = @{nameof(DayResultTableModel.RewardIndex)},
                {kTotalTasks} = @{nameof(DayResultTableModel.TotalTasks)},
                {kCorrectTasks} = @{nameof(DayResultTableModel.CorrectTasks)},
                {kRate} = @{nameof(DayResultTableModel.MiddleRate)},
                {kCompletedModes} = @{nameof(DayResultTableModel.CompletedModes)},
                {kDuration} = @{nameof(DayResultTableModel.Duration)}
            where {kDate} = @{nameof(DayResultTableModel.Date)}
            ";

        public static readonly string DeleteTable = $@"
            drop table if exists {kTableName}
            ";
    }
}

