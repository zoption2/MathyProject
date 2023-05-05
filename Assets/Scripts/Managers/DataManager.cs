using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.Core.Tasks;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;
using System.Linq;
using System.IO;
using System;
using System.Threading.Tasks;

namespace Mathy.Data
{
    public class DataManager : StaticInstance<DataManager>
    {
        #region FIELDS

        private const string statusKey = "isChallengeCompleted";
        private const string lastTimeChallenge = "LastTimeChallenge";
        private const string todayAwardKey = "WasTodayAwardGot";
        private const string lastTimeAwarded = "LastTimeAwarded";
        private List<string> bestScoreKeys = new List<string>{
            "AdditionSubtraction10",
            "AdditionSubtraction20",
            "Comparison10",
            "Comparison20",
            "MissingNumber20" };


        //private string saveDirectoryPath;
        //private string saveFilePath;
        /// <summary>
        /// Folder path that contains the database save file
        /// </summary>
        public string DatabasePath { get; private set; }
        
        private List<ISaveable> Savables = new List<ISaveable>();

        public DatabaseHandler DbHandler;

        //!! move to the DatabseHandler
        //Check if we can manipulate the save file (delete it as exemple)
        //Does't working (but it has worked), need to fix it later
        private bool isSaveFileLocked
        {
            get
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(DatabasePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException)
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    //or does not exist (has already been processed)
                    return true;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
                //file is not locked
                return false;
            }
        }


        #endregion

        #region MONO AND INITIALIZATION

        public void Subscribe(ISaveable savable)
        {
            Savables.Add(savable);
            savable.OnSaveEvent += GetData;
        }
        public void Unsubscribe(ISaveable savable)
        {
            Savables.Remove(savable);
            savable.OnSaveEvent -= GetData;
        }

        protected override void Awake()
        {
            base.Awake();

            Initialize();

            CheckLastTimeChallenge();
            CheckLastTimeAwarded();

            //Debug.LogError("HERE!!");
            //PlayerPrefs.DeleteAll();
        }

        private async void Initialize()
        {
            this.DbHandler = DatabaseHandler.Instance;
        }
        #endregion

        #region SAVE 

        //Getting data from sender to save it later
        private void GetData(object sender, EventArgs args)
        {
            switch (sender)
            {
                case TaskManager t:
                    {
                        SaveEventArgs saveEvent = (SaveEventArgs)args;
                        if (saveEvent != null)
                        {
                            DbHandler.DataToSave = saveEvent.TaskData;
                        }
                        break;
                    }
                default:
                    Debug.LogError("Somethin went wrong");
                    break;
            }
        }

        //public async void SaveTaskData(TaskData data)
        //{
        //    DbHandler.DataToSave = data;
        //    if (await DbHandler.IsTodayModeExist(DbHandler.DataToSave.Mode))
        //    {
        //        if (!await DbHandler.IsTodayModeCompleted(DbHandler.DataToSave.Mode))
        //        {
        //            await System.Threading.Tasks.Task.Run(() => DbHandler.UpdateData());
        //        }
        //    }
        //    else
        //    {
        //        await System.Threading.Tasks.Task.Run(() => DbHandler.SaveTaskData());
        //    }
        //}

        /// <summary>
        /// Ping changes and saving data to the database  
        /// </summary>
        //public async void Save()
        //{
        //    //Getting all data from all Savables
        //    foreach (var savable in Savables)
        //    {
        //        savable.Save();
        //    }

        //    if (await DbHandler.IsTodayModeExist(DbHandler.DataToSave.Mode))
        //    {
        //        if (!await DbHandler.IsTodayModeCompleted(DbHandler.DataToSave.Mode))
        //        {
        //            await System.Threading.Tasks.Task.Run(() => DbHandler.UpdateData());
        //        }
        //    }
        //    else
        //    {
        //        await System.Threading.Tasks.Task.Run(() => DbHandler.SaveTaskData());
        //    }
        //}

        public async void SaveChallenge(ChallengeData data)
        {
            await this.DbHandler.SaveChallengeData(data);
        }

        #endregion

        #region SAVE GRADE SETTINGS

        //public async System.Threading.Tasks.Task SaveGradeDatas(List<GradeData> gradeDatas)
        //{
        //    await DbHandler.SaveGradeDatas(gradeDatas);
        //}

        #endregion

        #region LOAD GRADE SETTINGS

        //public async Task<List<GradeData>> GetGradeDatas(List<GradeSettings> gradeSettings)
        //{
        //    return await DbHandler.GetGradeDatas(gradeSettings);
        //}

        #endregion

        //public async Task<bool> IsTodayModeCompleted(TaskMode mode)
        //{
        //    return await DbHandler.IsTodayModeCompleted(mode);
        //}

        //public async Task<bool> IsDateModeCompleted(TaskMode mode, DateTime date)
        //{
        //    return await DbHandler.IsDateModeCompleted(mode, date);
        //}

        public async Task<bool> IsTodayModeExist(TaskMode mode)
        {
            return await DbHandler.IsTodayModeExist(mode);
        }

        //public async Task<int> GetLastTaskIndexOfMode(TaskMode mode)
        //{ 
        //    return (await DbHandler.TodayDoneTasksAmount(mode));
        //}

        #region CALENDAR

        public async System.Threading.Tasks.Task<CalendarData> GetCalendarData(DateTime date)
        {
            return await DbHandler.GetCalendarData(date);
        }

