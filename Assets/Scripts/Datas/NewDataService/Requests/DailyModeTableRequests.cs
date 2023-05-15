using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace Mathy.Services.Data
{
    public static class DailyModeTableRequests
    {
        public const string kDailyModeTable = "DailyMode";

        private const string kId = "id";
        private const string kDate = "Date";
        public const string kMode = "Mode";
        public const string kModeIndex = "ModeIndex";
        public const string kIsModeDone = "IsDone";
        public const string kPlayedCount = "PlayedCount";
        public const string kCorrect = "CorrectAnswers";
        public const string kCorrectRate = "CorrectRate";
        public const string kDuration = "Duration";
        private const string kTotalTasks = "TotalTasks";
        private const string kTasksIds = "TasksIds";


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
            {kTotalTasks} INTEGER NOT NULL,
            {kTasksIds} STRING NOT NULL
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
            {kTotalTasks} as {nameof(DailyModeTableModel.TotalTasks)},
            {kTasksIds} as {nameof(DailyModeTableModel.TasksIds)}
            ";



        public static readonly string InsertDailyQuery = $@"insert into {kDailyModeTable}
            ({kDate}, {kMode}, {kModeIndex}, {kIsModeDone}, {kPlayedCount}, {kCorrect},
            {kCorrectRate}, {kDuration}, {kTotalTasks}, {kTasksIds})
            values(
                @{nameof(DailyModeTableModel.Date)},
                @{nameof(DailyModeTableModel.Mode)},
                @{nameof(DailyModeTableModel.ModeIndex)},
                @{nameof(DailyModeTableModel.IsComplete)},
                @{nameof(DailyModeTableModel.PlayedTasks)},
                @{nameof(DailyModeTableModel.CorrectAnswers)},
                @{nameof(DailyModeTableModel.CorrectRate)},
                @{nameof(DailyModeTableModel.Duration)},
                @{nameof(DailyModeTableModel.TotalTasks)},
                @{nameof(DailyModeTableModel.TasksIds)}
            )";

        public static readonly string UpdateDailyQuery = $@"update {kDailyModeTable}
            SET {kIsModeDone} = @{nameof(DailyModeTableModel.IsComplete)},
                {kPlayedCount} = @{nameof(DailyModeTableModel.PlayedTasks)},
                {kCorrect} = @{nameof(DailyModeTableModel.CorrectAnswers)},
                {kCorrectRate} = @{nameof(DailyModeTableModel.CorrectRate)},
                {kDuration} = @{nameof(DailyModeTableModel.Duration)},
                {kTotalTasks} = @{nameof(DailyModeTableModel.TotalTasks)},
                {kTasksIds} = @{nameof(DailyModeTableModel.TasksIds)}
            WHERE {kDate} = @{nameof(DailyModeTableModel.Date)}
            AND {kModeIndex} = @{nameof(DailyModeTableModel.ModeIndex)}";


        public static readonly string SelectByDateAndModeQuery = $@"select
            {_selectableDailyModeTableContent}
            from {kDailyModeTable}
            where {kDate} = @{nameof(DailyModeTableModel.Date)}
            and {kMode} = @{nameof(DailyModeTableModel.Mode)}
            ;";


        public static readonly string SelectByDateQuery = $@"select
            {_selectableDailyModeTableContent}
            from {kDailyModeTable}
            where {kDate} = @{nameof(DailyModeTableModel.Date)}
            ;";

        public static string SelectByMonthQuery = $@"select
            {kId} as {nameof(DailyModeTableModel.Id)},
            {kDate} as {nameof(DailyModeTableModel.Date)},
            {kMode} as {nameof(DailyModeTableModel.Mode)},
            {kModeIndex} as {nameof(DailyModeTableModel.ModeIndex)},
            {kIsModeDone} as {nameof(DailyModeTableModel.IsComplete)},
            {kPlayedCount} as {nameof(DailyModeTableModel.PlayedTasks)},
            {kCorrect} as {nameof(DailyModeTableModel.CorrectAnswers)},
            {kCorrectRate} as {nameof(DailyModeTableModel.CorrectRate)},
            {kDuration} as {nameof(DailyModeTableModel.Duration)},
            {kTotalTasks} as {nameof(DailyModeTableModel.TotalTasks)},
            {kTasksIds} as {nameof(DailyModeTableModel.TasksIds)}
                        from {kDailyModeTable}
            where strftime('%Y-%m', {kDate}) = strftime('%Y-%m', @{nameof(DailyModeTableModel.Date)})
            ;";

        public static string SelectCountByMonth = $@"SELECT COUNT(*) FROM {kDailyModeTable}
            where strftime('%Y-%m', {kDate}) = strftime('%Y-%m', @{nameof(DailyModeTableModel.Date)})
        ;";




        public static readonly string DeleteTable = $@"
            drop table if exists {kDailyModeTable}
            ";
    }
}

