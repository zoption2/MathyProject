namespace Mathy.Services.Data
{
    public static class KeyValueStringTableRequests
    {
        public const string kTableName = "KeyValueString";

        public const string kKey = "Key";
        public const string kValue = "Value";
        private const string kDate = "Date";

        public static readonly string CreateTable = $@"
            create table if not exists {kTableName}
            (
                {kKey} STRING PRIMARY KEY NOT NULL,
                {kValue} STRING NOT NULL,
                {kDate} STRING NOT NULL
            )";


        public static readonly string InsertQuery = $@"insert into {kTableName}
            (
                {kKey}, {kValue}, {kDate}
            )
            values(
                @{nameof(KeyValueStringDataModel.Key)},
                @{nameof(KeyValueStringDataModel.Value)},
                @{nameof(KeyValueStringDataModel.Date)}
            )";


        public static readonly string SelectByKeyQuery = $@"select
            {kKey} as {nameof(KeyValueStringDataModel.Key)},
            {kValue} as {nameof(KeyValueStringDataModel.Value)},
            {kDate} as {nameof(KeyValueStringDataModel.Date)}
            from {kTableName}
            where {kKey} = @{nameof(KeyValueStringDataModel.Key)}
            ;";


        public static readonly string GetCountQyery = $@"SELECT COUNT(*) FROM {kTableName}
            WHERE {kKey} = @{nameof(KeyValueStringDataModel.Key)}
        ;";


        public static readonly string UpdateQuery = $@"update {kTableName}
            set
                {kValue} = @{nameof(KeyValueStringDataModel.Value)},
                {kDate} = @{nameof(KeyValueStringDataModel.Date)}
            where
                {kKey} = @{nameof(KeyValueIntegerDataModel.Key)}
            ";


        public static readonly string DeleteTable = $@"
            drop table if exists {kTableName}
            ";
    }

}

