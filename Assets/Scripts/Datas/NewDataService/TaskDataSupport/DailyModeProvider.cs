using System;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;
using System.Collections.Generic;

namespace Mathy.Services.Data
{
    public interface IDailyModeProvider : IDataProvider
    {
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
        UniTask<List<DailyModeData>> GetMonthData(DateTime date);
    }


    public class DailyModeProvider : BaseDataProvider, IDailyModeProvider
    {
        public DailyModeProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestModel = data.ConvertToModel();

                var query = DailyModeTableRequests.SelectByDateAndModeQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Date), requestModel.Date);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Mode), requestModel.Mode);
                var reader = await command.ExecuteReaderAsync();

                var resultModel = new DailyModeTableModel();
                while (await reader.ReadAsync())
                {
                    resultModel.Id = Convert.ToInt32(reader[0]);
                    resultModel.Date = Convert.ToString(reader[1]);
                    resultModel.Mode = Convert.ToString(reader[2]);
                    resultModel.ModeIndex = Convert.ToInt32(reader[3]);
                    resultModel.IsComplete = Convert.ToBoolean(reader[4]);
                    resultModel.PlayedTasks = Convert.ToInt32(reader[5]);
                    resultModel.CorrectAnswers = Convert.ToInt32(reader[6]);
                    resultModel.CorrectRate = Convert.ToInt32(reader[7]);
                    resultModel.Duration = Convert.ToDouble(reader[8]);
                    resultModel.TotalTasks = Convert.ToInt32(reader[9]);
                    resultModel.TasksIds = Convert.ToString(reader[10]);
                }
                reader.Close();

                query = resultModel.Id == 0
                    ? DailyModeTableRequests.InsertDailyQuery
                    : DailyModeTableRequests.UpdateDailyQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.IsComplete), requestModel.IsComplete);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.PlayedTasks), requestModel.PlayedTasks);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.CorrectAnswers), requestModel.CorrectAnswers);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.CorrectRate), requestModel.CorrectRate);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Duration), requestModel.Duration);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.TotalTasks), requestModel.TotalTasks);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Date), requestModel.Date);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Mode), requestModel.Mode);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.ModeIndex), requestModel.ModeIndex);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.TasksIds), requestModel.TasksIds);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new DailyModeData() { Date = date, Mode = mode };
                var requestModel = requestData.ConvertToModel();

                string query = DailyModeTableRequests.SelectByDateAndModeQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Date), requestModel.Date);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Mode), requestModel.Mode);
                var reader = await command.ExecuteReaderAsync();
                 
                var result = new DailyModeTableModel();
                while (await reader.ReadAsync())
                {
                    result.Id = Convert.ToInt32(reader[0]);
                    result.Date = Convert.ToString(reader[1]);
                    result.Mode = Convert.ToString(reader[2]);
                    result.ModeIndex = Convert.ToInt32(reader[3]);
                    result.IsComplete = Convert.ToBoolean(reader[4]);
                    result.PlayedTasks = Convert.ToInt32(reader[5]);
                    result.CorrectAnswers = Convert.ToInt32(reader[6]);
                    result.CorrectRate = Convert.ToInt32(reader[7]);
                    result.Duration = Convert.ToDouble(reader[8]);
                    result.TotalTasks = Convert.ToInt32(reader[9]);
                    result.TasksIds = Convert.ToString(reader[10]);
                }
                reader.Close();

                if(result.Id == 0)
                {
                    return requestData;
                }

                connection.Close();
                connection.Dispose();

                var resultDate = result.ConvertToData();
                return resultDate;
            }
        }

        public async UniTask<List<DailyModeData>> GetMonthData(DateTime date)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new DailyModeData() { Date = date };
                var requestModel = requestData.ConvertToModel();
                var result = new List<DailyModeData>();

                string query = DailyModeTableRequests.SelectCountByMonth;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Date), requestModel.Date);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);
                if (count == 0)
                {
                    return result;
                }

                query = DailyModeTableRequests.SelectByMonthQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DailyModeTableModel.Date), requestModel.Date);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var entry = new DailyModeTableModel();
                    entry.Id = Convert.ToInt32(reader[0]);
                    entry.Date = Convert.ToString(reader[1]);
                    entry.Mode = Convert.ToString(reader[2]);
                    entry.ModeIndex = Convert.ToInt32(reader[3]);
                    entry.IsComplete = Convert.ToBoolean(reader[4]);
                    entry.PlayedTasks = Convert.ToInt32(reader[5]);
                    entry.CorrectAnswers = Convert.ToInt32(reader[6]);
                    entry.CorrectRate = Convert.ToInt32(reader[7]);
                    entry.Duration = Convert.ToDouble(reader[8]);
                    entry.TotalTasks = Convert.ToInt32(reader[9]);
                    entry.TasksIds = Convert.ToString(reader[10]);

                    var data = entry.ConvertToData();
                    result.Add(data);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                return result;
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = DailyModeTableRequests.TryCreateDailyModeTableQuery;
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
                var query = DailyModeTableRequests.DeleteTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

