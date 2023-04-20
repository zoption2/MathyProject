namespace Mathy.Services
{
    public static class GeneralResultsTableRequests
    {
        private const string kGeneralTable = "GeneralTasksResults";

        private const string kTotalTasks = "TasksPlayed";
        private const string kTotalCorrect = "CorrectAnswers";
        private const string kTasksTime = "TasksTime";
        private const string kEachTaskPlayed = "EachTaskPlayed";
        private const string kTaskMiddleRating = "TasksMiddleRating";
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
            {kTaskMiddleRating} STRING NOT NULL,
            {kEachModePlayed} STRING NOT NULL,
            {kModeMiddleRating} STRING NOT NULL,
            {kModeCompleted} STRING NOT NULL,
            {kSkillMiddleRating} STRING NOT NULL
            )";

        public static readonly string SelectQuery = $@"select
            {kTotalTasks} as {nameof(GeneralResultsTableModel.TotalTasksPlayed)},
            {kTotalCorrect} as {nameof(GeneralResultsTableModel.TotalCorrectAnswers)},
            {kTasksTime} as {nameof(GeneralResultsTableModel.TotalPlayedTime)},
            {kEachTaskPlayed} as {nameof(GeneralResultsTableModel.EachTaskPlayedJson)},
            {kTaskMiddleRating} as {nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
            {kEachModePlayed} as {nameof(GeneralResultsTableModel.EachModePlayedJson)},
            {kModeMiddleRating} as {nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
            {kModeCompleted} as {nameof(GeneralResultsTableModel.ModeCompletedJson)},
            {kSkillMiddleRating} as {nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
            from {kGeneralTable}
            ;";

        public static readonly string InsertQuery = $@"insert into {kGeneralTable}
            ({kTotalTasks}, {kTotalCorrect}, {kTasksTime}, {kEachTaskPlayed}, {kTaskMiddleRating},
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
            {kTaskMiddleRating} = @{nameof(GeneralResultsTableModel.TaskMiddleRatingJson)},
            {kEachModePlayed} = @{nameof(GeneralResultsTableModel.EachModePlayedJson)},
            {kModeMiddleRating} = @{nameof(GeneralResultsTableModel.ModeMiddleRatingJson)},
            {kModeCompleted} = @{nameof(GeneralResultsTableModel.ModeCompletedJson)},
            {kSkillMiddleRating} = @{nameof(GeneralResultsTableModel.SkillTypeMiddleRatingJson)}
";
    }

}

