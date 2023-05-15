using Mathy.Data;

namespace Mathy.Services.Data
{
    public static class SkillStatisticRequests
    {
        private const string kTableName = "SkillStatistic";

        private const string kId = "id";
        private const string kSkill = "Skill";
        private const string kSkillIndex = "SkillIndex";
        private const string kTotal = "Total";
        private const string kCorrect = "Correct";
        private const string kRate = "Rate";
        private const string kDuration = "Duration";
        private const string kGrade = "Grade";


        public static readonly string TryCreateTableQuery = $@"create table if not exists {kTableName}
            (
            {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
            {kSkill} STRING NOT NULL,
            {kSkillIndex} INTEGER NOT NULL,
            {kTotal} INTEGER NOT NULL,
            {kCorrect} INTEGER NOT NULL,
            {kRate} INTEGER NOT NULL,
            {kDuration} DOUBLE NOT NULL,
            {kGrade} INTEGER NOT NULL
            )";


        public static readonly string SelectBySkillAndGradeQuery = $@"select
            {kId} as {nameof(SkillStatisticModel.ID)},
            {kSkill} as {nameof(SkillStatisticModel.Skill)},
            {kSkillIndex} as {nameof(SkillStatisticModel.SkillIndex)},
            {kTotal} as {nameof(SkillStatisticModel.Total)},
            {kCorrect} as {nameof(SkillStatisticModel.Correct)},
            {kRate} as {nameof(SkillStatisticModel.Rate)},
            {kDuration} as {nameof(SkillStatisticModel.Duration)},
            {kGrade} as {nameof(SkillStatisticModel.Grade)}
            from {kTableName}
            where {kSkillIndex} = @{nameof(SkillStatisticModel.SkillIndex)}
            and {kGrade} = @{nameof(SkillStatisticModel.Grade)}
            ;";


        public static readonly string InsertQuery = $@"insert into {kTableName}
            ({kSkill}, {kSkillIndex}, {kTotal}, {kCorrect}, {kRate}, {kDuration}, {kGrade})
            values(
            @{nameof(SkillStatisticModel.Skill)},
            @{nameof(SkillStatisticModel.SkillIndex)},
            @{nameof(SkillStatisticModel.Total)},
            @{nameof(SkillStatisticModel.Correct)},
            @{nameof(SkillStatisticModel.Rate)},
            @{nameof(SkillStatisticModel.Duration)},
            @{nameof(SkillStatisticModel.Grade)}
            )";


        public static readonly string UpdateQuery = $@"update {kTableName}
            set 
                {kSkill} = @{nameof(SkillStatisticModel.Skill)},
                {kSkillIndex} = @{nameof(SkillStatisticModel.SkillIndex)},
                {kTotal} = @{nameof(SkillStatisticModel.Total)},
                {kCorrect} = @{nameof(SkillStatisticModel.Correct)},
                {kRate} = @{nameof(SkillStatisticModel.Rate)},
                {kDuration} = @{nameof(SkillStatisticModel.Duration)},
                {kGrade} = @{nameof(SkillStatisticModel.Grade)}
            where {kSkillIndex} = @{nameof(SkillStatisticModel.SkillIndex)}
            and {kGrade} = @{nameof(SkillStatisticModel.Grade)}
            ";

        public static readonly string UpdateIncrementQuery = $@"UPDATE {kTableName}
    SET 
        {kTotal} = {kTotal} + 1,
        {kCorrect} = {kCorrect} + CASE WHEN @{nameof(SkillStatisticModel.IsCorrectRequest)} = 1 THEN 1 ELSE 0 END,
        {kRate} = ({kCorrect} + CASE WHEN @{nameof(SkillStatisticModel.IsCorrectRequest)} = 1 THEN 1 ELSE 0 END) * 100.0 / ({kTotal} + 1),
        {kDuration} = {kDuration} + @{nameof(SkillStatisticModel.Duration)}
    WHERE 
        {kSkillIndex} = @{nameof(SkillStatisticModel.SkillIndex)} AND
        {kGrade} = @{nameof(SkillStatisticModel.Grade)}";


        public static readonly string GetCountQuery = $@"SELECT COUNT(*) FROM {kTableName}
            where {kSkillIndex} = @{nameof(SkillStatisticModel.SkillIndex)}
            and {kGrade} = @{nameof(SkillStatisticModel.Grade)}
        ;";


        public static readonly string DeleteTableQuery = $@"
            drop table if exists {kTableName}
            ";
    }
}