        public async System.Threading.Tasks.Task<List<CalendarData>> GetCalendarData(int month)
        {
            return await DbHandler.GetCalendarData(month);
        }
        
        public async System.Threading.Tasks.Task<List<CalendarData>> GetCalendarData(int month, int year)
        {
            return await DbHandler.GetCalendarData(month,year);
        }

        #endregion

        #region STATISTICS

        //public async System.Threading.Tasks.Task<int> GetCorrectAnswersOfModeByDate(TaskMode mode, DateTime date)
        //{
        //    return await DbHandler.GetCorrectAnswersOfModeByDate(mode, date);
        //}

        //public async System.Threading.Tasks.Task<double> GetCorrectRateOfMode(TaskMode mode)
        //{
        //    return await DbHandler.GetCorrectRateOfMode(mode);
        //}

        //public async System.Threading.Tasks.Task<double> GetCorrectRateOfTaskType(TaskType type)
        //{
        //    return await DbHandler.GetCorrectRateOfTaskType(type);
        //}

        //public async Task<List<string>> GetTaskResults(TaskMode mode, DateTime date)
        //{
        //    return await this.DbHandler.GetTaskResults(mode, date);
        //}
        //public async Task<List<bool>> GetAnswers(TaskMode mode, DateTime date)
        //{
        //    return await this.DbHandler.GetAnswers(mode, date);
        //}

        //public async Task<List<TimeSpan>> GetTimeSpansByModeAndDate(TaskMode mode, DateTime date)
        //{
        //    return await this.DbHandler.GetTimeSpansByModeAndDate(mode, date);
        //}

        //public async Task<List<bool>> GetTodayAnswers(TaskMode mode)
        //{
        //    return await this.DbHandler.GetAnswers(mode, DateTime.UtcNow);
        //}

        //public async Task<int> TodayDoneTasksAmount(TaskMode mode)
        //{
        //    return await this.DbHandler.TodayDoneTasksAmount(mode);
        //}

        //return value in milliseconds
        //public async System.Threading.Tasks.Task<long> GetTimeOfModeAndDate(TaskMode mode, DateTime date)
        //{
        //    return await DbHandler.GetTimeOfModeAndDate(mode, date);
        //}

        #endregion

        #region DAILY CHALLENGE STATUS

        public async System.Threading.Tasks.Task<bool> TodayChallengeStatus()
        {
            /*
            set
            {
                PlayerPrefs.SetInt(statusKey, value.ToInt());
                if (TodayChallengeStatus == true) PlayerPrefs.SetString(lastTimeChallenge, DateTime.Now.Date.ToString());
            }
            */
            /*
            get
            {
                //return PlayerPrefs.GetInt(statusKey, 0).ToBool();
                return await IsTodayModeExist(TaskMode.Challenge);
            }*/
            return await IsTodayModeExist(TaskMode.Challenge);
        }

        //useless
        private void CheckLastTimeChallenge()
        {
            /*
            if (IsDayPassed())
            {
                TodayChallengeStatus = false;
            }
            */
        }

        public bool IsDayPassed()
        {
            if (GetLastTimeChallenge() == "")
                return true;

            bool isPassed = DateTime.Now.Date != DateTime.Parse(GetLastTimeChallenge());
            return isPassed;
        }

        private string GetLastTimeChallenge()
        {
            if (PlayerPrefs.HasKey(lastTimeChallenge))
                return PlayerPrefs.GetString(lastTimeChallenge);
            return "";
        }

        #endregion

        #region DAILY AWARD STATUS

        public bool WasTodayAwardGot
        {
            set
            {
                PlayerPrefs.SetInt(todayAwardKey, value.ToInt());
                if (WasTodayAwardGot == true) PlayerPrefs.SetString(lastTimeAwarded, DateTime.Now.Date.ToString());
            }
            get
            {
                return PlayerPrefs.GetInt(todayAwardKey, 0).ToBool();
            }
        }

        private void CheckLastTimeAwarded()
        {
            if (IsDayAfterAwardPassed())
            {
                WasTodayAwardGot = false;
            }
        }

        public bool IsDayAfterAwardPassed()
        {
            if (GetLastTimeAwarded() == "")
                return true;

            bool isPassed = DateTime.Now.Date != DateTime.Parse(GetLastTimeAwarded());
            return isPassed;
        }

        private string GetLastTimeAwarded()
        {
            if (PlayerPrefs.HasKey(lastTimeAwarded))
                return PlayerPrefs.GetString(lastTimeAwarded);
            return "";
        }

        #endregion

        #region RESET
        //Reset need to be in the DatabaseHandler, нужно переименовать тут 
        //public async UniTask ResetSaveFile()
        //{
        //    await DbHandler.ResetSaveFile();
        //    PlayerPrefs.DeleteAll();
        //}

        //public void ResetAllBestScores()
        //{
        //    for (int i = 0; i < bestScoreKeys.Count; i++)
        //    {
        //        for (int difficultyIndex = 0; difficultyIndex < 4; difficultyIndex++)
        //        {
        //            string bestScoreKey = bestScoreKeys[i] + "BestScore" + difficultyIndex;
        //            if (PlayerPrefs.HasKey(bestScoreKey))
        //            {
        //                PlayerPrefs.DeleteKey(bestScoreKey);
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}