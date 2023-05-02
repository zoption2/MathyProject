using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Linq;
using System.Data;
using System;
using Cysharp.Threading.Tasks;
using System.IO;
using Mathy.Core.Tasks.DailyTasks;
using Mathy.Core.Tasks;
using System.Text;
using ModestTree;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEditor.Search;
using UnityEditor.MemoryProfiler;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using static UnityEditor.UIElements.ToolbarMenu;
using static UnityEngine.Rendering.DebugUI;
#endif
namespace Mathy.Data
{
    public class DatabaseHandler
    {
        private const string fileName = "SaveGame.db";
        private const string actualDatabaseVersion = "2.1.5";
        private const string correctResultColor = "#15c00f";
        private const string wrongResultColor = "#f94934";
        private const string kUnknownElementValue = "?";
        //The path to the database file specified in DataManager
        private string databasePath;
        private string saveDirectoryPath;
        private string saveFilePath;
        public TaskResultData? DataToSave { get; set; }

        private Dictionary<TaskType, string> taskTypeTables = new Dictionary<TaskType, string>
        {
            { TaskType.Addition, "Addition" },
            { TaskType.Subtraction, "Subtraction" },
            { TaskType.Comparison, "Comparison" },
            //{ TaskType.Multiplication, "Multiplication" },
            //{ TaskType.Division, "Division" },
            //{ TaskType.ComplexAddSub, "ComplexAddSub" },
            //{ TaskType.RandomArithmetic, "RandomArithmetic" },
            { TaskType.MissingNumber, "MissingNumber" },
            //{ TaskType.ImageOpening, "OpenImage" },
            //{ TaskType.PairsNumbers, "PairsNumbers" },
            //{ TaskType.PairsEquation, "PairsEquation" },
            //{ TaskType.PairsOperands, "PairsOperands" },
            //{ TaskType.ShapeGuessing, "ShapeGuessing" },
            { TaskType.MissingSign, "MissingSign" },
            //{ TaskType.MissingMultipleSigns, "MissingMultipleSigns" },
            { TaskType.IsThatTrue, "IsThatTrue" },
            { TaskType.ComparisonWithMissingNumber, "ComparisonWithMissingNumber" },
            { TaskType.ComparisonMissingElements, "ComparisonMissingElements" },
            { TaskType.AddSubMissingNumber, "AddSubMissingNumber" },
            { TaskType.ComparisonExpressions, "ExpressionsComparison" },
            { TaskType.SumOfNumbers, "SumOfNumbers" },
            { TaskType.MissingExpression, "MissingExpression" },
            { TaskType.CountTo10Images, "CountTo10Images" },
            { TaskType.CountTo10WhichShows, "SelectFromThreeCount" },
            { TaskType.CountTo10Frames, "CountTo10Frames" },
            { TaskType.CountTo20Frames,  "CountTo20Frames"}
        };

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

        //Singleton
        private static readonly DatabaseHandler instance = new DatabaseHandler();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static DatabaseHandler() { }

        private DatabaseHandler() 
        {
            string dataPath = Application.persistentDataPath;
            saveDirectoryPath = dataPath + "/Saves/";
            saveFilePath = saveDirectoryPath + fileName;
            databasePath = $"Data Source={saveFilePath}";

            if (!Directory.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
                this.CreateDataTables();
            }
            else
            {
                if (!File.Exists(saveFilePath))
                {
                    this.CreateDataTables();
                }
                else if (IsDatabaseVersionChanged())
                {
                    _ = ResetSaveFile();
                }
            }
        }

        // Compare the last saved database version with the actual database version and return whether they are different
        public bool IsDatabaseVersionChanged()
        {
            string lastSavedDatabaseVersion = GetLastSavedDatabaseVersion();
            bool isChanged = lastSavedDatabaseVersion != actualDatabaseVersion;
            return isChanged;
        }

