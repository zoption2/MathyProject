﻿using System;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;


namespace Mathy.Services.Data
{
    public interface IDayResultProvider : IDataProvider
    {
        UniTask AddDayResult(DateTime date, bool isComplete, Achievements reward, int middleRate);
        UniTask<DayResultData> GetDayResult(DateTime date);
    }


    public class DayResultProvider : BaseDataProvider, IDayResultProvider
    {
        public DayResultProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask AddDayResult(DateTime date, bool isComplete, Achievements reward, int middleRate)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new DayResultData()
                {
                    Date = date,
                    IsCompleted = isComplete,
                    Reward = reward,
                    MiddleRate = middleRate
                };
                var requestModel = requestData.ConvertToModel();

                var query = DayResultsTableRequests.GetCountQyery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.Date), requestModel.Date);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);
                if (count != 0)
                {
                    return;
                }

                query = DayResultsTableRequests.InsertQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.Date), requestModel.Date);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.IsComplete), requestModel.IsComplete);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.Reward), requestModel.Reward);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.RewardIndex), requestModel.RewardIndex);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.MiddleRate), requestModel.MiddleRate);
                await command.ExecuteScalarAsync();

                connection.Close();
                connection.Dispose();
            }
        }

        public async UniTask<DayResultData> GetDayResult(DateTime date)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new DayResultData() { Date = date };
                var requestModel = requestData.ConvertToModel();

                var query = DayResultsTableRequests.GetCountQyery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.Date), requestModel.Date);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);
                if (count == 0)
                {
                    return requestData;
                }

                query = DayResultsTableRequests.SelectQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DayResultTableModel.Date), requestModel.Date);
                var reader = await command.ExecuteReaderAsync();

                var resultModel = new DayResultTableModel();
                while (await reader.ReadAsync())
                {
                    requestModel.Id = Convert.ToInt32(reader[0]);
                    requestModel.Date = Convert.ToString(reader[1]);
                    requestModel.IsComplete = Convert.ToBoolean(reader[2]);
                    requestModel.Reward = Convert.ToString(reader[3]);
                    requestModel.RewardIndex = Convert.ToInt32(reader[4]);
                    requestModel.MiddleRate = Convert.ToInt32(reader[5]);
                }
                reader.Close();

                var result = resultModel.ConvertToData();
                return result;
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = DayResultsTableRequests.CreateTableQuery;
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
                var query = DayResultsTableRequests.DeleteTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

