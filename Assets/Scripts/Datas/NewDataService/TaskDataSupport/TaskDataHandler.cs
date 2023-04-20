using Cysharp.Threading.Tasks;
using Mathy.Data;
using ModestTree;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Mathy.Services
{
    public interface ITaskDataHandler
    {
        UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date);
        UniTask SaveTask(TaskResultData task);
        UniTask UpdateDailyMode(DailyModeData data);
        UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode);
    }


    public class TaskDataHandler : ITaskDataHandler
    {
        private readonly TaskResultsProvider _taskProvider;
        private readonly GeneralResultsProvider _generalProvider;
        private readonly DailyModeProvider _dailyModeProvider;

        private const string kFileFormat = "tasks_results_save_{0}.db";
        private const string kGeneralFileName = "general_results_save.db";

        private string _taskDBFilePath;
        private string _generalDBFilePath;
        private string _saveDirectoryPath;
        private int _currentYear;
        private GeneralResultsData _generalData;
        private static IDbConnection _taskDBConnection;
        private static IDbConnection _generalDBConnection;

        public TaskDataHandler(string directoryPath)
        {
            _saveDirectoryPath = directoryPath;
            _currentYear = DateTime.UtcNow.Year;
            var fileName = string.Format(kFileFormat, _currentYear);
            var saveFilePath = directoryPath + fileName;
            _taskDBFilePath = $"Data Source={saveFilePath}";

            var generalSavePath = directoryPath + kGeneralFileName;
            _generalDBFilePath = $"Data Source={generalSavePath}";

            _taskProvider = new TaskResultsProvider();
            _dailyModeProvider = new DailyModeProvider();
            _generalProvider = new GeneralResultsProvider();
        }

        public async UniTask Init()
        {
            await TryCreateTables();
            await InitProviders();
        }

        private IDbConnection OpenConnection(string path)
        {
            var connection = new SqliteConnection(path);
            connection.Open();
            return connection;
        }

        private void CloseConnection(IDbConnection connection)
        {
            connection.Close();
            connection.Dispose();
        }

        public async UniTask<TaskResultData[]> GetTasksByModeAndDate(TaskMode mode, DateTime date)
        {
            var databasePath = GetPathToTaskResultsDatabase(date);
            if (databasePath.IsEmpty())
            {
                return new TaskResultData[0];
            }
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            var result = await _taskProvider.GetTasksByModeAndDate(mode, date, _taskDBConnection);
            CloseConnection(_taskDBConnection);
            return result;
        }

        public async UniTask SaveTask(TaskResultData task)
        {
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            await _taskProvider.SaveTask(task, _taskDBConnection);
            CloseConnection(_taskDBConnection);
            UpdateGeneralData(task);
            SaveGeneralDataAsync();
        }

        public async UniTask UpdateDailyMode(DailyModeData data)
        {
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            await _dailyModeProvider.UpdateDailyMode(data, _taskDBConnection);
            CloseConnection(_taskDBConnection);
        }

        public async UniTask<DailyModeData> GetDailyModeData(DateTime date, TaskMode mode)
        {
            var databasePath = GetPathToTaskResultsDatabase(date);
            if (databasePath.IsEmpty())
            {
                return new DailyModeData() { Date = date, Mode = mode };
            }

            _taskDBConnection = OpenConnection(databasePath);
            var result = await _dailyModeProvider.GetDailyModeData(date, mode, _taskDBConnection);
            CloseConnection(_taskDBConnection);
            return result;
        }

        private async UniTask TryCreateTables()
        {
            _taskDBConnection = OpenConnection(_taskDBFilePath);
            await _taskProvider.TryCreateTable(_taskDBConnection);
            await _dailyModeProvider.TryCreateTable(_taskDBConnection);
            CloseConnection(_taskDBConnection);
            _generalDBConnection = OpenConnection(_generalDBFilePath);
            await _generalProvider.TryCreateTable(_generalDBConnection);
            CloseConnection (_generalDBConnection);
        }

        private async UniTask InitProviders()
        {
            _generalDBConnection = OpenConnection(_generalDBFilePath);
            _generalData = await _generalProvider.GetDataAsync(_generalDBConnection);
            CloseConnection(_generalDBConnection);
        }

        private void UpdateGeneralData(TaskResultData task)
        {
            _generalData.TotalTasksPlayed++;
            _generalData.TotalCorrectAnswers = task.IsAnswerCorrect ? ++_generalData.TotalCorrectAnswers : _generalData.TotalCorrectAnswers;
            _generalData.TotalPlayedTime += task.Duration;
            var taskDictionary = _generalData.EachTaskPlayed;
            if (!taskDictionary.ContainsKey(task.TaskType))
            {
                taskDictionary.Add(task.TaskType, 0);
            }
            _generalData.EachTaskPlayed[task.TaskType]++;

            var modeDictionary = _generalData.EachModePlayed;
            if (!modeDictionary.ContainsKey(task.Mode))
            {
                modeDictionary.Add(task.Mode, 0);
            }
            _generalData.EachModePlayed[task.Mode]++;
        }

        private async void SaveGeneralDataAsync()
        {
            _generalDBConnection = OpenConnection(_generalDBFilePath);
            await _generalProvider.SaveAsync(_generalData, _generalDBConnection);
            CloseConnection(_generalDBConnection);
        }

        //Method return path to database file based on date.Year, as every year has it own file.db
        private string GetPathToTaskResultsDatabase(DateTime date)
        {
            if (date.Year == _currentYear)
            {
                return _taskDBFilePath;
            }
            else
            {
                var fileName = string.Format(kFileFormat, date.Year);
                var saveFilePath = _saveDirectoryPath + fileName;
                if (File.Exists(saveFilePath))
                {
                    var selectedDatabasePath = $"Data Source={fileName}";
                    return selectedDatabasePath;
                }
                else
                {
                    return "";
                }
            }
        }
    }


    public class TaskResultFormatProcessor
    {
        private const string correctResultColor = "#15c00f";
        private const string wrongResultColor = "#f94934";
        private const string kUnknownElementValue = "?";

        private ITaskDataHandler _taskDataHandler;

        Dictionary<string, string> operatorChars = new Dictionary<string, string>()
        {
            {"Plus", "+"},
            {"Minus", "-"},
            {"Multiply", "x"},
            {"Divide", ":"},
            {"MoreThan", ">"},
            {"LessThan", "<"},
            {"Equal", "="},
            {"QuestionMark", "?"},
        };

        public TaskResultFormatProcessor(ITaskDataHandler dataHandler)
        {
            _taskDataHandler = dataHandler;
        }


        public async UniTask<List<string>> GetTaskResults(TaskMode mode, DateTime date)
        {
            List<string> results = new List<string>();
            var tasks = await _taskDataHandler.GetTasksByModeAndDate(mode, date);

            for (int x = 0, y = tasks.Length; x < y; x++)
            {
                bool isCorrect = tasks[x].IsAnswerCorrect;
                List<string> new_elements = tasks[x].ElementValues;
                List<string> new_operators = tasks[x].OperatorValues;
                List<string> new_variants = tasks[x].VariantValues;
                List<int> new_selectedIndexes = tasks[x].SelectedAnswerIndexes;
                List<int> new_correctIndexes = tasks[x].CorrectAnswerIndexes;

                for (int i = 0; i < new_variants.Count; i++)
                {
                    if (operatorChars.ContainsKey(new_variants[i]))
                        new_variants[i] = operatorChars[new_variants[i]];
                }

                int variantIndex = 0;
                for (int i = 0; i < new_elements.Count; i++)
                {
                    if (new_elements[i] == kUnknownElementValue)
                    {
                        if (variantIndex >= new_selectedIndexes.Count)
                        {
                            new_elements[i] = "?";
                        }
                        else
                        {
                            int index = new_selectedIndexes[variantIndex];
                            new_elements[i] = $"<color={(isCorrect ? correctResultColor : wrongResultColor)}>" +
                                            $"{new_variants[index]}</color>";
                            variantIndex++;
                        }
                    }
                }
                
                var coloredOperatorList = new_operators.Select(o =>
                {
                    if (o == kUnknownElementValue)
                    {
                        return $"<color={(isCorrect ? correctResultColor : wrongResultColor)}>" +
                        $"{new_variants[new_selectedIndexes[0]}</color>";
                    }
                    else
                        return operatorChars.ContainsKey(o) ? operatorChars[o] : o;
                }).ToList();

                StringBuilder sbResult = new StringBuilder();
                for (int i = 0; i < elementList.Count(); i++)
                {
                    sbResult.Append($"{elementList.ElementAt(i)} ");
                    if (i < coloredOperatorList.Count())
                    {
                        if (!coloredOperatorList[i].IsEmpty())
                        {
                            sbResult.Append($"{coloredOperatorList.ElementAt(i)} ");
                        }
                    }
                }
                if (!isCorrect)
                {
                    var correctAnswersValues = correctAnswersList.Select(i => int.Parse(i))
                                .Where(i => i >= 0 && i < variantsList.Length)
                                .Select(i => variantsList[i].TryLocalizeTaskVariant());
                    sbResult.Append($" ({string.Join(", ", correctAnswersValues)})");

                }
                results.Add(sbResult.ToString());
            }
            return results;
        }
    }
}

