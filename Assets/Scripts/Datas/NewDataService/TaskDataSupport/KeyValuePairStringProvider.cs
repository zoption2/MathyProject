using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;
using System;

namespace Mathy.Services.Data
{
    public interface IKeyValuePairStringProvider : IDataProvider
    {
        UniTask<KeyValueStringData> GetDataByKey(string key, string defaultValue = "");
        UniTask<string> GetStringOrDefaultByKey(string key, string defaultValue = "");
        UniTask SetValue(string key, string value, DateTime date);
    }


    public class KeyValuePairStringProvider : BaseDataProvider, IKeyValuePairStringProvider
    {
        public KeyValuePairStringProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<KeyValueStringData> GetDataByKey(string key, string defaultValue = "")
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueStringData() { Key = key, Value = defaultValue, Date = DateTime.UtcNow };
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueStringTableRequests.GetCountQyery;

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Key), requestModel.Key);

                var scaler = await command.ExecuteScalarAsync();
                var value = Convert.ToInt32(scaler);
                if (value == 0)
                {
                    query = KeyValueStringTableRequests.InsertQuery;
                    command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Key), requestModel.Key);
                    command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Value), requestModel.Value);
                    command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Date), requestModel.Date);
                    await command.ExecuteNonQueryAsync();
                    return requestData;
                }

                query = KeyValueStringTableRequests.SelectByKeyQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Key), requestModel.Key);
                var reader = await command.ExecuteReaderAsync();

                var resultModel = new KeyValueStringDataModel();
                while (await reader.ReadAsync())
                {
                    resultModel.Key = Convert.ToString(reader[0]);
                    resultModel.Value = Convert.ToString(reader[1]);
                    resultModel.Date = Convert.ToString(reader[2]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                var result = resultModel.ConvertToData();
                return result;
            }
        }

        public async UniTask<string> GetStringOrDefaultByKey(string key, string defaultValue = "")
        {
            var data = await GetDataByKey(key, defaultValue);
            return data.Value;
        }

        public async UniTask SetValue(string key, string value, DateTime date)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueStringData() { Key = key, Value = value, Date = date };
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueStringTableRequests.GetCountQyery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Key), requestModel.Key);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                query = count == 0
                    ? KeyValueStringTableRequests.InsertQuery
                    : KeyValueStringTableRequests.UpdateQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Key), requestModel.Key);
                command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Value), requestModel.Value);
                command.Parameters.AddWithValue(nameof(KeyValueStringDataModel.Date), requestModel.Date);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = KeyValueStringTableRequests.CreateTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = KeyValueStringTableRequests.DeleteTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

