using Mathy.Data;

namespace Mathy.Services.Data
{
    public static class GradesTableRequests
    {
        private const string kTable = "Grades";

        private const string kId = "id";
        private const string kGrade = "Grade";
        private const string kIsEnable = "IsEnable";


        public static readonly string TryCreateTableQuery = $@"create table if not exists {kTable}
            (
            {kId} INTEGER PRIMARY KEY AUTOINCREMENT,
            {kGrade} INTEGER NOT NULL,
            {kIsEnable} BOOLEAN NOT NULL
            )";


        public static readonly string SelectIsEnableQuery = $@"select
            {kGrade} as {nameof(SkillPlanTableModel.Grade)},
            {kIsEnable} as {nameof(SkillPlanTableModel.IsEnabled)}
            from {kTable}
            where {kIsEnable} = @IsEnable
            ;";


        public static readonly string SelectGradeQuery = $@"select
            {kGrade} as {nameof(SkillPlanTableModel.Grade)},
            {kIsEnable} as {nameof(SkillPlanTableModel.IsEnabled)}
            from {kTable}
            where {kGrade} = @{nameof(SkillPlanTableModel.Grade)}
            ;";


        public static readonly string GetCountQuery = $@"SELECT COUNT(*) FROM {kTable}
            WHERE {kGrade} = @{nameof(SkillPlanTableModel.Grade)}
        ;";


        public static readonly string InsertOrUpdateGradeQuery = $@"
            INSERT OR REPLACE INTO {kTable} 
            ({kGrade}, {kIsEnable})
            VALUES(
                @{nameof(GradeTableModel.Grade)},
                @{nameof(GradeTableModel.IsEnable)}
            )";


        public static readonly string InsertGradeQuery = $@"insert into {kTable}
            ({kGrade}, {kIsEnable})
            values(
            @{nameof(GradeTableModel.Grade)},
            @{nameof(GradeTableModel.IsEnable)}
            )";


        public static readonly string UpdateGradeQuery = $@"update {kTable}
            set {kIsEnable} = @{nameof(GradeTableModel.IsEnable)}
            where {kGrade} = @{nameof(GradeTableModel.Grade)}
            ";


        public static readonly string DeleteTable = $@"
            drop table if exists {kTable}
            ";
    }
}

