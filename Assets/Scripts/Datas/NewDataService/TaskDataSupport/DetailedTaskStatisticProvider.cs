using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mono.Data.Sqlite;
using System;
using System.Text;

namespace Mathy.Services.Data
{
    public interface IDetailedTaskStatisticProvider : IDataProvider
    {
        UniTask<DetailedTasksViewData> GetDataAsync(TaskType taskType);
    }


    public class DetailedTaskStatisticProvider : BaseDataProvider, IDetailedTaskStatisticProvider
    {
        public DetailedTaskStatisticProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<DetailedTasksViewData> GetDataAsync(TaskType taskType)
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
                    result.TaskTypeIndex = Convert.ToInt32(reader[1]);
                    result.TotalTasksPlayed = Convert.ToInt32(reader[2]);
                    result.TotalCorrectAnswers = Convert.ToInt32(reader[3]);
                    result.MiddleRate = Convert.ToInt32(reader[4]);
                    result.TotalPlayedTime = Convert.ToDouble(reader[5]);
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
                var prefix = GeneralResultsTableRequests.PrefixDetailedStatisticView;
                var body = GeneralResultsTableRequests.BodyDetailedStatisticView;
                var sufix = GeneralResultsTableRequests.SufixDetailedStatisticView;
                var queryFormat = "{0} {1} {2} {3}";
                var union = "UNION";
                var tasks = (TaskType[])Enum.GetValues(typeof(TaskType));
               
                var sb = new StringBuilder();
                var query = string.Format(queryFormat, prefix, body, tasks[0], sufix);
                sb.Append(query);

                for (int i = 1, j = tasks.Length; i < j; i++)
                {
                    query = string.Format(queryFormat, union, body, tasks[i], sufix);
                    sb.Append(query);
                }
                sb.Append(";");

                query = sb.ToString();
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
                var query = GeneralResultsTableRequests.DropDetailedTasksViewQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
            //    connection.Open();
            //    var abstractQuery = GeneralResultsTableRequests.DropAbstractViewQuery;
            //    var sufix = ";";
            //    var queryFormat = "{0} {1}{2}";

            //    var tasks = (TaskType[])Enum.GetValues(typeof(TaskType));

            //    var sb = new StringBuilder();
            //    string query = "";

            //    for (int i = 0, j = tasks.Length; i < j; i++)
            //    {
            //        query = string.Format(queryFormat, abstractQuery, tasks[i], sufix);
            //        sb.Append(query);
            //    }

            //    SqliteCommand command = new SqliteCommand(query, connection);
            //    await command.ExecuteNonQueryAsync();
            //    connection.Close();
            //    connection.Dispose();
            //}

        }
    }
}

