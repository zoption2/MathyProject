using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;
using System;


namespace Mathy.Services.Data
{
    public interface IGradeSettingsProvider : IDataProvider
    {
        UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true);
        UniTask SaveGradeSettings(int grade, bool isEnable);
    }


    public class GradeSettingsProvider : BaseDataProvider, IGradeSettingsProvider
    {
        public GradeSettingsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestModel = new GradeTableModel() { Grade = grade, IsEnable = defaultIsEnable };
                var result = requestModel.IsEnable;

                var query = GradesTableRequests.GetCountQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(GradeTableModel.Grade), requestModel.Grade);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                if (count == 0)
                {
                    query = GradesTableRequests.InsertGradeQuery;
                    command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(GradeTableModel.Grade), requestModel.Grade);
                    command.Parameters.AddWithValue(nameof(GradeTableModel.IsEnable), requestModel.IsEnable);
                    await command.ExecuteNonQueryAsync();
                }
                else
                {
                    query = GradesTableRequests.SelectGradeQuery;
                    command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(GradeTableModel.Grade), requestModel.Grade);
                    var reader = await command.ExecuteReaderAsync();

                    var data = new GradeTableModel();
                    while (await reader.ReadAsync())
                    {
                        data.Grade = Convert.ToInt32(reader[1]);
                        data.IsEnable = Convert.ToBoolean(reader[2]);
                    }
                    reader.Close();
                    result = data.IsEnable;
                }
                connection.Close();
                connection.Dispose();
                return result;
            }
        }

        public async UniTask SaveGradeSettings(int grade, bool isEnable)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestModel = new GradeTableModel() { Grade = grade, IsEnable = isEnable };
                var query = GradesTableRequests.GetCountQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(GradeTableModel.Grade), requestModel.Grade);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                query = count == 0
                    ? GradesTableRequests.InsertGradeQuery
                    : GradesTableRequests.UpdateGradeQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(GradeTableModel.Grade), requestModel.Grade);
                command.Parameters.AddWithValue(nameof(GradeTableModel.IsEnable), requestModel.IsEnable);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = GradesTableRequests.TryCreateTableQuery;
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
                var query = GradesTableRequests.DeleteTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

