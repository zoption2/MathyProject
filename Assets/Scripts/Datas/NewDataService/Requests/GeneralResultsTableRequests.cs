using static UnityEngine.Rendering.DebugUI;

namespace Mathy.Services.Data
{
    public static class GeneralResultsTableRequests
    {
        private const string kGeneralViewName = "TotalTaskResultsView";
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





        public static readonly string CreateGeneralView = $@"create view IF NOT EXISTS {kGeneralViewName}
            as
            select
            (select {KeyValueIntegerTableRequests.kValue} WHERE {KeyValueIntegerTableRequests.kKey} = '{nameof(KeyValuePairKeys.TotalTasksIndexer)}') AS {kTotalTasks},
            sum(case when {TaskResultsTableRequests.kIsCorrect} then 1 else 0 end) as {kTotalCorrect},
            cast((sum(case when {TaskResultsTableRequests.kIsCorrect} then 1 else 0 end) * 100.0 / count(*)) as integer) as {kMiddleRating},
            sum({TaskResultsTableRequests.kDuration}) as {kTasksTime}
            from {KeyValueIntegerTableRequests.kTableName};
            ";


        public static readonly string CreateDetailedView = $@"
            CREATE VIEW IF NOT EXISTS {kDetailedViewName}
            AS
            SELECT 
                {TaskResultsTableRequests.kTaskType},
                COUNT(*) AS {kTotalTasks},
                SUM(CASE WHEN {TaskResultsTableRequests.kIsCorrect} THEN 1 ELSE 0 END) AS {kTotalCorrect},
                CAST((SUM(CASE WHEN {TaskResultsTableRequests.kIsCorrect} THEN 1 ELSE 0 END) * 100.0 / COUNT(*)) AS INTEGER) AS {kMiddleRating},
                SUM({TaskResultsTableRequests.kDuration}) AS {kTasksTime}
            FROM 
                {TaskResultsTableRequests.kTaskType}
            GROUP BY 
                {TaskResultsTableRequests.kTaskType};
            ";


        public static readonly string CreateModeView = $@"
            create view if not exists {kDailyModeViewName}
            as
            select
                {DailyModeTableRequests.kMode},
                {DailyModeTableRequests.kModeIndex} as {kModeIndex},
                sum(case when {DailyModeTableRequests.kIsModeDone} then 1 else 0 end) as {kTotalCompletedMode}
            from
                {DailyModeTableRequests.kDailyModeTable}
            group by
                {DailyModeTableRequests.kMode};
            ";




        public static readonly string SelectFromGeneralTasksViewQuery = $@"select
            {kTotalTasks} as {nameof(GeneralTasksViewModel.TotalTasksPlayed)},
            {kTotalCorrect} as {nameof(GeneralTasksViewModel.TotalCorrectAnswers)},
            {kTasksTime} as {nameof(GeneralTasksViewModel.TotalPlayedTime)},
            {kMiddleRating} as {nameof(GeneralTasksViewModel.MiddleRate)}
            from {kGeneralViewName}
            ;";


        public static readonly string GetGeneralCountViewQuery = $@"SELECT COUNT(*) FROM {kGeneralViewName};";

        public static readonly string GetDetailedCountViewQuery = $@"SELECT COUNT(*) FROM {kDetailedViewName}
            where {kTaskType} = @{nameof(DetailedTasksViewModel.TaskType)}
        ;";

        public static readonly string GetDailyModeCountViewQuery = $@"SELECT COUNT(*) FROM {kDailyModeViewName}
            where {kModeType} = @{nameof(DailyModeViewModel.Mode)}
        ;";


        public static readonly string SelectFromDetailedTaskViewByTypeQuery = $@"
            {kTaskType} as {nameof(DetailedTasksViewModel.TaskType)},
            {kTypeIndex} as {nameof(DetailedTasksViewModel.TaskTypeIndex)},
            {kTotalTasks} as {nameof(DetailedTasksViewModel.TotalTasksPlayed)},
            {kTotalCorrect} as {nameof(DetailedTasksViewModel.TotalCorrectAnswers)},
            {kTasksTime} as {nameof(DetailedTasksViewModel.TotalPlayedTime)},
            {kMiddleRating} as {nameof(DetailedTasksViewModel.MiddleRate)}
            from {kGeneralViewName}
            where {kTaskType} = @{nameof(DetailedTasksViewModel.TaskType)}
            ";



        public static readonly string SelectDailyModeViewByModeQuery = $@"
            {kModeType} as {nameof(DailyModeViewModel.Mode)},
            {kModeIndex} as {nameof(DailyModeViewModel.ModeIndex)},
            {kTotalCompletedMode} as {nameof(DailyModeViewModel.TotalCompleted)}
            from {kDailyModeViewName}
            where {kModeType} = @{nameof(DailyModeViewModel.Mode)}
            ";


        public static readonly string DropGeneralTasksViewQuery = $@"
            drop view if exists {kGeneralViewName}
            ";


        public static readonly string DropDetailedTasksViewQuery = $@"
            drop view if exists {kDetailedViewName}
            ";


        public static readonly string DropDailyModeViewQuery = $@"
            drop view if exists {kDailyModeViewName}
            ";

        public static readonly string DropAllViewsQuery = $@"
            drop view if exists {kGeneralViewName}, {kDetailedViewName}, {kDailyModeViewName}
        ;";
    }

}

