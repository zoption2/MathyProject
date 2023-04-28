namespace Mathy.Services.Data
{
    public static class KeyValueIntegerTableRequests
    {
        private const string kTableName = "KeyValueInteger";

        private const string kId = "id";
        private const string kKey = "Key";
        private const string kValue = "Value";
        private const string kDate = "Date";

        public static readonly string CreateTable = $@"
            create table if not exists {kTableName}
            (
                {kId} INTEGER AUTOINCREMENT,
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
            {kId} as {nameof(KeyValueIntegerDataModel.ID)},
            {kKey} as {nameof(KeyValueIntegerDataModel.Key)},
            {kValue} as {nameof(KeyValueIntegerDataModel.Value)},
            {kDate} as {nameof(KeyValueIntegerDataModel.Date)}
            from {kTableName}
            where {kKey} = @{nameof(KeyValueIntegerDataModel.Key)}
            ;";


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

