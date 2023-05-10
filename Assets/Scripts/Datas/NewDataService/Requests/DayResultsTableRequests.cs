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
        private const string kRate = "Rate";

        public static readonly string CreateTableQuery = $@"
            create table if not exists {kTableName}
            (
                {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
                {kDate} STRING NOT NULL,
                {kIsComplete} BOOLEAN NOT NULL,
                {kReward} STRING NOT NULL,
                {kRewardIndex} INTEGER NOT NULL,
                {kRate} INTEGER NOT NULL
            )";


        public static readonly string GetCountQyery = $@"SELECT COUNT(*) FROM {kTableName}
            WHERE {kDate} = @{nameof(DayResultTableModel.Date)}
        ;";


        public static readonly string InsertQuery = $@"
            insert into {kTableName}
            (
                {kDate}, {kIsComplete}, {kReward}, {kRewardIndex}, {kRate}
            )
            values(
                @{nameof(DayResultTableModel.Date)},
                @{nameof(DayResultTableModel.IsComplete)},
                @{nameof(DayResultTableModel.Reward)},
                @{nameof(DayResultTableModel.RewardIndex)},
                @{nameof(DayResultTableModel.MiddleRate)}
            )";


        public static readonly string SelectQuery = $@"
            select
                {kId} as {nameof(DayResultTableModel.Id)},
                {kDate} as {nameof(DayResultTableModel.Date)},
                {kIsComplete} as {nameof(DayResultTableModel.IsComplete)},
                {kReward} as {nameof(DayResultTableModel.Reward)},
                {kRewardIndex} as {nameof(DayResultTableModel.RewardIndex)},
                {kRate} as {nameof(DayResultTableModel.MiddleRate)}
            from {kTableName}
            where {kDate} = @{nameof(DayResultTableModel.Date)}
            ";


        public static readonly string UpdateQuery = $@"
            update {kTableName}
            set
                {kIsComplete} = @{nameof(DayResultTableModel.IsComplete)},
                {kReward} = @{nameof(DayResultTableModel.Reward)},
                {kRewardIndex} = @{nameof(DayResultTableModel.RewardIndex)},
                {kRate} = @{nameof(DayResultTableModel.MiddleRate)}
            where {kDate} = @{nameof(DayResultTableModel.Date)}
            ";

        public static readonly string DeleteTable = $@"
            drop table if exists {kTableName}
            ";
    }
}

