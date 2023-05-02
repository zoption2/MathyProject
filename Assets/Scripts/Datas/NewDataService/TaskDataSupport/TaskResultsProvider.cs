using System;
using Mathy.Data;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;
using System.Text;
using System.Collections.Generic;

namespace Mathy.Services.Data
{
    public interface ITaskResultsProvider : IDataProvider
    {
        UniTask<TaskResultData> GetTaskById(int id);
        UniTask<List<TaskResultData>> GetTasksByModeAndDate(TaskMode mode, DateTime date);
        UniTask SaveTask(TaskResultData task);
    }

    public class TaskResultsProvider : BaseDataProvider, ITaskResultsProvider
    {
        private const string kInsertQueryFormat = "insert into {0} {1}";
        public TaskResultsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<List<TaskResultData>> GetTasksByModeAndDate(TaskMode mode, DateTime date)
        {
            using(var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new TaskResultData() {Date = date, Mode = mode};
                var requestModel = requestData.ConvertToModel();

                var results = new List<TaskResultData>();
                var parameters = new List<SqliteParameter>();
                StringBuilder sb = new StringBuilder();
                var tasks = (TaskType[])Enum.GetValues(typeof(TaskType));
                var firstQuery = TaskResultsTableRequests.GetSelectQueryModeAndDate(tasks[0].ToString());
                sb.Append(firstQuery);
                for (int i = 1, j = tasks.Length; i < j; i++)
                {
                    var union = "union";
                    var query = TaskResultsTableRequests.GetSelectQueryModeAndDate(tasks[i].ToString());
                    var formatedQuery = string.Format("{0} {1}", union, query);
                    sb.Append(formatedQuery);

                    parameters.Add(new SqliteParameter(nameof(TaskDataTableModel.TaskModeIndex), requestModel.TaskModeIndex));
                    parameters.Add(new SqliteParameter(nameof(TaskDataTableModel.Date), requestModel.Date));
                }
                sb.Append(";");
                var resultQuery = sb.ToString();

                SqliteCommand command = new SqliteCommand(resultQuery, connection);
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }

                var reader = await command.ExecuteReaderAsync();


                while (await reader.ReadAsync())
                {
                    var result = new TaskDataTableModel();

                    result.ID = reader.GetInt32(0);
                    result.Date = reader.GetString(1);
                    result.Mode = reader.GetString(2);
                    result.TaskModeIndex = reader.GetInt32(3);
                    result.TaskType = reader.GetString(4);
                    result.TaskTypeIndex = reader.GetInt32(5);
                    result.SkillType = reader.GetString(6);
                    result.SkillIndex = reader.GetInt32(7);
                    result.ElementValues = reader.GetString(8);
                    result.OperatorValues = reader.GetString(9);
                    result.VariantValues = reader.GetString(10);
                    var indexes = reader.GetValue(11);
                    result.SelectedAnswerIndexes = Convert.ToString(indexes);
                    var correctIndexes = reader.GetValue(12);
                    result.CorrectAnswerIndexes = Convert.ToString(correctIndexes);
                    result.IsAnswerCorrect = reader.GetBoolean(13);
                    result.Duration = reader.GetDouble(14);
                    result.MaxValue = reader.GetInt32(15);
                    

                    var convertedResult = result.ConvertToData();
                    results.Add(convertedResult);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();
                return results;
            }
        }

        public async UniTask<TaskResultData> GetTaskById(int id)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new TaskResultData() { ID = id};
                var requestModel = requestData.ConvertToModel();

                StringBuilder sb = new StringBuilder();
                var tasks = (TaskType[])Enum.GetValues(typeof(TaskType));
                var firstQuery = TaskResultsTableRequests.GetSelectQueryByIdAndTable(tasks[0].ToString());
                sb.Append(firstQuery);
                for (int i = 1, j = tasks.Length; i < j; i++)
                {
                    var union = "union";
                    var query = TaskResultsTableRequests.GetSelectQueryByIdAndTable(tasks[i].ToString());
                    var formatedQuery = string.Format("{0} {1}", union, query);
                    sb.Append(formatedQuery);
                }
                sb.Append(";");
                var resultQuery = sb.ToString();

                SqliteCommand command = new SqliteCommand(resultQuery, connection);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.ID), requestModel.ID);
                var reader = await command.ExecuteReaderAsync();

                var result = new TaskDataTableModel();
                while (await reader.ReadAsync())
                { 
                    result.ID = reader.GetInt32(0);
                    result.Date = reader.GetString(1);
                    result.Mode = reader.GetString(2);
                    result.TaskModeIndex = reader.GetInt32(3);
                    result.TaskType = reader.GetString(4);
                    result.TaskTypeIndex = reader.GetInt32(5);
                    result.SkillType = reader.GetString(6);
                    result.SkillIndex = reader.GetInt32(7);
                    result.ElementValues = reader.GetString(8);
                    result.OperatorValues = reader.GetString(9);
                    result.VariantValues = reader.GetString(10);
                    result.SelectedAnswerIndexes = reader.GetString(11);
                    result.CorrectAnswerIndexes = reader.GetString(12);
                    result.IsAnswerCorrect = reader.GetBoolean(13);
                    result.Duration = reader.GetDouble(14);
                    result.MaxValue = reader.GetInt32(15);
                }
                reader.Close();

                if (result.ID == 0)
                {
                    return requestData;
                }

                connection.Close();
                connection.Dispose();

                var resultDate = result.ConvertToData();
                return resultDate;
            }
        }

        public async UniTask SaveTask(TaskResultData task)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();

                var taskTable = task.TaskType.ToString();

                var requestModel = task.ConvertToModel();

                var query = string.Format(kInsertQueryFormat
                    , taskTable
                    , TaskResultsTableRequests.InsertableContentQuery);

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.ID), requestModel.ID);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.Date), requestModel.Date);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.Mode), requestModel.Mode);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.TaskModeIndex), requestModel.TaskModeIndex);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.TaskType), requestModel.TaskType);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.TaskTypeIndex), requestModel.TaskTypeIndex);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.SkillType), requestModel.SkillType);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.SkillIndex), requestModel.SkillIndex);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.ElementValues), requestModel.ElementValues);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.OperatorValues), requestModel.OperatorValues);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.VariantValues), requestModel.VariantValues);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.SelectedAnswerIndexes), requestModel.SelectedAnswerIndexes);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.CorrectAnswerIndexes), requestModel.CorrectAnswerIndexes);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.IsAnswerCorrect), requestModel.IsAnswerCorrect);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.Duration), requestModel.Duration);
                command.Parameters.AddWithValue(nameof(TaskDataTableModel.MaxValue), requestModel.MaxValue);
                var reader = await command.ExecuteReaderAsync();

                reader.Close();
                connection.Close();
                connection.Dispose();
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var tasks = (TaskType[])Enum.GetValues(typeof(TaskType));
                for (int i = 0, j = tasks.Length; i < j; i++)
                {
                    var query = $@"create table if not exists {tasks[i]} {TaskResultsTableRequests.CreatingTableColumns}";
                    SqliteCommand command = new SqliteCommand(query, connection);
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();
                connection.Dispose();
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = TaskResultsTableRequests.DeleteTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

