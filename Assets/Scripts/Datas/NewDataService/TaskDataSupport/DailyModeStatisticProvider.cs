using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mono.Data.Sqlite;
using System;

namespace Mathy.Services.Data
{
    public interface IDailyModeStatisticProvider : IDataProvider
    {
        UniTask<DailyModeViewData> GetDailyModeDataAsync(TaskMode mode);
    }


    public class DailyModeStatisticProvider : BaseDataProvider, IDailyModeStatisticProvider
    {
        public DailyModeStatisticProvider(string dbFilePath) : base(dbFilePath)
        {
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
                    result.TotalCompletedModes = Convert.ToInt32(reader[2]);
                    result.TotalTasks = Convert.ToInt32(reader[3]);
                    result.TotalCorrect = Convert.ToInt32(reader[4]);
                    result.MiddleRate = Convert.ToInt32(reader[5]);
                    result.TotalTime = Convert.ToDouble(reader[6]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                return result.ConvertToData();
            }
        }

        public async override UniTask TryCreateTable()
        {
            await DeleteTable();
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = GeneralResultsTableRequests.CreateModeView;
                var command = new SqliteCommand(query, connection);
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
                var query = GeneralResultsTableRequests.DropDailyModeViewQuery;
                var command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

