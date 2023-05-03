using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mono.Data.Sqlite;
using System;
using System.Text;
using UnityEngine.Rendering;

namespace Mathy.Services.Data
{
    public interface IGeneralResultsProvider : IDataProvider
    {
        UniTask<GeneralTasksViewData> GetGeneralTasksDataAsync();
        UniTask<DetailedTasksViewData> GetDetailedTasksDataAsync(TaskType taskType);
        UniTask<DailyModeViewData> GetDailyModeDataAsync(TaskMode mode);
    }


    public class GeneralResultsProvider : BaseDataProvider, IGeneralResultsProvider
    {
        public GeneralResultsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<GeneralTasksViewData> GetGeneralTasksDataAsync()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var data = new GeneralTasksViewData();
                var requestModel = data.ConvertToModel();

                var query = GeneralResultsTableRequests.GetGeneralCountViewQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                if (count == 0)
                {
                    return data;
                }

                query = GeneralResultsTableRequests.SelectFromGeneralTasksViewQuery;
                command = new SqliteCommand(query, connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    data.TotalTasksPlayed = Convert.ToInt32(reader[0]);
                    data.TotalCorrectAnswers = Convert.ToInt32(reader[1]);
                    data.MiddleRate = Convert.ToInt32(reader[2]);
                    data.TotalPlayedTime = Convert.ToDouble(reader[3]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                return data;
            }
        }

        public async UniTask<DetailedTasksViewData> GetDetailedTasksDataAsync(TaskType taskType)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var data = new DetailedTasksViewData() { TaskType = taskType };
                var requestModel = data.ConvertToModel();

                var query = GeneralResultsTableRequests.GetDetailedCountViewQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DetailedTasksViewModel.TaskType), requestModel.TaskType);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                if (count == 0)
                {
                    return data;
                }

                query = GeneralResultsTableRequests.SelectFromDetailedTaskViewByTypeQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DetailedTasksViewModel.TaskType), requestModel.TaskType);
                var reader = await command.ExecuteReaderAsync();

                var result = new DetailedTasksViewModel();
                while (await reader.ReadAsync())
                {
                    result.TaskType = Convert.ToString(reader[0]);
                    result.TotalTasksPlayed = Convert.ToInt32(reader[1]);
                    result.TotalCorrectAnswers = Convert.ToInt32(reader[2]);
                    result.MiddleRate = Convert.ToInt32(reader[3]);
                    result.TotalPlayedTime = Convert.ToDouble(reader[4]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                return result.ConvertToData();
            }
        }

        public async UniTask<DailyModeViewData> GetDailyModeDataAsync(TaskMode mode)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var data = new DailyModeViewData() { Mode = mode };
                var requestModel = data.ConvertToModel();

                var query = GeneralResultsTableRequests.GetDailyModeCountViewQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DailyModeViewModel.Mode), requestModel.Mode);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                if (count == 0)
                {
                    return data;
                }

                query = GeneralResultsTableRequests.SelectDailyModeViewByModeQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(DailyModeViewModel.Mode), requestModel.Mode);
                var reader = await command.ExecuteReaderAsync();

                var result = new DailyModeViewModel();
                while (await reader.ReadAsync())
                {
                    result.Mode = Convert.ToString(reader[0]);
                    result.ModeIndex = Convert.ToInt32(reader[1]);
                    result.TotalCompleted = Convert.ToInt32(reader[2]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                return result.ConvertToData();
            }
        }


        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                //We can delete view on game start, if potentially they will changed often,
                //and then create new ones. It's fast and save methods with no data lost.
                var query = GeneralResultsTableRequests.DropGeneralTasksViewQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = GeneralResultsTableRequests.DropDetailedTasksViewQuery;
                command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = GeneralResultsTableRequests.DropDailyModeViewQuery;
                command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                var sb = new StringBuilder();
                sb.Append(GeneralResultsTableRequests.CreateGeneralView);
                sb.Append(GeneralResultsTableRequests.CreateDetailedView);
                sb.Append(GeneralResultsTableRequests.CreateModeView);
                query = sb.ToString();
                command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = GeneralResultsTableRequests.DropGeneralTasksViewQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = GeneralResultsTableRequests.DropDetailedTasksViewQuery;
                command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                query = GeneralResultsTableRequests.DropDailyModeViewQuery;
                command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }
    }

}

