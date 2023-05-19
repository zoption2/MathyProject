using System;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;


namespace Mathy.Services.Data
{
    public interface IKeyValuePairIntegerProvider : IDataProvider
    {
        UniTask<KeyValueIntegerData> GetDataByKey(string key, int defaultValue = 0);
        UniTask<int> GetIntOrDefaultByKey(string key, int defaultValue = 0);
        UniTask SetValue(string key, int value, DateTime date);
        UniTask IncrementValue(string key, DateTime date);
        void SaveIntValue(string key, int value, DateTime date);
        int GetIntValue(string key, int defaultValue = 0);
    }


    public class KeyValuePairIntegerProvider : BaseDataProvider, IKeyValuePairIntegerProvider
    {
        public KeyValuePairIntegerProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<KeyValueIntegerData> GetDataByKey(string key, int defaultValue = 0)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueIntegerData() { Key = key, Value = defaultValue, Date = DateTime.UtcNow};
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueIntegerTableRequests.GetCountQyery;

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);

                var scaler = await command.ExecuteScalarAsync();
                var value = Convert.ToInt32(scaler);
                if (value == 0)
                {
                    query = KeyValueIntegerTableRequests.InsertQuery;
                    command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                    command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Value), requestModel.Value);
                    command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Date), requestModel.Date);
                    await command.ExecuteNonQueryAsync();
                    return requestData;
                }

                query = KeyValueIntegerTableRequests.SelectByKeyQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                var reader = await command.ExecuteReaderAsync();

                var resultModel = new KeyValueIntegerDataModel();
                while (await reader.ReadAsync())
                {
                    //resultModel.ID = Convert.ToInt32(reader[0]);
                    resultModel.Key = Convert.ToString(reader[0]);
                    resultModel.Value = Convert.ToInt32(reader[1]);
                    resultModel.Date = Convert.ToString(reader[2]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                var result = resultModel.ConvertToData();
                return result;
            }
        }

        public async UniTask<int> GetIntOrDefaultByKey(string key, int defaultValue = 0)
        {
            var data = await GetDataByKey(key, defaultValue);
            return data.Value;
        }

        public async UniTask SetValue(string key, int value, DateTime date)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueIntegerData() { Key = key, Value = value , Date = date};
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueIntegerTableRequests.GetCountQyery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                query = count == 0
                    ? KeyValueIntegerTableRequests.InsertQuery 
                    : KeyValueIntegerTableRequests.UpdateQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Value), requestModel.Value);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Date), requestModel.Date);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }

        public async UniTask IncrementValue(string key, DateTime date)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueIntegerData() { Key = key, Date = date};
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueIntegerTableRequests.InsertOrReplaceQuery;

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Date), requestModel.Date);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }

        public void SaveIntValue(string key, int value, DateTime date)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueIntegerData() { Key = key, Value = value, Date = date };
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueIntegerTableRequests.GetCountQyery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                var scaler = command.ExecuteScalar();
                var count = Convert.ToInt32(scaler);

                query = count == 0
                    ? KeyValueIntegerTableRequests.InsertQuery
                    : KeyValueIntegerTableRequests.UpdateQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Value), requestModel.Value);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Date), requestModel.Date);
                command.ExecuteNonQuery();

                connection.Close();
                connection.Dispose();
            }
        }

        public int GetIntValue(string key, int defaultValue = 0)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new KeyValueIntegerData() { Key = key, Value = defaultValue, Date = DateTime.UtcNow };
                var requestModel = requestData.ConvertToModel();

                var query = KeyValueIntegerTableRequests.GetCountQyery;

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);

                var scaler = command.ExecuteScalar();
                var value = Convert.ToInt32(scaler);
                if (value == 0)
                {
                    query = KeyValueIntegerTableRequests.InsertQuery;
                    command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                    command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Value), requestModel.Value);
                    command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Date), requestModel.Date);
                    command.ExecuteNonQuery();
                    return requestData.Value;
                }

                query = KeyValueIntegerTableRequests.SelectByKeyQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(KeyValueIntegerDataModel.Key), requestModel.Key);
                var reader = command.ExecuteReader();

                var resultModel = new KeyValueIntegerDataModel();
                while (reader.Read())
                {
                    resultModel.Key = Convert.ToString(reader[0]);
                    resultModel.Value = Convert.ToInt32(reader[1]);
                    resultModel.Date = Convert.ToString(reader[2]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                var result = resultModel.ConvertToData();
                return result.Value;
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

