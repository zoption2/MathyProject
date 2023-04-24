﻿namespace Mathy.Services.Data
{
    public static class DailyModeTableRequests
    {
        public const string kDailyModeTable = "DailyMode";

        private const string kId = "id";
        private const string kDate = "Date";
        public const string kMode = "Mode";
        public const string kModeIndex = "ModeIndex";
        public const string kIsModeDone = "IsDone";
        private const string kPlayedCount = "PlayedCount";
        private const string kCorrect = "CorrectAnswers";
        private const string kCorrectRate = "CorrectRate";
        private const string kDuration = "Duration";
        private const string kTotalTasks = "TotalTasks";


        public static readonly string TryCreateDailyModeTableQuery = $@"create table if not exists {kDailyModeTable}
            (
            {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
            {kDate} STRING NOT NULL,
            {kMode} STRING NOT NULL,
            {kModeIndex} INTEGER NOT NULL,
            {kIsModeDone} BOOLEAN NOT NULL,
            {kPlayedCount} INTEGER NOT NULL,
            {kCorrect} INTEGER NOT NULL,
            {kCorrectRate} INTEGER NOT NULL,
            {kDuration} DOUBLE NOT NULL,
            {kTotalTasks} INTEGER NOT NULL
            )";

        private static string _selectableDailyModeTableContent = $@"
            {kId} as {nameof(DailyModeTableModel.Id)},
            {kDate} as {nameof(DailyModeTableModel.Date)},
            {kMode} as {nameof(DailyModeTableModel.Mode)},
            {kModeIndex} as {nameof(DailyModeTableModel.ModeIndex)},
            {kIsModeDone} as {nameof(DailyModeTableModel.IsComplete)},
            {kPlayedCount} as {nameof(DailyModeTableModel.PlayedTasks)},
            {kCorrect} as {nameof(DailyModeTableModel.CorrectAnswers)},
            {kCorrectRate} as {nameof(DailyModeTableModel.CorrectRate)},
            {kDuration} as {nameof(DailyModeTableModel.Duration)},
            {kTotalTasks} as {nameof(DailyModeTableModel.TotalTasks)}
            ";


        public static readonly string UpdateOrInsertDailyQuery = $@"
        INSERT OR REPLACE INTO {kDailyModeTable}
        ({kDate}, {kMode}, {kModeIndex}, {kIsModeDone}, {kPlayedCount})
        SELECT
            @{nameof(DailyModeTableModel.Date)},
            @{nameof(DailyModeTableModel.Mode)},
            @{nameof(DailyModeTableModel.ModeIndex)},
            @{nameof(DailyModeTableModel.IsComplete)},
            @{nameof(DailyModeTableModel.PlayedTasks)}
        WHERE NOT EXISTS (
            SELECT 1 FROM {kDailyModeTable}
            WHERE {kDate} = @{nameof(DailyModeTableModel.Date)}
            AND {kModeIndex} = @{nameof(DailyModeTableModel.ModeIndex)}
        )
        OR {kDate} = @{nameof(DailyModeTableModel.Date)}
        AND {kModeIndex} = @{nameof(DailyModeTableModel.ModeIndex)}
    ";


        public static readonly string InsertDailyQuery = $@"insert into {kDailyModeTable}
            ({kDate}, {kMode}, {kModeIndex}, {kIsModeDone}, {kPlayedCount}, {kCorrect},
            {kCorrectRate}, {kDuration}, {kTotalTasks})
            values(
                @{nameof(DailyModeTableModel.Date)},
                @{nameof(DailyModeTableModel.Mode)},
                @{nameof(DailyModeTableModel.ModeIndex)},
                @{nameof(DailyModeTableModel.IsComplete)},
                @{nameof(DailyModeTableModel.PlayedTasks)},
                @{nameof(DailyModeTableModel.CorrectAnswers)},
                @{nameof(DailyModeTableModel.CorrectRate)},
                @{nameof(DailyModeTableModel.Duration)},
                @{nameof(DailyModeTableModel.TotalTasks)}
            )";

        public static readonly string UpdateDailyQuery = $@"update {kDailyModeTable}
            SET {kIsModeDone} = @{nameof(DailyModeTableModel.IsComplete)},
                {kPlayedCount} = @{nameof(DailyModeTableModel.PlayedTasks)},
                {kCorrect} = @{nameof(DailyModeTableModel.CorrectAnswers)},
                {kCorrectRate} = @{nameof(DailyModeTableModel.CorrectRate)},
                {kDuration} = {kDuration} + @{nameof(DailyModeTableModel.Duration)},
                {kTotalTasks} = @{nameof(DailyModeTableModel.TotalTasks)}
            WHERE {kDate} = @{nameof(DailyModeTableModel.Date)}
            AND {kModeIndex} = @{nameof(DailyModeTableModel.ModeIndex)}";


        public static readonly string SelectByDateAndModeQuery = $@"select
            {_selectableDailyModeTableContent}
            from {kDailyModeTable}
            where {kDate} = @{nameof(DailyModeTableModel.Date)}
            and {kModeIndex} = @{nameof(DailyModeTableModel.ModeIndex)}
            ;";


        public static readonly string SelectByDateQuery = $@"select
            {_selectableDailyModeTableContent}
            from {kDailyModeTable}
            where {kDate} = @{nameof(DailyModeTableModel.Date)}
            ;";
    }

}

