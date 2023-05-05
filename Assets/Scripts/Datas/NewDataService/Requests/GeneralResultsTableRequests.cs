namespace Mathy.Services.Data
{
    public static class GeneralResultsTableRequests
    {
        private const string kDetailedViewName = "DetailedTasksResultsView";
        private const string kDailyModeViewName = "DailyModesView";

        private const string kTaskType = "TaskType";
        private const string kTypeIndex = "TypeIndex";
        private const string kModeType = "Mode";
        private const string kModeIndex = "ModeIndex";
        private const string kTotalTasks = "TotalTasks";
        private const string kTotalCorrect = "TotalCorrect";
        private const string kTasksTime = "TotalTime";
        private const string kTotalCompletedMode = "TotalCompleted";
        private const string kMiddleRating = "MiddleRating";




        #region DetailedView
        public static readonly string PrefixDetailedStatisticView = $@"
            CREATE VIEW IF NOT EXISTS {kDetailedViewName}
            AS";

        public static readonly string BodyDetailedStatisticView = $@"
            SELECT 
                {TaskResultsTableRequests.kTaskType},
                {TaskResultsTableRequests.kTaskTypeIndex},
                COUNT(*) AS {kTotalTasks},
                SUM(CASE WHEN {TaskResultsTableRequests.kIsCorrect} THEN 1 ELSE 0 END) AS {kTotalCorrect},
                CAST((SUM(CASE WHEN {TaskResultsTableRequests.kIsCorrect} THEN 1 ELSE 0 END) * 100.0 / COUNT(*)) AS INTEGER) AS {kMiddleRating},
                SUM({TaskResultsTableRequests.kDuration}) AS {kTasksTime}
            FROM";

        public static readonly string SufixDetailedStatisticView = $@"
            GROUP BY 
                {TaskResultsTableRequests.kTaskType}
            ";


        public static readonly string GetDetailedCountViewQuery = $@"SELECT COUNT(*) FROM {kDetailedViewName}
            where {kTaskType} = @{nameof(DetailedTasksViewModel.TaskType)}
        ;";

        public static readonly string GetDailyModeCountViewQuery = $@"SELECT COUNT(*) FROM {kDailyModeViewName}
            where {kModeType} = @{nameof(DailyModeViewModel.Mode)}
        ;";


        public static readonly string SelectFromDetailedTaskViewByTypeQuery = $@"select
            {kTaskType} as {nameof(DetailedTasksViewModel.TaskType)},
            {kTypeIndex} as {nameof(DetailedTasksViewModel.TaskTypeIndex)},
            {kTotalTasks} as {nameof(DetailedTasksViewModel.TotalTasksPlayed)},
            {kTotalCorrect} as {nameof(DetailedTasksViewModel.TotalCorrectAnswers)},
            {kTasksTime} as {nameof(DetailedTasksViewModel.TotalPlayedTime)},
            {kMiddleRating} as {nameof(DetailedTasksViewModel.MiddleRate)}
            from {kDetailedViewName}
            where {kTaskType} = @{nameof(DetailedTasksViewModel.TaskType)}
            ";



        public static readonly string DropDetailedTasksViewQuery = $@"
            drop view if exists {kDetailedViewName}
            ";

        #endregion

        #region ModeView
        public static readonly string CreateModeView = $@"
            create view if not exists {kDailyModeViewName}
            as
            select
                {DailyModeTableRequests.kMode},
                {DailyModeTableRequests.kModeIndex} as {kModeIndex},
                sum(case when {DailyModeTableRequests.kIsModeDone} then 1 else 0 end) as {kTotalCompletedMode},
                sum({DailyModeTableRequests.kPlayedCount}) as {kTotalTasks},
                sum({DailyModeTableRequests.kCorrect}) as {kTotalCorrect},
                CAST(sum({DailyModeTableRequests.kCorrect}) * 100.0 / sum({DailyModeTableRequests.kPlayedCount}) AS INTEGER) AS {kMiddleRating},
                sum({DailyModeTableRequests.kDuration}) as {kTasksTime}
            from
                {DailyModeTableRequests.kDailyModeTable}
            group by
                {DailyModeTableRequests.kMode};
            ";

        public static readonly string SelectDailyModeViewByModeQuery = $@"select
            {kModeType} as {nameof(DailyModeViewModel.Mode)},
            {kModeIndex} as {nameof(DailyModeViewModel.ModeIndex)},
            {kTotalCompletedMode} as {nameof(DailyModeViewModel.TotalCompletedModes)},
            {kTotalTasks} as {nameof(DailyModeViewModel.TotalTasks)},
            {kTotalCorrect} as {nameof(DailyModeViewModel.TotalCorrect)},
            {kMiddleRating} as {nameof(DailyModeViewModel.MiddleRate)},
            {kTasksTime} as {nameof(DailyModeViewModel.TotalTime)}
            from {kDailyModeViewName}
            where {kModeType} = @{nameof(DailyModeViewModel.Mode)}
            ";


        public static readonly string DropDailyModeViewQuery = $@"
            drop view if exists {kDailyModeViewName}
            ";

        #endregion

        public static readonly string DropAllViewsQuery = $@"
            drop view if exists {kDetailedViewName}, {kDailyModeViewName}
        ;";
    }

}

