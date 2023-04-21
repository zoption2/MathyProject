namespace Mathy.Services.Data
{
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

}

