namespace Mathy.Services.Data
{
    public static class KeyValueIntegerTableRequests
    {
        public const string kTableName = "KeyValueInteger";

        private const string kId = "id";
        public const string kKey = "Key";
        public const string kValue = "Value";
        private const string kDate = "Date";

        public static readonly string CreateTable = $@"
            create table if not exists {kTableName}
            (
                {kKey} STRING PRIMARY KEY NOT NULL,
                {kValue} INTEGER NOT NULL,
                {kDate} STRING NOT NULL
            )";


        public static readonly string InsertQuery = $@"insert into {kTableName}
            (
                {kKey}, {kValue}, {kDate}
            )
            values(
                @{nameof(KeyValueIntegerDataModel.Key)},
                @{nameof(KeyValueIntegerDataModel.Value)},
                @{nameof(KeyValueIntegerDataModel.Date)}
            )";


        public static readonly string SelectByKeyQuery = $@"select
            {kKey} as {nameof(KeyValueIntegerDataModel.Key)},
            {kValue} as {nameof(KeyValueIntegerDataModel.Value)},
            {kDate} as {nameof(KeyValueIntegerDataModel.Date)}
            from {kTableName}
            where {kKey} = @{nameof(KeyValueIntegerDataModel.Key)}
            ;";


        public static readonly string GetCountQyery = $@"SELECT COUNT(*) FROM {kTableName}
            WHERE {kKey} = @{nameof(KeyValueIntegerDataModel.Key)}
        ;";


        public static readonly string InsertOrReplaceQuery = $@"INSERT OR REPLACE INTO {kTableName}
            (
                {kKey}, {kValue}, {kDate}
            )
            VALUES (
                @{nameof(KeyValueIntegerDataModel.Key)},
                CASE
                    WHEN EXISTS(SELECT 1 FROM {kTableName} WHERE {kKey} = @{nameof(KeyValueIntegerDataModel.Key)})
                        THEN (SELECT value FROM {kTableName} WHERE {kKey} = @{nameof(KeyValueIntegerDataModel.Key)}) + 1
                    ELSE 1
                END,
                @{nameof(KeyValueIntegerDataModel.Date)}
            );
            ";


        public static readonly string UpdateQuery = $@"update {kTableName}
            set
                {kValue} = @{nameof(KeyValueIntegerDataModel.Value)},
                {kDate} = @{nameof(KeyValueIntegerDataModel.Date)}
            where
                {kKey} = @{nameof(KeyValueIntegerDataModel.Key)}
            ";


        public static readonly string DeleteTable = $@"
            drop table if exists {kTableName}
            ";
    }

}

