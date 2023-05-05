using UnityEngine;
using System.IO;
using Mathy.Services.Data;
using System;
using Cysharp.Threading.Tasks;


namespace Mathy.Services
{
    public interface IDataService
    {
        event Action ON_RESET;
        ITaskDataHandler TaskData { get; }
        ISkillPlanHandler SkillPlan { get; }
        IKeyValuePairDataHandler KeyValueStorage { get; }
        IGeneralStatisticHandler GeneralStatistic { get; }
        UniTask ResetProgress();
    }


    public class DataService : IDataService
    {
        public event Action ON_RESET;

        private const string kFileName = "save.db";

        //WARNING! Do not change if not nesassery!
        //using for for DB reset if versions are different 
        private const int kDatabaseControl = 1;


        private GeneralStatisticHandler _statisticHandler;
        private TaskDataHandler _taskDataHandler;
        private SkillPlanHandler _skillPlanHandler;
        private KeyValuePairDataHandler _keyValuehandler;
        private string _saveDirectoryPath;
        private string _taskDBFilePath;
        private string _saveFilePath;
        private bool _isFirstCreation;

        public string DatabasePath => _taskDBFilePath;

        public ITaskDataHandler TaskData => _taskDataHandler;
        public ISkillPlanHandler SkillPlan => _skillPlanHandler;
        public IKeyValuePairDataHandler KeyValueStorage => _keyValuehandler;
        public IGeneralStatisticHandler GeneralStatistic => _statisticHandler;


        public DataService()
        {
            string dataPath = Application.persistentDataPath;
            _saveDirectoryPath = dataPath + "/Saves/";
            _saveFilePath = _saveDirectoryPath + kFileName;
            _taskDBFilePath = $"Data Source={_saveFilePath}";
            if (!Directory.Exists(_saveDirectoryPath))
            {
                Directory.CreateDirectory(_saveDirectoryPath);
            }

            _taskDataHandler = new TaskDataHandler(this);
            _skillPlanHandler = new SkillPlanHandler(this);
            _keyValuehandler = new KeyValuePairDataHandler(this);
            _statisticHandler = new GeneralStatisticHandler(this);
            InitHandlers();
        }

        public async UniTask ResetProgress()
        {
            await _taskDataHandler.ClearData();
            await _skillPlanHandler.ClearData();
            await _statisticHandler.ClearData();
            await _keyValuehandler.ClearData();

            await InitHandlersAsync();

            ON_RESET?.Invoke();
            Debug.Log("Data reseted");
        }

        private async void InitHandlers()
        {
            await InitHandlersAsync();
            //var needToClear = await TryToClearDatabase(kDatabaseControl);
            //if (!needToClear)
            //{
            //    await InitHandlersAsync();
            //}
        }

        private async UniTask InitHandlersAsync()
        {
            await _taskDataHandler.Init();
            await _skillPlanHandler.Init();
            await _keyValuehandler.Init();
            await _statisticHandler.Init();

            //if(_isFirstCreation)
            //{
            //    await _keyValuehandler.SaveIntValue(KeyValueIntegerKeys.DatabaseControl, kDatabaseControl);
            //}
        }

        //private async UniTask<bool> TryToClearDatabase(int controlVersion)
        //{
        //    if (!File.Exists(_saveFilePath))
        //    {
        //        _isFirstCreation = true;
        //        return false;
        //    }

        //    var existingVersion = await _keyValuehandler.GetIntValue(KeyValueIntegerKeys.DatabaseControl);
        //    if (controlVersion != existingVersion)
        //    {
        //        await ResetProgress();
        //        await _keyValuehandler.SaveIntValue(KeyValueIntegerKeys.DatabaseControl, kDatabaseControl);
        //        return true;
        //    }
        //    return false;
        //}
    }
}

