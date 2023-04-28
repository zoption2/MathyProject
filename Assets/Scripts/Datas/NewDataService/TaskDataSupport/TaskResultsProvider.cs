using System;
using System.Linq;
using Dapper;
using System.Data;
using Mathy.Data;
using Cysharp.Threading.Tasks;
using Mono.Data.Sqlite;


namespace Mathy.Services.Data
{
    public interface ITaskResultsProvider : IDataProvider
    {
        UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date);
        UniTask SaveTask(TaskResultData task);
    }

    public class TaskResultsProvider : BaseDataProvider, ITaskResultsProvider
    {
        private const string kInsertQueryFormat = "insert into {0} {1}";
        public TaskResultsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date)
        {
            using(var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new TaskResultData() {Date = date, Mode = mode};
                var requestModel = requestData.ConvertToModel();
                var tableModels = await connection.QueryAsync<TaskDataTableModel>(TaskResultsTableRequests.SelectTaskByModeAndDateQuery, requestModel);
                var result = tableModels.Select(x => x.ConvertToData()).ToArray();
                return result;
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

                var idParam = nameof(TaskDataTableModel.ID);
                var dateParam = nameof(TaskDataTableModel.Date);
                var modeParam = nameof(TaskDataTableModel.Mode);
                var modeIndexParam = nameof(TaskDataTableModel.TaskModeIndex);
                var typeParam = nameof(TaskDataTableModel.TaskType);
                var typeIndexParam = nameof(TaskDataTableModel.TaskTypeIndex);
                var elementsParam = nameof(TaskDataTableModel.ElementValues);
                var operatorsParam = nameof(TaskDataTableModel.OperatorValues);
                var variantsParam = nameof(TaskDataTableModel.VariantValues);
                var selectedIndexesParam = nameof(TaskDataTableModel.SelectedAnswerIndexes);
                var correctIndexesParam = nameof(TaskDataTableModel.CorrectAnswerIndexes);
                var isCorrectParam = nameof(TaskDataTableModel.IsAnswerCorrect);
                var durationParam = nameof(TaskDataTableModel.Duration);
                var maxValueParam = nameof(TaskDataTableModel.MaxValue);

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(idParam, requestModel.ID);
                command.Parameters.AddWithValue(dateParam, requestModel.Date);
                command.Parameters.AddWithValue(modeParam, requestModel.Mode);
                command.Parameters.AddWithValue(modeIndexParam, requestModel.TaskModeIndex);
                command.Parameters.AddWithValue(typeParam, requestModel.TaskType);
                command.Parameters.AddWithValue(typeIndexParam, requestModel.TaskTypeIndex);
                command.Parameters.AddWithValue(elementsParam, requestModel.ElementValues);
                command.Parameters.AddWithValue(operatorsParam, requestModel.OperatorValues);
                command.Parameters.AddWithValue(variantsParam, requestModel.VariantValues);
                command.Parameters.AddWithValue(selectedIndexesParam, requestModel.SelectedAnswerIndexes);
                command.Parameters.AddWithValue(correctIndexesParam, requestModel.CorrectAnswerIndexes);
                command.Parameters.AddWithValue(isCorrectParam, requestModel.IsAnswerCorrect);
                command.Parameters.AddWithValue(durationParam, requestModel.Duration);
                command.Parameters.AddWithValue(maxValueParam, requestModel.MaxValue);
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
                await connection.ExecuteAsync(TaskResultsTableRequests.DeleteTable);
            }
        }
    }
}

