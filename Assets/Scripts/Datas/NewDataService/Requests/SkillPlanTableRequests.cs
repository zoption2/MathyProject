using Mathy.Data;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace Mathy.Services.Data
{
    public static class SkillPlanTableRequests
    {
        private const string kTable = "SkillPlan";

        private const string kId = "id";
        private const string kGrade = "Grade";
        private const string kSkill = "Skill";
        private const string kIsEnable = "IsEnable";
        private const string kValue = "Value";
        private const string kMinValue = "MinValue";
        private const string kMaxValue = "MaxValue";

        public static readonly string TryCreateTableQuery = $@"create table if not exists {kTable}
            (
            {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
            {kGrade} INTEGER NOT NULL,
            {kSkill} STRING NOT NULL,
            {kIsEnable} BOOLEAN NOT NULL,
            {kValue} INTEGER NOT NULL,
            {kMinValue} INTEGER NOT NULL,
            {kMaxValue} INTEGER NOT NULL
            )";

        public static readonly string SelectAllEntriesQuery = $@"select
            {kGrade} as {nameof(SkillPlanTableModel.Grade)},
            {kSkill} as {nameof(SkillPlanTableModel.Skill)},
            {kIsEnable} as {nameof(SkillPlanTableModel.IsEnabled)},
            {kValue} as {nameof(SkillPlanTableModel.Value)},
            {kMinValue} as {nameof(SkillPlanTableModel.MinValue)},
            {kMaxValue} as {nameof(SkillPlanTableModel.MaxValue)}
            from {kTable}
            ;";

        public static readonly string SelectByGradeAndSkillQuery = $@"SELECT
            {kId} AS {nameof(SkillPlanTableModel.Id)},
            {kGrade} AS {nameof(SkillPlanTableModel.Grade)},
            {kSkill} AS {nameof(SkillPlanTableModel.Skill)},
            {kIsEnable} AS {nameof(SkillPlanTableModel.IsEnabled)},
            {kValue} AS {nameof(SkillPlanTableModel.Value)},
            {kMinValue} AS {nameof(SkillPlanTableModel.MinValue)},
            {kMaxValue} AS {nameof(SkillPlanTableModel.MaxValue)}
            FROM {kTable}
                WHERE {kGrade} = @{nameof(SkillPlanTableModel.Grade)}
                AND {kSkill} = @{nameof(SkillPlanTableModel.Skill)}
            ;";


        public static readonly string InsertEntryQueryTest = $@"if exist(select* from ) begin update end ";


        public static readonly string GetCountQuery = $@"SELECT COUNT(*) as Count
            FROM {kTable}
                where {kGrade} = @{nameof(SkillPlanTableModel.Grade)}
                and {kSkill} = @{nameof(SkillPlanTableModel.Skill)}
            ;";


        public static readonly string InsertEntryQuery = $@"insert into {kTable}
            ({kGrade}, {kSkill}, {kIsEnable}, {kValue}, {kMinValue}, {kMaxValue})
            values(
            @{nameof(SkillPlanTableModel.Grade)},
            @{nameof(SkillPlanTableModel.Skill)},
            @{nameof(SkillPlanTableModel.IsEnabled)},
            @{nameof(SkillPlanTableModel.Value)},
            @{nameof(SkillPlanTableModel.MinValue)},
            @{nameof(SkillPlanTableModel.MaxValue)}
            )";

        public static readonly string UpdateSkillQuery = $@"update {kTable}
            SET {kIsEnable} = @{nameof(SkillPlanTableModel.IsEnabled)},
                {kValue} = @{nameof(SkillPlanTableModel.Value)},
                {kMinValue} = @{nameof(SkillPlanTableModel.MinValue)},
                {kMaxValue} = @{nameof(SkillPlanTableModel.MaxValue)}
            WHERE {kGrade} = @{nameof(SkillPlanTableModel.Grade)}
            AND {kSkill} = @{nameof(SkillPlanTableModel.Skill)}";


        public static readonly string DeleteTable = $@"
            drop table if exists {kTable}
            ";
    }
}

