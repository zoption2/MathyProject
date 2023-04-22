namespace Mathy.Services.Data
{
    public static class GeneralResultsTableRequests
    {
        private const string kGeneralTable = "GeneralTasksResults";

        private const string kGeneralViewName = "TotalTaskResultsView";
        private const string kDetailedViewName = "DetailedTasksResultsView";

        private const string kTotalTasks = "TotalTasks";
        private const string kTotalCorrect = "TotalCorrect";
        private const string kTasksTime = "TotalTime";

        private const string kEachTaskPlayed = "EachTaskPlayed";
        private const string kMiddleRating = "MiddleRating";
        private const string kEachModePlayed = "EachModePlayed";
        private const string kModeMiddleRating = "ModeMiddleRating";
        private const string kModeCompleted = "ModeCompleted";
        private const string kSkillMiddleRating = "SkillMiddleRating";



        public static readonly string TryCreateTableQuery = $@"create table if not exists {kGeneralTable}
            (
            {kTotalTasks} INTEGER NOT NULL,
            {kTotalCorrect} INTEGER NOT NULL,
            {kTasksTime} DOUBLE NOT NULL,
            {kEachTaskPlayed} STRING NOT NULL,
            {kMiddleRating} STRING NOT NULL,
            {kEachModePlayed} STRING NOT NULL,
            {kModeMiddleRating} STRING NOT NULL,
            {kModeCompleted} STRING NOT NULL,
            {kSkillMiddleRating} STRING NOT NULL
            )";

        public static readonly string CreateGeneralView = $@"create view IF NOT EXISTS {kGeneralViewName}
            as
            select
            count(*) as {kTotalTasks},
            sum(case when {TaskResultsTableRequests.kIsCorrect} then 1 else 0 end) as {kTotalCorrect},
            sum({TaskResultsTableRequests.kDuration}) as {kTasksTime},
            from {TaskResultsTableRequests.kTasksTable};
            ";

        public static readonly string CreateDetailedViews = $@"
            CREATE VIEW IF NOT EXISTS {kDetailedViewName}
            AS
            SELECT 
                {TaskResultsTableRequests.kTaskType} AS PRIMARY KEY,
                COUNT(*) AS {kTotalTasks},
                SUM(CASE WHEN {TaskResultsTableRequests.kIsCorrect} = 1 THEN 1 ELSE 0 END) AS {kTotalCorrect},
                CAST(({kTotalCorrect} * 100.0 / {kTotalTasks}) AS INTEGER) AS {kMiddleRating},
                SUM({TaskResultsTableRequests.kDuration}) AS {kTasksTime}
                
            FROM 
                {TaskResultsTableRequests.kTasksTable}
            GROUP BY 
                {TaskResultsTableRequests.kTaskType};
            ";




        public static readonly string SelectQuery = $@"select
            {kTotalTasks} as {nameof(GeneralResultsTableModel.TotalTasksPlayed)},
            {kTotalCorrect} as {nameof(GeneralResultsTableModel.TotalCorrectAnswers)},
            {kTasksTime} as {nameof(GeneralResultsTableModel.TotalPlayedTime)},
            {kEachTaskPlayed} as {nameof(GeneralResultsTableModel.EachTaskPlayedJson)},
            {kMiddleRating} as {nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
            {kEachModePlayed} as {nameof(GeneralResultsTableModel.EachModePlayedJson)},
            {kModeMiddleRating} as {nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
            {kModeCompleted} as {nameof(GeneralResultsTableModel.ModeCompletedJson)},
            {kSkillMiddleRating} as {nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
            from {kGeneralTable}
            ;";

        public static readonly string InsertQuery = $@"insert into {kGeneralTable}
            ({kTotalTasks}, {kTotalCorrect}, {kTasksTime}, {kEachTaskPlayed}, {kMiddleRating},
            {kEachModePlayed}, {kModeMiddleRating}, {kModeCompleted}, {kSkillMiddleRating})
            values(
                @{nameof(GeneralResultsTableModel.TotalTasksPlayed)},
                @{nameof(GeneralResultsTableModel.TotalCorrectAnswers)},
                @{nameof(GeneralResultsTableModel.TotalPlayedTime)},
                @{nameof(GeneralResultsTableModel.EachTaskPlayedJson)},
                @{nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
                @{nameof(GeneralResultsTableModel.EachModePlayedJson)},
                @{nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
                @{nameof(GeneralResultsTableModel.ModeCompletedJson)},
                @{nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
            )";


        public static readonly string UpdateQuery = $@"update {kGeneralTable}
            set
            {kTotalTasks} = @{nameof(GeneralResultsTableModel.TotalTasksPlayed)},
            {kTotalCorrect} = @{nameof(GeneralResultsTableModel.TotalCorrectAnswers)},
            {kTasksTime} = @{nameof(GeneralResultsTableModel.TotalPlayedTime)},
            {kEachTaskPlayed} = @{nameof(GeneralResultsTableModel.EachTaskPlayedJson)},
            {kMiddleRating} = @{nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
            {kEachModePlayed} = @{nameof(GeneralResultsTableModel.EachModePlayedJson)},
            {kModeMiddleRating} = @{nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
            {kModeCompleted} = @{nameof(GeneralResultsTableModel.ModeCompletedJson)},
            {kSkillMiddleRating} = @{nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
";
    }

}

