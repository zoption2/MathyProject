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
        UniTask<List<DailyModeData>> GetDailyData(DateTime date);
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
                var dateParam = $@"@{nameof(DailyModeTableModel.Date)}";
                var modeParam = $@"@{nameof(DailyModeTableModel.Mode)}";
                var modeIndexParam = $@"@{nameof(DailyModeTableModel.ModeIndex)}";
                var isCompleteParam = $@"@{nameof(DailyModeTableModel.IsComplete)}";
                var countParam = $@"@{nameof(DailyModeTableModel.PlayedTasks)}";
                var correctParam = $@"@{nameof(DailyModeTableModel.CorrectAnswers)}";
                var rateParam = $@"@{nameof(DailyModeTableModel.CorrectRate)}";
                var durationParam = $@"@{nameof(DailyModeTableModel.Duration)}";
                var totalTasksParam = $@"@{nameof(DailyModeTableModel.TotalTasks)}";

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(dateParam, requestModel.Date);
                command.Parameters.AddWithValue(modeParam, requestModel.Mode);
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
                }
                reader.Close();

                query = resultModel.Id == 0
                    ? DailyModeTableRequests.InsertDailyQuery
                    : DailyModeTableRequests.UpdateDailyQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(isCompleteParam, requestModel.IsComplete);
                command.Parameters.AddWithValue(countParam, requestModel.PlayedTasks);
                command.Parameters.AddWithValue(correctParam, requestModel.CorrectAnswers);
                command.Parameters.AddWithValue(rateParam, requestModel.CorrectRate);
                command.Parameters.AddWithValue(durationParam, requestModel.Duration);
                command.Parameters.AddWithValue(totalTasksParam, requestModel.TotalTasks);
                command.Parameters.AddWithValue(dateParam, requestModel.Date);
                command.Parameters.AddWithValue(modeParam, requestModel.Mode);
                command.Parameters.AddWithValue(modeIndexParam, requestModel.ModeIndex);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {

            if (date == DateTime.Today)
            {
                UnityEngine.Debug.Log("Today");
            }

            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new DailyModeData() { Date = date, Mode = mode };
                var requestModel = requestData.ConvertToModel();

                string query = DailyModeTableRequests.SelectByDateAndModeQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                var dateParam = $@"@{nameof(DailyModeTableModel.Date)}";
                command.Parameters.AddWithValue(dateParam, requestModel.Date);
                var modeParam = $@"@{nameof(DailyModeTableModel.Mode)}";
                command.Parameters.AddWithValue(modeParam, requestModel.Mode);
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

        public async UniTask<List<DailyModeData>> GetDailyData(DateTime date)
        {
            throw new NotImplementedException();
            //List<DailyModeData> results = new List<DailyModeData>();
            //using (var connection = new SqliteConnection(_dbFilePath))
            //{
            //    connection.Open();
            //    var requestData = new DailyModeData() { Date = date };
            //    var requestModel = requestData.ConvertToModel();
            //    var models = await connection.QueryAsync<DailyModeTableModel>
            //        (DailyModeTableRequests.SelectByDateQuery, requestModel);
            //    var modelsArray = models.ToArray();
            //    for (int i = 0, j = modelsArray.Length; i < j; i++)
            //    {
            //        var data = modelsArray[i].ConvertToData();
            //        results.Add(data);
            //    }
            //    return results;
            //}
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