        // Retrieve the last saved database version from the SystemInfo table
        private string GetLastSavedDatabaseVersion()
        {
            string databaseVersion = "";
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        string query = "SELECT DatabaseVersion FROM SystemInfo";
                        command.CommandText = query;
                        IDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            databaseVersion = reader.GetString(0);
                        }
                        reader.Close();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Some GetTimeOfModeAndDate error: " + e.ToString());
                }
            }
            return databaseVersion;
        }

        // Update the DatabaseVersion column in the SystemInfo table with the given value
        //private void UpdateSystemInfo(string databaseVersion, string gameVersion)
        //{
        //    Debug.Log("UpdateSystemInfo");
        //    using (SqliteConnection connection = new SqliteConnection(databasePath))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            string query = "UPDATE SystemInfo SET DatabaseVersion = @dbVersion, GameVersion = @gameVersion;";
        //            SqliteCommand command = new SqliteCommand(query, connection);
        //            command.Parameters.AddWithValue("@dbVersion", databaseVersion);
        //            command.Parameters.AddWithValue("@gameVersion", gameVersion);
        //            command.ExecuteNonQuery();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.LogError("Some UpdateSystemInfo error: " + e.ToString());
        //        }
        //    }  
        //}

        private void UpdateSystemInfo(string databaseVersion, string gameVersion)
        {
            Debug.Log("UpdateSystemInfo");
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO SystemInfo (DatabaseVersion, GameVersion) " +
                        "VALUES (@dbVersion, @gameVersion);";
                    SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@dbVersion", databaseVersion);
                    command.Parameters.AddWithValue("@gameVersion", gameVersion);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some UpdateSystemInfo error: " + e.ToString());
                }
            }
        }

        public static DatabaseHandler Instance
        {
            get
            {
                return instance;
            }
        }

        #region HELPERS
        
        private async System.Threading.Tasks.Task<int> GetTaskUniqueID()
        {
            int id = 0;
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                connection.Open();
                
                using (SqliteCommand command = connection.CreateCommand())
                {

                    string query = "SELECT MAX(ID) FROM ( SELECT MAX(Id) as ID FROM Addition " +
                    "UNION SELECT MAX(Id) as ID FROM Subtraction " +
                    "UNION SELECT MAX(Id) as ID FROM AddSubMissingNumber " +
                    "UNION SELECT MAX(Id) as ID FROM Comparison " +
                    "UNION SELECT MAX(Id) as ID FROM ComparisonWithMissingNumber " +
                    "UNION SELECT MAX(Id) as ID FROM ExpressionsComparison " +
                    "UNION SELECT MAX(Id) as ID FROM IsThatTrue " +
                    "UNION SELECT MAX(Id) as ID FROM ComparisonMissingElements " +
                    "UNION SELECT MAX(Id) as ID FROM MissingExpression " +
                    "UNION SELECT MAX(Id) as ID FROM MissingNumber " +
                    "UNION SELECT MAX(Id) as ID FROM MissingSign " +
                    "UNION SELECT MAX(Id) as ID FROM SumOfNumbers " +
                    "UNION SELECT MAX(Id) as ID FROM SelectFromThreeCount " +
                    "UNION SELECT MAX(Id) as ID FROM CountTo10Frames " +
                    "UNION SELECT MAX(Id) as ID FROM CountTo20Frames " +
                    "UNION SELECT MAX(Id) as ID FROM CountTo10Images ) ";

                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            object value = reader[0];
                            if (value.GetType() == typeof(System.DBNull))
                            {
                                id = 0;
                            }
                            else
                            {
                                id = (Convert.ToInt32(value) + 1);
                            }
                        }
                    }
                }
                connection.Close();
            }
            return id;
        }

        private async System.Threading.Tasks.Task<int> GetChallengeUniqueID()
        {
            int id = 0;
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    string query = "SELECT MAX(ID) FROM " +
                    "( SELECT MAX(Id) as ID FROM OpenImage UNION SELECT MAX(Id) as ID FROM PairsNumbers ) ";

                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            object value = reader[0];
                            if (value.GetType() == typeof(System.DBNull))
                            {
                                id = 0;
                            }
                            else
                            {
                                id = (Convert.ToInt32(value) + 1);
                            }
                        }
                    }
                }
                connection.Close();
            }
            return id;
        }
        #endregion


        #region CHECKING
        public async Task<bool> IsTodayModeExist(TaskMode mode)
        {
            bool value = false;
            try
            {
                SqliteConnection connection = new SqliteConnection(databasePath);
                connection.Open();

                string query = "SELECT EXISTS(SELECT 1 FROM TaskMode " +
                    "WHERE strftime('%Y-%m-%d',Date) = @date " +
                    "AND ModeCode = @modeCode LIMIT 1);";

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@modeCode", (int)mode);
                value = Convert.ToBoolean(await command.ExecuteScalarAsync());

                connection.Close();
            }
            catch (Exception e)
            {
                Debug.LogError("Some IsTodayModeExist error: " + e.ToString());
            }
            return value;
        }

        public async Task<bool> IsDateModeCompleted(TaskMode mode, DateTime date)
        {
            bool value = false;
            try
            {
                SqliteConnection connection = new SqliteConnection(databasePath);

                connection.Open();

                string IsModeOfDateDoneQuery = "SELECT IsModeDone FROM TaskMode " +
                    "WHERE strftime('%Y-%m-%d',Date) = @date " +
                    "AND ModeCode = @modeCode;";

                SqliteCommand IsModeOfDateDoneCommand = new SqliteCommand(IsModeOfDateDoneQuery, connection);
                IsModeOfDateDoneCommand.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                IsModeOfDateDoneCommand.Parameters.AddWithValue("@modeCode", (int)mode);
                value = Convert.ToBoolean(await IsModeOfDateDoneCommand.ExecuteScalarAsync());

                connection.Close();
            }
            catch (Exception e)
            {
                Debug.Log("Some IsDateModeCompleted error: " + e.ToString());
            }
            return value;
        }

        public async Task<bool> IsTodayModeCompleted(TaskMode mode)
        {
            bool value = false;
            try
            {
                SqliteConnection connection = new SqliteConnection(databasePath);
                connection.Open();

                string query = "SELECT IsModeDone FROM TaskMode " +
                   "WHERE strftime('%Y-%m-%d',Date) = @date " +
                   "AND ModeCode = @modeCode;";

                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@modeCode", (int)mode);
                value = Convert.ToBoolean(await command.ExecuteScalarAsync());

                connection.Close();
            }
            catch (Exception e)
            {
                Debug.Log("Some IsTodayModeCompleted error: " + e.ToString());
            }
            return value;
        }
        #endregion

        #region SAVING

        public async void CreateDataTables()
        {
            try
            {
                SqliteConnection connection = new SqliteConnection(databasePath);
                connection.Open();

                string query = "CREATE TABLE Addition (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode)," +
                    "Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL," +
                    "Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, " +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE Subtraction (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode)," +
                    "Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL," +
                    "Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, " +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE AddSubMissingNumber (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL," +
                    "Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL," +
                    "Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL," +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE ChallengeStatistics (ChallengeType INTEGER NOT NULL PRIMARY KEY REFERENCES ChallengeTypes (TypeCode)," +
                    "CorrectAnswers INTEGER NOT NULL, WrongAnswers INTEGER NOT NULL, CorrectRate DOUBLE NOT NULL," +
                    "TotalPlayed INTEGER NOT NULL, AverageTime DOUBLE NOT NULL, PracticBestTime DOUBLE NOT NULL);" +

                    "CREATE TABLE ChallengeTypes (TypeCode INTEGER PRIMARY KEY UNIQUE NOT NULL, Name STRING NOT NULL);" +

                    "CREATE TABLE Comparison (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL," +
                    "Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL, Operators STRING NOT NULL," +
                    "Variants STRING NOT NULL, SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL," +
                    " MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE ComparisonWithMissingNumber (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE ExpressionsComparison (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE IsThatTrue (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL," +
                    "Variants STRING NOT NULL, SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL," +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE ComparisonMissingElements (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE MissingExpression (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE MissingNumber (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL," +
                    "Variants STRING NOT NULL, SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL," +
                    "IsUserAnswerCorrect BOOLEAN NOT NULL, MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE MissingSign (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL," +
                    "Variants STRING NOT NULL, SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL," +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE OpenImage (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "ChallengeTypes INTEGER NOT NULL REFERENCES ChallengeTypes (TypeCode), Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Duration DOUBLE NOT NULL," +
                    "MaxNumber INTEGER NOT NULL, CorrectRate DOUBLE NOT NULL);" +

                    "CREATE TABLE PairsNumbers (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "ChallengeTypes INTEGER NOT NULL REFERENCES ChallengeTypes (TypeCode), Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Duration DOUBLE NOT NULL," +
                    "MaxNumber INTEGER NOT NULL, CorrectRate DOUBLE NOT NULL);" +

                    "CREATE TABLE PlayerData (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Name STRING NOT NULL, Birthday DATE NOT NULL, Device STRING NOT NULL," +
                    "OS STRING NOT NULL, OSVersion STRING NOT NULL, Level INTEGER NOT NULL, TotalExperience INTEGER NOT NULL, TotalAppTime INTEGER NOT NULL," +
                    "TotalCorrectAnswers INTEGER NOT NULL, TotalWrongAnswers INTEGER NOT NULL, TotalChallengesPlayed INTEGER NOT NULL);" +

                    "CREATE TABLE SumOfNumbers (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode), Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id)," +
                    "Elements STRING NOT NULL, Operators STRING NOT NULL," +
                    "Variants STRING NOT NULL, SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL," +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE CountTo10Images (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode)," +
                    "Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL," +
                    "Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, " +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE SelectFromThreeCount (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode)," +
                    "Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL," +
                    "Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, " +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE CountTo10Frames (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode)," +
                    "Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL," +
                    "Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, " +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE CountTo20Frames (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, Seed INTEGER NOT NULL," +
                    "TaskTypes INTEGER NOT NULL REFERENCES TaskTypes (TypeCode)," +
                    "Duration DOUBLE NOT NULL, Mode INTEGER NOT NULL REFERENCES TaskMode (Id), Elements STRING NOT NULL," +
                    "Operators STRING NOT NULL, Variants STRING NOT NULL," +
                    "SelectedAnswers STRING NOT NULL, CorrectAnswers STRING NOT NULL, IsUserAnswerCorrect BOOLEAN NOT NULL, " +
                    "MaxNumber INTEGER NOT NULL);" +

                    "CREATE TABLE SystemInfo(DatabaseVersion STRING NOT NULL UNIQUE,GameVersion STRING NOT NULL UNIQUE);" +

                    "CREATE TABLE TaskMode(Id INTEGER NOT NULL UNIQUE PRIMARY KEY AUTOINCREMENT, Name STRING NOT NULL, ModeCode INTEGER NOT NULL," +
                    "Date STRING NOT NULL, IsModeDone BOOLEAN NOT NULL, LastTaskModeIndex INTEGER NOT NULL);" +

                    "CREATE TABLE TaskStatistics (TaskType INTEGER PRIMARY KEY NOT NULL REFERENCES TaskTypes (TypeCode), CorrectAnswers INTEGER NOT NULL," +
                    "WrongAnswers INTEGER NOT NULL, TotalPlayed INTEGER NOT NULL, CorrectRate DOUBLE NOT NULL, AverageTime DOUBLE NOT NULL," +
                    "EndlessBestScore INTEGER NOT NULL, EasyBestScore INTEGER NOT NULL, MediumBestScore INTEGER NOT NULL, HardBestScore INTEGER NOT NULL);" +

                    "CREATE TABLE SkillSettings (Id INTEGER PRIMARY KEY NOT NULL UNIQUE, SkillTypeIndex INTEGER NOT NULL," +
                    "MaxNumber INTEGER NOT NULL, IsActive BOOLEAN NOT NULL, GradeIndex INTEGER NOT NULL);" +

                    "CREATE TABLE TaskTypes (TypeCode INTEGER PRIMARY KEY UNIQUE NOT NULL, Name STRING NOT NULL);" +
                    "CREATE TABLE UserSettings (MaxNumber INTEGER NOT NULL, Language INTEGER NOT NULL, IsMusicOn BOOLEAN NOT NULL, IsSoundsOn BOOLEAN NOT NULL," +
                    "IsVibrationOn BOOLEAN NOT NULL, IsHelpTextOn BOOLEAN NOT NULL, IsHelpVoiceOn BOOLEAN NOT NULL, IsNotificationsOn BOOLEAN NOT NULL," +
                    "DailyRewardLastTime DOUBLE, DailyRewardCurrentDay DATE);";

;

                SqliteCommand createTablesCommand = new SqliteCommand(query, connection);
                await createTablesCommand.ExecuteNonQueryAsync();

                //Filling the TaskTypes and ChallengeTypes Tables
                await FillTaskTypes();
                await FillChallengeTypes();

                connection.Close();
            }
            catch (Exception e)
            {
                Debug.Log("Some error: " + e.ToString());
            }
            // Update the last saved database version to the actual version
            UpdateSystemInfo(actualDatabaseVersion, Application.version);
        }

        //public async System.Threading.Tasks.Task SaveGradeDatas(List<GradeData> gradeDatas)
        //{
        //    using (SqliteConnection connection = new SqliteConnection(databasePath))
        //    {
        //        try
        //        {
        //            await connection.OpenAsync();

        //            foreach (var gradeData in gradeDatas)
        //            {
        //                int gradeIndex = gradeData.GradeIndex;
        //                for (int i = 0; i < gradeData.SkillDatas.Count; i++)
        //                {
        //                    var skillData = gradeData.SkillDatas[i];
        //                    int skillTypeIndex = (int)skillData.SkillType;

        //                    var updateCommand = new SqliteCommand("UPDATE SkillSettings SET MaxNumber = @maxNumber, IsActive = @isActive WHERE GradeIndex = @gradeIndex AND SkillTypeIndex = @skillTypeIndex", connection);
        //                    updateCommand.Parameters.AddWithValue("@maxNumber", skillData.MaxNumber);
        //                    updateCommand.Parameters.AddWithValue("@isActive", skillData.IsActive);
        //                    updateCommand.Parameters.AddWithValue("@gradeIndex", gradeIndex);
        //                    updateCommand.Parameters.AddWithValue("@skillTypeIndex", skillTypeIndex);

        //                    await updateCommand.ExecuteNonQueryAsync();
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("Some error: " + e.ToString());
        //        }
        //    }
        //}

        private async System.Threading.Tasks.Task FillTaskTypes() 
        {
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    List<TaskType> types = new List<TaskType>() { TaskType.Addition, TaskType.Subtraction, TaskType.AddSubMissingNumber,
                    TaskType.Comparison,TaskType.ComparisonExpressions,TaskType.ComparisonWithMissingNumber,
                    TaskType.ComparisonMissingElements, TaskType.MissingExpression, TaskType.MissingNumber, TaskType.IsThatTrue,
                    TaskType.MissingSign, TaskType.SumOfNumbers, TaskType.CountTo10Frames, TaskType.CountTo10Images, TaskType.CountTo10WhichShows };

                    string FillTaskTypesQuery = "";

                    foreach (TaskType type in types)
                    {
                        FillTaskTypesQuery += " INSERT INTO TaskTypes (TypeCode, Name) VALUES ('" + (int)type + "', '" + type.ToString() + "');"; ;
                    }
                    SqliteCommand FillTaskTypesCommand = new SqliteCommand(FillTaskTypesQuery, connection);

                    await FillTaskTypesCommand.ExecuteNonQueryAsync();

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.Log("Some error: " + e.ToString());
                }
            }
        }

        private async System.Threading.Tasks.Task FillChallengeTypes()
        {
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    List<TaskType> types = new List<TaskType>() { TaskType.ImageOpening, TaskType.PairsNumbers};

                    string FillChallengeTypesQuery = "";

                    foreach (TaskType type in types)
                    {
                        FillChallengeTypesQuery += " INSERT INTO ChallengeTypes (TypeCode, Name) VALUES ('" + (int)type + "', '" + type.ToString() + "');"; ;
                    }
                    SqliteCommand FillChallengeTypesCommand = new SqliteCommand(FillChallengeTypesQuery, connection);

                    await FillChallengeTypesCommand.ExecuteNonQueryAsync();

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.Log("Some error: " + e.ToString());
                }
            }
        }

        private async System.Threading.Tasks.Task<int> GetTaskModeId(TaskMode mode, DateTime date)
        {
            int modeId = -1;
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string GetModeIdQuery = "SELECT [Id] FROM TaskMode " +
                        "WHERE strftime('%Y-%m-%d', Date) = @date AND ModeCode = @mode LIMIT 1";
                    
                    SqliteCommand GetModeIdCommand = new SqliteCommand(GetModeIdQuery, connection);
                    GetModeIdCommand.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd")); 
                    GetModeIdCommand.Parameters.AddWithValue("@mode", (int)mode); 

                    int? temp = null;
                    using (var reader = await GetModeIdCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            temp = Convert.ToInt32(reader[0]);
                        }
                    }
                    
                    if(!temp.HasValue)
                    {
                        modeId = 0;
                    }
                    else
                    {
                        modeId = temp.Value;
                    }
                    
                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some SaveTaskData error: " + e.ToString());
                }
            }
            return modeId;
        }

        private async System.Threading.Tasks.Task<List<int>> GetAllModeIds(TaskMode mode)
        {
            List<int> modeIds = new List<int>();
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string GetModeIdQuery = "SELECT [Id] FROM TaskMode WHERE ModeCode = @mode";

                    SqliteCommand GetModeIdCommand = new SqliteCommand(GetModeIdQuery, connection);
                    GetModeIdCommand.Parameters.AddWithValue("@mode", (int)mode);

                    using (var reader = await GetModeIdCommand.ExecuteReaderAsync())
                    {
                        int? temp = null;
                        while (await reader.ReadAsync())
                        {
                            temp = Convert.ToInt32(reader[0]);
                            if (temp.HasValue)
                            {
                                modeIds.Add(temp.Value);
                            }
                        }
                    }

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some error: " + e.ToString());
                }
            }
            return modeIds;
        }

        private async System.Threading.Tasks.Task InsertTaskTables()
        {
            int modeId = await GetTaskModeId(DataToSave.Mode, DateTime.UtcNow);
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string elements = String.Join(",", DataToSave.ElementValues);
                    string operators = String.Join(",", DataToSave.OperatorValues);
                    string variants = String.Join(",", DataToSave.VariantValues);
                    string selectedAnswers = String.Join(",", DataToSave.SelectedAnswerIndexes);
                    string correctAnswers = String.Join(",", DataToSave.CorrectAnswerIndexes);

                    switch (DataToSave.TaskType)
                    {
                        case TaskType.Addition:
                            {
                                string query = "INSERT INTO Addition (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";
                  
                                SqliteCommand AdditionCommand = new SqliteCommand(query, connection);
                                AdditionCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                AdditionCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                AdditionCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                AdditionCommand.Parameters.AddWithValue("@mode", modeId);
                                AdditionCommand.Parameters.AddWithValue("@elements", elements);
                                AdditionCommand.Parameters.AddWithValue("@operators", operators);
                                AdditionCommand.Parameters.AddWithValue("@variants", variants);
                                AdditionCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                AdditionCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                AdditionCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                AdditionCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);
                                
                                await AdditionCommand.ExecuteNonQueryAsync();

                                break;
                            }
                        case TaskType.Subtraction:
                            {
                                string query = "INSERT INTO Subtraction (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand AddSubCommand = new SqliteCommand(query, connection);
                                AddSubCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                AddSubCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                AddSubCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                AddSubCommand.Parameters.AddWithValue("@mode", modeId);
                                AddSubCommand.Parameters.AddWithValue("@elements", elements);
                                AddSubCommand.Parameters.AddWithValue("@operators", operators);
                                AddSubCommand.Parameters.AddWithValue("@variants", variants);
                                AddSubCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                AddSubCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                AddSubCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                AddSubCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await AddSubCommand.ExecuteNonQueryAsync();

                                break;
                            }

                        case TaskType.AddSubMissingNumber:
                            {
                                string AddSubMissingQuery = "INSERT INTO AddSubMissingNumber (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand AddSubMissingCommand = new SqliteCommand(AddSubMissingQuery, connection);
                                AddSubMissingCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                AddSubMissingCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                AddSubMissingCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                AddSubMissingCommand.Parameters.AddWithValue("@mode", modeId);
                                AddSubMissingCommand.Parameters.AddWithValue("@elements", elements);
                                AddSubMissingCommand.Parameters.AddWithValue("@operators", operators);
                                AddSubMissingCommand.Parameters.AddWithValue("@variants", variants);
                                AddSubMissingCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                AddSubMissingCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                AddSubMissingCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                AddSubMissingCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await AddSubMissingCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.Comparison:
                            {
                                string ComparisonQuery = "INSERT INTO Comparison (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand ComparisonCommand = new SqliteCommand(ComparisonQuery, connection);
                                ComparisonCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                ComparisonCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                ComparisonCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                ComparisonCommand.Parameters.AddWithValue("@mode", modeId);
                                ComparisonCommand.Parameters.AddWithValue("@elements", elements);
                                ComparisonCommand.Parameters.AddWithValue("@operators", operators);
                                ComparisonCommand.Parameters.AddWithValue("@variants", variants);
                                ComparisonCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                ComparisonCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                ComparisonCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                ComparisonCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await ComparisonCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.ComparisonExpressions:
                            {
                                string ExpressionsComparisonQuery = "INSERT INTO ExpressionsComparison (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand ExpressionsComparisonCommand = new SqliteCommand(ExpressionsComparisonQuery, connection);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@mode", modeId);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@elements", elements);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@operators", operators);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@variants", variants);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                ExpressionsComparisonCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await ExpressionsComparisonCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.ComparisonWithMissingNumber:
                            {
                                string ComparisonMissingNumQuery = "INSERT INTO ComparisonWithMissingNumber (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand ComparisonMissingNumCommand = new SqliteCommand(ComparisonMissingNumQuery, connection);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@mode", modeId);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@elements", elements);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@operators", operators);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@variants", variants);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await ComparisonMissingNumCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.ComparisonMissingElements:
                            {
                                string ComparisonMissingNumQuery = "INSERT INTO ComparisonMissingElements (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand ComparisonMissingNumCommand = new SqliteCommand(ComparisonMissingNumQuery, connection);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@mode", modeId);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@elements", elements);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@operators", operators);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@variants", variants);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await ComparisonMissingNumCommand.ExecuteNonQueryAsync();
                                break;
                                break;
                            }
                        case TaskType.MissingExpression:
                            {
                                string MissingExpQuery = "INSERT INTO MissingExpression (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand MissingExpCommand = new SqliteCommand(MissingExpQuery, connection);
                                MissingExpCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                MissingExpCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                MissingExpCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                MissingExpCommand.Parameters.AddWithValue("@mode", modeId);
                                MissingExpCommand.Parameters.AddWithValue("@elements", elements);
                                MissingExpCommand.Parameters.AddWithValue("@operators", operators);
                                MissingExpCommand.Parameters.AddWithValue("@variants", variants);
                                MissingExpCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                MissingExpCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                MissingExpCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                MissingExpCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await MissingExpCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.MissingNumber:
                            {
                                string ComparisonMissingNumQuery = "INSERT INTO MissingNumber (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand ComparisonMissingNumCommand = new SqliteCommand(ComparisonMissingNumQuery, connection);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@mode", modeId);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@elements", elements);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@operators", operators);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@variants", variants);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                ComparisonMissingNumCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await ComparisonMissingNumCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.IsThatTrue:
                            {
                                string IsThatTrueQuery = "INSERT INTO IsThatTrue (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand IsThatTrueCommand = new SqliteCommand(IsThatTrueQuery, connection);
                                IsThatTrueCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                IsThatTrueCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                IsThatTrueCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                IsThatTrueCommand.Parameters.AddWithValue("@mode", modeId);
                                IsThatTrueCommand.Parameters.AddWithValue("@elements", elements);
                                IsThatTrueCommand.Parameters.AddWithValue("@operators", operators);
                                IsThatTrueCommand.Parameters.AddWithValue("@variants", variants);
                                IsThatTrueCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                IsThatTrueCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                IsThatTrueCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                IsThatTrueCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await IsThatTrueCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.MissingSign:
                            {
                                string MissingSignQuery = "INSERT INTO MissingSign (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand MissingSignCommand = new SqliteCommand(MissingSignQuery, connection);
                                MissingSignCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                MissingSignCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                MissingSignCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                MissingSignCommand.Parameters.AddWithValue("@mode", modeId);
                                MissingSignCommand.Parameters.AddWithValue("@elements", elements);
                                MissingSignCommand.Parameters.AddWithValue("@operators", operators);
                                MissingSignCommand.Parameters.AddWithValue("@variants", variants);
                                MissingSignCommand.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                MissingSignCommand.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                MissingSignCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                MissingSignCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await MissingSignCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.SumOfNumbers:
                            {
                                string SumOfNumbersQuery = "INSERT INTO SumOfNumbers (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnswers, @correctAnswers, @isUserCorrect, @maxNumb);";

                                SqliteCommand SumOfNumbersCommand = new SqliteCommand(SumOfNumbersQuery, connection);
                                SumOfNumbersCommand.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                SumOfNumbersCommand.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                SumOfNumbersCommand.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                SumOfNumbersCommand.Parameters.AddWithValue("@mode", modeId);
                                SumOfNumbersCommand.Parameters.AddWithValue("@elements", elements);
                                SumOfNumbersCommand.Parameters.AddWithValue("@operators", operators);
                                SumOfNumbersCommand.Parameters.AddWithValue("@variants", variants);
                                SumOfNumbersCommand.Parameters.AddWithValue("@selectedAnswers", selectedAnswers);
                                SumOfNumbersCommand.Parameters.AddWithValue("@correctAnswers", correctAnswers);
                                SumOfNumbersCommand.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                SumOfNumbersCommand.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await SumOfNumbersCommand.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.CountTo10Images:
                            {
                                string query = "INSERT INTO CountTo10Images (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand command = new SqliteCommand(query, connection);
                                command.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                command.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                command.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                command.Parameters.AddWithValue("@mode", modeId);
                                command.Parameters.AddWithValue("@elements", elements);
                                command.Parameters.AddWithValue("@operators", operators);
                                command.Parameters.AddWithValue("@variants", variants);
                                command.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                command.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                command.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                command.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await command.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.CountTo10WhichShows:
                            {
                                string query = "INSERT INTO SelectFromThreeCount (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand command = new SqliteCommand(query, connection);
                                command.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                command.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                command.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                command.Parameters.AddWithValue("@mode", modeId);
                                command.Parameters.AddWithValue("@elements", elements);
                                command.Parameters.AddWithValue("@operators", operators);
                                command.Parameters.AddWithValue("@variants", variants);
                                command.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                command.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                command.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                command.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await command.ExecuteNonQueryAsync();
                                break;
                            }

                        case TaskType.CountTo10Frames:
                            {
                                string query = "INSERT INTO CountTo10Frames (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @seed, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand command = new SqliteCommand(query, connection);
                                command.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                command.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                command.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                command.Parameters.AddWithValue("@mode", modeId);
                                command.Parameters.AddWithValue("@elements", elements);
                                command.Parameters.AddWithValue("@operators", operators);
                                command.Parameters.AddWithValue("@variants", variants);
                                command.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                command.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                command.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                command.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await command.ExecuteNonQueryAsync();
                                break;
                            }
                        case TaskType.CountTo20Frames:
                            {
                                string query = "INSERT INTO CountTo20Frames (Id, TaskTypes, Duration, Mode, Elements, Operators, " +
                                    "Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, MaxNumber) " +
                                    "VALUES( @id, @taskType, @duration, @mode, @elements, @operators, @variants, @selectedAnsw, @correctAnsw, @isUserCorrect, @maxNumb);";

                                SqliteCommand command = new SqliteCommand(query, connection);
                                command.Parameters.AddWithValue("@id", await GetTaskUniqueID());
                                command.Parameters.AddWithValue("@taskType", (int)DataToSave.TaskType);
                                command.Parameters.AddWithValue("@duration", DataToSave.Duration);
                                command.Parameters.AddWithValue("@mode", modeId);
                                command.Parameters.AddWithValue("@elements", elements);
                                command.Parameters.AddWithValue("@operators", operators);
                                command.Parameters.AddWithValue("@variants", variants);
                                command.Parameters.AddWithValue("@selectedAnsw", selectedAnswers);
                                command.Parameters.AddWithValue("@correctAnsw", correctAnswers);
                                command.Parameters.AddWithValue("@isUserCorrect", DataToSave.IsAnswerCorrect);
                                command.Parameters.AddWithValue("@maxNumb", GameSettingsManager.Instance.MaxNumber);

                                await command.ExecuteNonQueryAsync();
                                break;
                            }
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some SaveTaskData error: " + e.ToString());
                }
            }
        }


        private async System.Threading.Tasks.Task UpdateTaskStatisticTables(TaskType type)
        {
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string AddQuery = "";
                    SqliteCommand AddCommand;
                    if (await IsTaskStatisticExists(DataToSave.TaskType))
                    {
                        AddQuery = "UPDATE TaskStatistics SET CorrectAnswers = @correctAnsw, WrongAnswers = @wrongAnsw, " +
                        " TotalPlayed = @totalPlayed, CorrectRate = @rate, AverageTime = @time" +
                        " WHERE TaskType = @type;";
                                    
                        AddCommand = new SqliteCommand(AddQuery, connection);
                        AddCommand.Parameters.AddWithValue("@type", (int)type);

                        TaskStatisticData statisticData = await GetTaskStatisticData(type);

                        if (statisticData!= null) 
                        {

                            if (DataToSave.IsAnswerCorrect)
                            {
                                AddCommand.Parameters.AddWithValue("@correctAnsw", statisticData.CorrectAnswers + 1);
                                AddCommand.Parameters.AddWithValue("@wrongAnsw", statisticData.WrongAnswers );
                            }
                            else
                            {
                                AddCommand.Parameters.AddWithValue("@correctAnsw", statisticData.CorrectAnswers );
                                AddCommand.Parameters.AddWithValue("@wrongAnsw", statisticData.WrongAnswers + 1 );
                            }
                            AddCommand.Parameters.AddWithValue("@totalPlayed", statisticData.TotalPlayed + 1 );
                                        
                            //Temp like this
                            AddCommand.Parameters.AddWithValue("@rate", 1);
                            AddCommand.Parameters.AddWithValue("@time", 1);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                                    
                    }
                    else
                    {
                        AddQuery = "INSERT INTO TaskStatistics ( TaskType, CorrectAnswers, WrongAnswers, TotalPlayed, CorrectRate, " +
                        "AverageTime, EndlessBestScore, EasyBestScore, MediumBestScore, HardBestScore) " +
                        "VALUES ( @type, @correctAnsw, @wrongAnsw, @totalPlayed, @rate, @time, @endlesscore, @easyScore, @mediumScore, @hardScore );";

                        AddCommand = new SqliteCommand(AddQuery, connection);
                        AddCommand.Parameters.AddWithValue("@type", (int)type);
                        if (DataToSave.IsAnswerCorrect)
                        {
                            AddCommand.Parameters.AddWithValue("@correctAnsw", 1);
                            AddCommand.Parameters.AddWithValue("@wrongAnsw", 0);
                            AddCommand.Parameters.AddWithValue("@rate", 1);
                        }
                        else
                        {
                            AddCommand.Parameters.AddWithValue("@correctAnsw", 0);
                            AddCommand.Parameters.AddWithValue("@wrongAnsw", 1);
                            AddCommand.Parameters.AddWithValue("@rate", 0);
                        }

                        AddCommand.Parameters.AddWithValue("@totalPlayed", 1);
                        AddCommand.Parameters.AddWithValue("@time", DataToSave.Duration);
                        AddCommand.Parameters.AddWithValue("@endlesScore", 0);
                        AddCommand.Parameters.AddWithValue("@easyScore", 0);
                        AddCommand.Parameters.AddWithValue("@mediumScore", 0);
                        AddCommand.Parameters.AddWithValue("@hardScore", 0);
                    }

                    await AddCommand.ExecuteNonQueryAsync();

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some UpdateTaskStatisticTables error: " + e.ToString());
                }
            }
        }

        private async System.Threading.Tasks.Task<TaskStatisticData> GetTaskStatisticData(TaskType type)
        {
            TaskStatisticData statisticData = new TaskStatisticData();
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string GetTaskStatisticDataQuery = "SELECT CorrectAnswers, WrongAnswers, TotalPlayed, CorrectRate, AverageTime " +
                        "FROM TaskStatistics WHERE TaskType = @type;";

                    SqliteCommand GetTaskStatisticDataCommand = new SqliteCommand(GetTaskStatisticDataQuery, connection);
                    GetTaskStatisticDataCommand.Parameters.AddWithValue("@type", (int)type );

                    using (var GetModeIdReader = await GetTaskStatisticDataCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await GetModeIdReader.ReadAsync())
                        {
                            statisticData.CorrectAnswers = Convert.ToInt32(GetModeIdReader[0]);
                            statisticData.WrongAnswers = Convert.ToInt32(GetModeIdReader[1]);
                            statisticData.TotalPlayed = Convert.ToInt32(GetModeIdReader[2]);
                            statisticData.CorrectRate = Convert.ToDouble(GetModeIdReader[3]);
                            statisticData.AverageTime = Convert.ToDouble(GetModeIdReader[4]);
                        }
                    }

                    connection.Close();

                    return statisticData;
                }
                catch (Exception e)
                {
                    Debug.LogError("Some UpdateTaskStatisticTables error: " + e.ToString());
                }
            }
            return null;
        }

        private async System.Threading.Tasks.Task<bool> IsTaskStatisticExists(TaskType type)
        {
            bool result = false;
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT EXISTS( SELECT 1 FROM TaskStatistics WHERE TaskType = @type LIMIT 1 );";

                    SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@type", (int)type);
                    result = Convert.ToBoolean(await command.ExecuteScalarAsync());

                    connection.Close();

                    return result;
                }
                catch (Exception e)
                {
                    Debug.LogError("Some UpdateTaskStatisticTables error: " + e.ToString());
                }
            }
            return result;
        }


        //Saving task data to database
        public async void SaveTaskData()
        {
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string TaskModeQuery = "INSERT INTO TaskMode (Name, ModeCode, Date, LastTaskModeIndex) " +
                        "VALUES(@name, @mode, @date, @lastIndex);";

                    SqliteCommand TaskModeCommand = new SqliteCommand(TaskModeQuery, connection);
                    TaskModeCommand.Parameters.AddWithValue("@name", DataToSave.Mode.ToString());
                    TaskModeCommand.Parameters.AddWithValue("@mode", (int)DataToSave.Mode);
                    TaskModeCommand.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                   // TaskModeCommand.Parameters.AddWithValue("@lastIndex", DataToSave.TaskModeIndex);
                    await TaskModeCommand.ExecuteNonQueryAsync();

                    await InsertTaskTables();

                    await UpdateTaskStatisticTables(DataToSave.TaskType);

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some SaveTaskData error: " + e.ToString());
                }
            }
        }

        public async System.Threading.Tasks.Task SaveChallengeData(ChallengeData data)
        {
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string TaskModeQuery = "INSERT INTO TaskMode (Name, ModeCode, Date, IsModeDone, LastTaskModeIndex) " +
                        "VALUES(@name, @mode, @date, @isDone, @lastIndex);";

                    SqliteCommand TaskModeCommand = new SqliteCommand(TaskModeQuery, connection);
                    TaskModeCommand.Parameters.AddWithValue("@name", data.Mode.ToString());
                    TaskModeCommand.Parameters.AddWithValue("@mode", (int)data.Mode);
                    TaskModeCommand.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                    TaskModeCommand.Parameters.AddWithValue("@isDone", data.IsDone);
                    TaskModeCommand.Parameters.AddWithValue("@lastIndex", 0);
                    await TaskModeCommand.ExecuteNonQueryAsync();

                    //Insert into differrente tables
                    await InsertChallengeTables(data);

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("SaveChallengeData error: " + e.ToString());
                }
            }
        }

        private async System.Threading.Tasks.Task InsertChallengeTables(ChallengeData challengeData)
        {
            int modeId = await GetTaskModeId(challengeData.Mode, DateTime.UtcNow);
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    switch (challengeData.TaskType)
                    {
                        case TaskType.ImageOpening:
                            {
                                string OpenImageQuery = "INSERT INTO OpenImage (Id, ChallengeTypes, Mode, Duration, MaxNumber, CorrectRate)" +
                                "VALUES( @id, @challengeType, @mode, @duration, @maxNumb, @correctRate );";

                                SqliteCommand OpenImageCommand = new SqliteCommand(OpenImageQuery, connection);
                                OpenImageCommand.Parameters.AddWithValue("@id", await GetChallengeUniqueID());
                                OpenImageCommand.Parameters.AddWithValue("@challengeType", (int)challengeData.TaskType);
                                OpenImageCommand.Parameters.AddWithValue("@mode", modeId);
                                OpenImageCommand.Parameters.AddWithValue("@duration", challengeData.Duration.TotalMilliseconds);
                                OpenImageCommand.Parameters.AddWithValue("@maxNumb", challengeData.MaxNumber);
                                OpenImageCommand.Parameters.AddWithValue("@correctRate", challengeData.CorrectRate);

                                await OpenImageCommand.ExecuteNonQueryAsync();
                                //Debug.LogError("ImageOpening");

                                break;
                            }
                        case TaskType.PairsNumbers:
                            {
                                string PairsNumbersQuery = "INSERT INTO PairsNumbers (Id, ChallengeTypes, Mode, Duration, MaxNumber, CorrectRate)" +
                                "VALUES( @id, @challengeType, @mode, @duration, @maxNumb, @correctRate );";

                                SqliteCommand PairsNumbersCommand = new SqliteCommand(PairsNumbersQuery, connection);
                                PairsNumbersCommand.Parameters.AddWithValue("@id", await GetChallengeUniqueID());
                                PairsNumbersCommand.Parameters.AddWithValue("@challengeType", (int)challengeData.TaskType);
                                PairsNumbersCommand.Parameters.AddWithValue("@mode", modeId);
                                PairsNumbersCommand.Parameters.AddWithValue("@duration", challengeData.Duration.TotalMilliseconds);
                                PairsNumbersCommand.Parameters.AddWithValue("@maxNumb", challengeData.MaxNumber);
                                PairsNumbersCommand.Parameters.AddWithValue("@correctRate", challengeData.CorrectRate);

                                await PairsNumbersCommand.ExecuteNonQueryAsync();
                                //Debug.LogError("PairsNumbers");
                                break;
                            }

                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("InsertChallengeTables error: " + e.ToString());
                }
            }
        }

        public async void UpdateData()
        {
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string UpdateModeQuery = "UPDATE TaskMode SET LastTaskModeIndex = @lastIndex " +
                    "WHERE strftime('%Y-%m-%d',Date) = @date AND ModeCode = @mode";

                    SqliteCommand UpdateModeCommand = new SqliteCommand(UpdateModeQuery, connection);
                    UpdateModeCommand.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                    UpdateModeCommand.Parameters.AddWithValue("@mode", (int)DataToSave.Mode);
                    //UpdateModeCommand.Parameters.AddWithValue("@lastIndex", DataToSave.TaskModeIndex);

                    await UpdateModeCommand.ExecuteNonQueryAsync();
                    
                    await InsertTaskTables();

                    await UpdateTaskStatisticTables(DataToSave.TaskType);

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some UpdateData error: " + e.ToString());
                }
            }
        }
        #endregion

        #region CALENDAR

        public async System.Threading.Tasks.Task<CalendarData> GetCalendarData(DateTime date)
        {
            CalendarData calendarData = new CalendarData(date);
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string GetDoneModesQuery = "SELECT ModeCode FROM TaskMode " +
                        "WHERE Date = @date AND IsModeDone = '1';";

                    SqliteCommand GetDoneModesCommand = new SqliteCommand(GetDoneModesQuery, connection);
                    GetDoneModesCommand.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

                    using (var GetModeIdReader = await GetDoneModesCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await GetModeIdReader.ReadAsync())
                        {
                            TaskMode mode = (TaskMode)Convert.ToInt32(GetModeIdReader[0]);
                            calendarData.ModeData.Add(mode, true);
                            //if (mode != TaskMode.Challenge)
                            //{
                            //    calendarData.ModeData.Add(mode, true);
                            //}
                        }
                    }

                    foreach (TaskMode mode in (TaskMode[])Enum.GetValues(typeof(TaskMode)))
                    {
                        if (!calendarData.ModeData.ContainsKey(mode))
                        {
                            calendarData.ModeData.Add(mode, false);
                        }
                    }
                }
                catch (Exception e)
                {
                    //Todo: process the error
                    Debug.Log("Some error: " + e.ToString());
                }
            }

            return calendarData;
        }

        public async System.Threading.Tasks.Task<List<CalendarData>> GetCalendarData(int month)
        {
            List<CalendarData> calendarDatas = new List<CalendarData>();
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    string monthData;
                    if (month <= 9)
                    {
                        monthData = "0" + month;
                    }
                    else
                    {
                        monthData = month.ToString();
                    }

                    connection.Open();

                    string GetDoneDateAndModesQuery = "SELECT Date, ModeCode FROM TaskMode " +
                        "WHERE strftime('%m',Date) = @month AND IsModeDone = '1';";

                    SqliteCommand GetDoneDateAndModesCommand = new SqliteCommand(GetDoneDateAndModesQuery, connection);
                    GetDoneDateAndModesCommand.Parameters.AddWithValue("@month", monthData);

                    using (var reader = await GetDoneDateAndModesCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            if (calendarDatas.Any(x => x.Date == (Convert.ToDateTime(reader[0])).Date))
                            {
                                calendarDatas.FirstOrDefault(x => x.Date == (Convert.ToDateTime(reader[0])).Date).ModeData.Add((TaskMode)Convert.ToInt32(reader[1]), true);
                            }
                            else
                            {
                                CalendarData data = new CalendarData((Convert.ToDateTime(reader[0])).Date);
                                data.ModeData.Add((TaskMode)Convert.ToInt32(reader[1]), true);
                                calendarDatas.Add(data);
                            }
                        }
                    }
                    //Small optimization: for instead of foreach coz its a bit faster
                    for (int i = 0; i < calendarDatas.Count; i++)
                    {
                        foreach (TaskMode mode in (TaskMode[])Enum.GetValues(typeof(TaskMode)))
                        {
                            if (!calendarDatas[i].ModeData.ContainsKey(mode))
                            {
                                calendarDatas[i].ModeData.Add(mode, false);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Todo: process the error
                    Debug.Log("Some error: " + e.ToString());
                }
            }
            return calendarDatas;
        }

        public async System.Threading.Tasks.Task<List<CalendarData>> GetCalendarData(int month, int year)
        {
            List<CalendarData> calendarDatas = new List<CalendarData>();
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    string monthData;
                    if (month <= 9)
                    {
                        monthData = "0" + month;
                    }
                    else
                    {
                        monthData = month.ToString();
                    }

                    connection.Open();

                    string GetDoneDateAndModesQuery = "SELECT Date, ModeCode FROM TaskMode " +
                        "WHERE strftime('%Y-%m',Date) = @yearMonth AND IsModeDone = '1';";

                    SqliteCommand GetDoneDateAndModesCommand = new SqliteCommand(GetDoneDateAndModesQuery, connection);
                    GetDoneDateAndModesCommand.Parameters.AddWithValue("@yearMonth", (year + "-" + monthData));

                    using (var reader = await GetDoneDateAndModesCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            if (calendarDatas.Any(x => x.Date == (Convert.ToDateTime(reader[0])).Date))
                            {
                                calendarDatas.FirstOrDefault(x => x.Date == (Convert.ToDateTime(reader[0])).Date).ModeData.Add((TaskMode)Convert.ToInt32(reader[1]), true);
                            }
                            else
                            {
                                CalendarData data = new CalendarData((Convert.ToDateTime(reader[0])).Date);
                                data.ModeData.Add((TaskMode)Convert.ToInt32(reader[1]), true);
                                calendarDatas.Add(data);
                            }
                        }
                    }

                    //Small optimization: for instead of foreach coz its a bit faster
                    for (int i = 0; i < calendarDatas.Count; i++)
                    {
                        foreach (TaskMode mode in (TaskMode[])Enum.GetValues(typeof(TaskMode)))
                        {
                            if (!calendarDatas[i].ModeData.ContainsKey(mode))
                            {
                                calendarDatas[i].ModeData.Add(mode, false);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Todo: process the error
                    Debug.Log("Some error: " + e.ToString());
                }
            }

            return calendarDatas;
        }

        public async System.Threading.Tasks.Task<double> GetCorrectRateOfMode(TaskMode mode)
        {
            double value = 0;
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    int correct = 0, tasksCount = 0;
                    List<int> modeIds = await GetAllModeIds(mode);
                    string idString = String.Join(",", modeIds);

                    connection.Open();

                    string GetCorrectQuery = "SELECT COUNT(*) FROM ( " +
                    " SELECT Id FROM Addition WHERE Mode = @mode AND IsUserAnswerCorrect = '1'" +
                    " UNION SELECT Id FROM Subtraction WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM AddSubMissingNumber WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM Comparison WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM ComparisonWithMissingNumber WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM ExpressionsComparison WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM IsThatTrue WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM ComparisonMissingElements WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM MissingExpression WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM MissingNumber WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM MissingSign WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM SelectFromThreeCount WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM CountTo10Images WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM CountTo10Frames WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM CountTo20Frames WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id FROM SumOfNumbers WHERE Mode = @mode AND IsUserAnswerCorrect = '1' );";

                    SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                    GetCorrectCommand.Parameters.AddWithValue("@mode", idString);

                    int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                    if (correctAnswCount.HasValue)
                    {
                        correct = correctAnswCount.Value;
                    }

                    string GetSummaryQuery = "SELECT COUNT(*) FROM ( " +
                    " SELECT Id FROM Addition WHERE Mode = @mode " +
                    " UNION SELECT Id FROM Subtraction WHERE Mode = @mode" +
                    " UNION SELECT Id FROM AddSubMissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id FROM Comparison WHERE Mode = @mode " +
                    " UNION SELECT Id FROM ComparisonWithMissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id FROM ExpressionsComparison WHERE Mode = @mode " +
                    " UNION SELECT Id FROM IsThatTrue WHERE Mode = @mode " +
                    " UNION SELECT Id FROM ComparisonMissingElements WHERE Mode = @mode " +
                    " UNION SELECT Id FROM MissingExpression WHERE Mode = @mode " +
                    " UNION SELECT Id FROM MissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id FROM MissingSign WHERE Mode = @mode " +
                    " UNION SELECT Id FROM SelectFromThreeCount WHERE Mode = @mode " +
                    " UNION SELECT Id FROM CountTo10Images WHERE Mode = @mode " +
                    " UNION SELECT Id FROM CountTo10Frames WHERE Mode = @mode " +
                    " UNION SELECT Id FROM CountTo20Frames WHERE Mode = @mode " +
                    " UNION SELECT Id FROM SumOfNumbers WHERE Mode = @mode );";

                    SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                    GetSummaryCommand.Parameters.AddWithValue("@mode", idString);

                    int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                    if (answCount.HasValue)
                    {
                        tasksCount = answCount.Value;
                    }

                    if (correct != 0)
                    {
                        value = (double)correct / (double)tasksCount;
                    }

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some error: " + e.ToString());
                }
            }
            return value;
        }

        public async System.Threading.Tasks.Task<double> GetCorrectRateOfTaskType(TaskType type)
        {
            double value = 0;
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    int correct = 0, tasksCount = 0;
                    connection.Open();

                    switch (type)
                    {
                        case TaskType.Addition:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM Addition WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM Addition";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }                                
                                break;
                            }
                        case TaskType.Subtraction:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM Subtraction WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM Subtraction";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                break;
                            }
                        case TaskType.AddSubMissingNumber:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM AddSubMissingNumber WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM AddSubMissingNumber";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                break;
                            }
                        case TaskType.Comparison:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM Comparison WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM Comparison";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                break;
                            }
                        case TaskType.ComparisonExpressions:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM ExpressionsComparison WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM ExpressionsComparison";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                        case TaskType.ComparisonWithMissingNumber:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM ComparisonWithMissingNumber WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM ComparisonWithMissingNumber";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                        case TaskType.ComparisonMissingElements:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM ComparisonMissingElements WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM ComparisonMissingElements";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                        case TaskType.MissingExpression:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM MissingExpression WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM MissingExpression";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                        case TaskType.MissingNumber:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM MissingNumber WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM MissingNumber";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                        case TaskType.IsThatTrue:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM IsThatTrue WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM IsThatTrue";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                        case TaskType.MissingSign:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM MissingSign WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM MissingSign";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                        case TaskType.ImageOpening:
                            {
                                string GetCorrectQuery = "SELECT CorrectRate FROM OpenImage";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                double? rate = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (rate.HasValue)
                                {
                                    return rate.Value;
                                }
                                
                                break;
                            }
                        case TaskType.PairsNumbers:
                            {
                                string GetCorrectQuery = "SELECT CorrectRate FROM PairsNumbers";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                double? rate = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (rate.HasValue)
                                {
                                    return rate.Value;
                                }
                                
                                break;
                            }
                        case TaskType.SumOfNumbers:
                            {
                                string GetCorrectQuery = "SELECT COUNT(*) FROM SumOfNumbers WHERE IsUserAnswerCorrect = '1'";
                                SqliteCommand GetCorrectCommand = new SqliteCommand(GetCorrectQuery, connection);
                                int? correctAnswCount = Convert.ToInt32(await GetCorrectCommand.ExecuteScalarAsync());
                                if (correctAnswCount.HasValue)
                                {
                                    correct = correctAnswCount.Value;
                                }

                                string GetSummaryQuery = "SELECT COUNT(*) FROM SumOfNumbers";
                                SqliteCommand GetSummaryCommand = new SqliteCommand(GetSummaryQuery, connection);
                                int? answCount = Convert.ToInt32(await GetSummaryCommand.ExecuteScalarAsync());
                                if (answCount.HasValue)
                                {
                                    tasksCount = answCount.Value;
                                }
                                
                                break;
                            }
                    }

                    if (correct != 0)
                    {
                        value = (double)correct / (double)tasksCount;
                    }

                    connection.Close();
                    return value;
                }
                catch (Exception e)
                {
                    //Todo: process the error
                    Debug.Log("Some error: " + e.ToString());
                }
            }
            return value;
        }

        #endregion

        #region LOADING

        //public async Task<List<GradeData>> GetGradeDatas(List<GradeSettings> gradeSettings)
        //{
        //    List<GradeData> gradeDatas = new List<GradeData>();

        //    using (SqliteConnection connection = new SqliteConnection(databasePath))
        //    {
        //        try 
        //        { 
        //            connection.Open();

        //            foreach (var gradeSetting in gradeSettings)
        //            {
        //                var gradeData = new GradeData();
        //                int gradeIndex = gradeSettings.IndexOf(gradeSetting) + 1;
        //                gradeData.GradeIndex = gradeIndex;
        //                gradeData.IsActive = true;
        //                gradeData.SkillDatas = new List<SkillData>();
        //                for (int i = 0; i < gradeSetting.SkillSettings.Count; i++)
        //                {
        //                    SkillData skillData = new SkillData();
        //                    skillData.SkillType = gradeSetting.SkillSettings[i].SkillType;
        //                    skillData.TaskSettings = gradeSetting.SkillSettings[i].TaskSettings;
        //                    int skillTypeIndex = (int)gradeSetting.SkillSettings[i].SkillType;

        //                    var selectCommand = new SqliteCommand("SELECT * FROM SkillSettings WHERE GradeIndex = @gradeIndex AND SkillTypeIndex = @skillTypeIndex", connection);
        //                    selectCommand.Parameters.AddWithValue("@gradeIndex", gradeIndex);
        //                    selectCommand.Parameters.AddWithValue("@skillTypeIndex", skillTypeIndex);

        //                    var reader = await selectCommand.ExecuteReaderAsync();
        //                    if (!reader.HasRows)
        //                    {
        //                        reader.Close();
        //                        var insertCommand = new SqliteCommand("INSERT INTO SkillSettings (GradeIndex, SkillTypeIndex, MaxNumber, IsActive) VALUES (@gradeIndex, @skillTypeIndex, @maxNumber, @isActive)", connection);
        //                        insertCommand.Parameters.AddWithValue("@gradeIndex", gradeIndex);
        //                        insertCommand.Parameters.AddWithValue("@skillTypeIndex", skillTypeIndex);
        //                        insertCommand.Parameters.AddWithValue("@maxNumber", 20);
        //                        insertCommand.Parameters.AddWithValue("@isActive", true);
        //                        await insertCommand.ExecuteNonQueryAsync();

        //                        reader = await selectCommand.ExecuteReaderAsync();
        //                    }
        //                    while (await reader.ReadAsync())
        //                    {
        //                        skillData.IsActive = (bool)reader["IsActive"];
        //                        skillData.MaxNumber = Convert.ToInt32(reader["MaxNumber"]);
        //                        gradeData.SkillDatas.Add(skillData);
        //                    }
        //                    reader.Close();
        //                }
        //                gradeDatas.Add(gradeData);
        //            }
        //            connection.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log("Some error: " + e.ToString());
        //        }
        //    }
        //    return gradeDatas;
        //}


        //Returns list of TimeSpans represents time of the task completion 
        //depending on a specific date and task mod
        public async Task<List<TimeSpan>> GetTimeSpansByModeAndDate(TaskMode mode, DateTime date)
        {
            List<TimeSpan> timeSpans = new List<TimeSpan>();
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    int modeId = await GetTaskModeId(mode, date);

                    string GetDurationQuery = "SELECT Duration FROM ( " +
                    " SELECT Id as ID, Duration FROM Addition WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM Subtraction WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM AddSubMissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM Comparison WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM ComparisonWithMissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM ExpressionsComparison WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM IsThatTrue WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM ComparisonMissingElements WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM MissingExpression WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM MissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM MissingSign WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM SumOfNumbers WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM CountTo10Images WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM SelectFromThreeCount WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM CountTo10Frames WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM CountTo20Frames WHERE Mode = @mode " +
                    " ) ORDER BY ID ASC;";

                    SqliteCommand GetDurationCommand = new SqliteCommand(GetDurationQuery, connection);
                    GetDurationCommand.Parameters.AddWithValue("@mode", modeId);

                    double? duration;
                    using (var reader = await GetDurationCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            duration = Convert.ToDouble(reader[0]);
                            if (duration.HasValue)
                            {
                                timeSpans.Add(TimeSpan.FromMilliseconds(duration.Value)); 
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Some GetTimeOfModeAndDate error: " + e.ToString());
                }
            }

            return timeSpans;
        }

        public async Task<List<string>> GetTaskResults(TaskMode mode, DateTime date)
        {
            List<string> results = new List<string>();
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();
                    int modeId = await GetTaskModeId(mode, date);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT Elements, Operators, Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, ID FROM ( ");
                    foreach (var tableName in taskTypeTables)
                        sb.Append($"SELECT Elements, Operators, Variants, SelectedAnswers, CorrectAnswers, IsUserAnswerCorrect, Id as ID FROM {tableName.Value} WHERE Mode = @mode UNION ");
                    sb.Length -= " UNION ".Length; // remove the last " UNION "
                    sb.Append(" ) ORDER BY ID ASC;");
                    string query = sb.ToString();

                    SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@mode", modeId);

                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            string elements = reader.GetString(0);
                            string operators = reader.GetString(1);
                            string variants = reader.GetString(2);
                            object selectedAnswersObject = reader.GetValue(3);
                            string selectedAnswers = selectedAnswersObject.ToString();
                            object correctAnswersObject = reader.GetValue(4);
                            string correctAnswers = correctAnswersObject.ToString();
                            bool isCorrect = Convert.ToBoolean(reader.GetInt32(5));
                            int id = reader.GetInt32(6);
                            string[] elementList = elements.Split(',');
                            string[] operatorList = operators.Split(',');
                            string[] variantsList = variants.Split(',');
                            string[] selectedAnswersList = selectedAnswers.Split(',');
                            string[] correctAnswersList = correctAnswers.Split(',');

                            for (int i = 0; i < variantsList.Length; i++)
                            {
                                if (operatorChars.ContainsKey(variantsList[i]))
                                    variantsList[i] = operatorChars[variantsList[i]];
                            }

                            int variantIndex = 0;
                            for (int i = 0; i < elementList.Length; i++)
                            {
                                if (elementList[i] == kUnknownElementValue)
                                {
                                    if (variantIndex >= selectedAnswersList.Length)
                                    {
                                        elementList[i] = "?";
                                    }
                                    else
                                    {
                                        int index = Int32.Parse(selectedAnswersList[variantIndex]);
                                        elementList[i] = $"<color={(isCorrect ? correctResultColor : wrongResultColor)}>" +
                                                        $"{variantsList[index]}</color>";
                                        variantIndex++;
                                    }
                                }
                            }

                            var coloredOperatorList = operatorList.Select(o =>
                            {
                                if (o == kUnknownElementValue)
                                {
                                    return $"<color={(isCorrect ? correctResultColor : wrongResultColor)}>" +
                                    $"{variantsList[Int32.Parse(selectedAnswersList[0])]}</color>";
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
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("GetTaskResult error: " + e.ToString());
                }
            }
            return results;
        }

        public async Task<List<bool>> GetAnswers(TaskMode mode, DateTime date)
        {
            List<bool> answers = new List<bool>();
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();
                    int modeId = await GetTaskModeId(mode, date);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT Correct FROM ( ");
                    foreach (var tableName in taskTypeTables)
                        sb.Append($"SELECT Id as ID, IsUserAnswerCorrect as Correct FROM {tableName.Value} WHERE Mode = @mode UNION ");
                    sb.Length -= " UNION ".Length; // remove the last " UNION "
                    sb.Append(" ) ORDER BY ID ASC;");
                    string query = sb.ToString();

                    SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue("@mode", modeId);

                    int? temp = null;
                    using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            temp = Convert.ToInt32(reader[0]);
                            if (temp.HasValue)
                            {
                                answers.Add(Convert.ToBoolean(temp));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Some UpdateData error: " + e.ToString());
                }
            }
            return answers;
        }

        public async System.Threading.Tasks.Task<int> GetCorrectAnswersOfModeByDate(TaskMode mode, DateTime date)
        {
            int correctAnswersCount = 0;

            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    int modeId = await GetTaskModeId(mode, date);

                    string GetCorrectTaskAmountQuery = "SELECT COUNT(*) FROM ( " +
                    " SELECT Id as ID, IsUserAnswerCorrect as Correct FROM Addition WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM Subtraction WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM AddSubMissingNumber WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM Comparison WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM ComparisonWithMissingNumber WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM ExpressionsComparison WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM IsThatTrue WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM ComparisonMissingElements WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM MissingExpression WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM MissingNumber WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM MissingSign WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM SumOfNumbers WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM CountTo10Images WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM SelectFromThreeCount WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM CountTo10Frames WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " UNION SELECT Id as ID, IsUserAnswerCorrect as Correct FROM CountTo20Frames WHERE Mode = @mode AND IsUserAnswerCorrect = '1' " +
                    " ) ORDER BY ID ASC;";

                    SqliteCommand GetCorrectTaskAmountCommand = new SqliteCommand(GetCorrectTaskAmountQuery, connection);
                    GetCorrectTaskAmountCommand.Parameters.AddWithValue("@mode", modeId);

                    //Do I need to do null check here
                    correctAnswersCount = Convert.ToInt32(await GetCorrectTaskAmountCommand.ExecuteScalarAsync());

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Some UpdateData error: " + e.ToString());
                }
            }
            return correctAnswersCount;
        }


        //return value in milliseconds
        public async System.Threading.Tasks.Task<long> GetTimeOfModeAndDate(TaskMode mode, DateTime date)
        {
            long milliseconds = 0;

            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    int modeId = await GetTaskModeId(mode, date);

                    //Переделать все квери для того что б был запрос 

                    string GetDurationQuery = "SELECT Duration FROM ( " +
                    " SELECT Id as ID, Duration FROM Addition WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM Subtraction WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM AddSubMissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM Comparison WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM ComparisonWithMissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM ExpressionsComparison WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM IsThatTrue WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM ComparisonMissingElements WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM MissingExpression WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM MissingNumber WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM MissingSign WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM SumOfNumbers WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM CountTo10Images WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM SelectFromThreeCount WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM CountTo10Frames WHERE Mode = @mode " +
                    " UNION SELECT Id as ID, Duration FROM CountTo20Frames WHERE Mode = @mode " +
                    " ) ORDER BY ID ASC;";

                    SqliteCommand GetDurationCommand = new SqliteCommand(GetDurationQuery, connection);
                    GetDurationCommand.Parameters.AddWithValue("@mode", modeId);

                    double? duration;
                    using (var reader = await GetDurationCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            duration = Convert.ToDouble(reader[0]);
                            if (duration.HasValue)
                            {
                                milliseconds += Convert.ToInt32(duration.Value);
                            }
                            
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Some GetTimeOfModeAndDate error: " + e.ToString());
                }
            }
            return milliseconds;
        }

        //Return amount of tasks that was done, if no tasks was done return 0
        public async Task<int> TodayDoneTasksAmount(TaskMode mode)
        {
            int value = 0;
            using (SqliteConnection connection = new SqliteConnection(databasePath))
            {
                try
                {
                    connection.Open();

                    string GetModeIdQuery = "SELECT LastTaskModeIndex FROM TaskMode " +
                        "WHERE strftime('%Y-%m-%d', Date) = @date AND ModeCode = @mode LIMIT 1";

                    SqliteCommand GetModeIdCommand = new SqliteCommand(GetModeIdQuery, connection);
                    GetModeIdCommand.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                    GetModeIdCommand.Parameters.AddWithValue("@mode", (int)mode);

                    int? temp = null;
                    using (var reader = await GetModeIdCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            temp = Convert.ToInt32(reader[0]);
                        }
                    }

                    if (!temp.HasValue)
                    {
                        value = 0;
                    }
                    else
                    {
                        value = temp.Value;
                    }

                    connection.Close();
                }
                catch (Exception e)
                {
                    Debug.Log("Some TodayDoneTasksAmount error: " + e.ToString());
                }
            }
            return value;
        }

        #endregion

        #region RESET

        public async UniTask ResetSaveFile()
        {
            if (Directory.Exists(saveDirectoryPath))
            {
                // Delete the existing database file
                if (File.Exists(saveFilePath))
                {
                    try
                    {
                        // Close all existing connections to the old database file
                        Debug.Log("Trying to Reset Save File");
                        SqliteConnection.ClearAllPools();
                        File.Delete(saveFilePath);
                        while (File.Exists(saveFilePath))
                        {
                            await UniTask.Delay(100);
                        }
                        this.CreateDataTables();
                        //await UniTask.Delay(100);
                        //// Update the last saved database version to the actual version
                        //UpdateSystemInfo(actualDatabaseVersion, Application.version);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("ResetSaveFile error: " + e.ToString());
                    }
                }
            }
            
        }

        #endregion
    }
}