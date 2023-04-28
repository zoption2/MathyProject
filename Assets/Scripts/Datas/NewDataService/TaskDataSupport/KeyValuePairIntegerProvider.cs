using System;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;

namespace Mathy.Services.Data
{
    public interface IKeyValuePairIntegerProvider : IDataProvider
    {
        UniTask<KeyValueIntegerData> GetDataByKey(KeyValuePairKeys key, int defaultValue = 0);
        UniTask<int> GetIntOrDefaultByKey(KeyValuePairKeys key, int defaultValue = 0);
        UniTask SaveIntWithKey(KeyValuePairKeys key, int value, DateTime date);
    }


    public class KeyValuePairIntegerProvider : BaseDataProvider, IKeyValuePairIntegerProvider
    {
        public KeyValuePairIntegerProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<KeyValueIntegerData> GetDataByKey(KeyValuePairKeys key, int defaultValue = 0)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueIntegerData() { Key = key, Value = defaultValue};
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueIntegerTableRequests.SelectByKeyQuery;
                var keyParam = $@"@{nameof(KeyValueIntegerDataModel.Key)}";
                var valueParam = $@"@{nameof(KeyValueIntegerDataModel.Value)}";
                var dateParam = $@"@{nameof(KeyValueIntegerDataModel.Date)}";

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(keyParam, requestModel.Key);
                var reader = await command.ExecuteReaderAsync();

                var resultModel = new KeyValueIntegerDataModel();
                while (await reader.ReadAsync())
                {
                    resultModel.ID = Convert.ToInt32(reader[0]);
                    resultModel.Key = Convert.ToString(reader[1]);
                    resultModel.Value = Convert.ToInt32(reader[2]);
                    resultModel.Date = Convert.ToString(reader[3]);
                }
                reader.Close();

                if (resultModel.ID == 0)
                {
                    query = KeyValueIntegerTableRequests.InsertQuery;
                    command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(keyParam, requestModel.Key);
                    command.Parameters.AddWithValue(valueParam, requestModel.Value);
                    command.Parameters.AddWithValue(dateParam, requestModel.Date);
                    await command.ExecuteNonQueryAsync();
                }

                connection.Close();
                connection.Dispose();

                var result = resultModel.ConvertToData();
                return result;
            }
        }

        public async UniTask<int> GetIntOrDefaultByKey(KeyValuePairKeys key, int defaultValue = 0)
        {
            var data = await GetDataByKey(key, defaultValue);
            return data.Value;
        }

        public async UniTask SaveIntWithKey(KeyValuePairKeys key, int value, DateTime date)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueIntegerData() { Key = key, Value = value , Date = date};
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueIntegerTableRequests.SelectByKeyQuery;
                var keyParam = $@"@{nameof(KeyValueIntegerDataModel.Key)}";
                var valueParam = $@"@{nameof(KeyValueIntegerDataModel.Value)}";
                var dateParam = $@"@{nameof(KeyValueIntegerDataModel.Date)}";

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(keyParam, requestModel.Key);
                var reader = await command.ExecuteReaderAsync();

                var resultModel = new KeyValueIntegerDataModel();
                while (await reader.ReadAsync())
                {
                    resultModel.ID = Convert.ToInt32(reader[0]);
                    resultModel.Key = Convert.ToString(reader[1]);
                    resultModel.Value = Convert.ToInt32(reader[2]);
                    resultModel.Date = Convert.ToString(reader[3]);
                }
                reader.Close();

                query = resultModel.ID == 0 
                    ? KeyValueIntegerTableRequests.InsertQuery 
                    : KeyValueIntegerTableRequests.UpdateQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(keyParam, requestModel.Key);
                command.Parameters.AddWithValue(valueParam, requestModel.Value);
                command.Parameters.AddWithValue(dateParam, requestModel.Date);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }

        public override async UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = KeyValueIntegerTableRequests.CreateTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }

        public override async UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = KeyValueIntegerTableRequests.DeleteTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }

}

