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


        private GeneralStatisticHandler _statisticHandler;
        private TaskDataHandler _taskDataHandler;
        private SkillPlanHandler _skillPlanHandler;
        private KeyValuePairDataHandler _keyValuehandler;
        private string _saveDirectoryPath;
        private string _taskDBFilePath;

        public string DatabasePath => _taskDBFilePath;

        public ITaskDataHandler TaskData => _taskDataHandler;
        public ISkillPlanHandler SkillPlan => _skillPlanHandler;
        public IKeyValuePairDataHandler KeyValueStorage => _keyValuehandler;
        public IGeneralStatisticHandler GeneralStatistic => _statisticHandler;


        public DataService()
        {
            string dataPath = Application.persistentDataPath;
            _saveDirectoryPath = dataPath + "/Saves/";
            var saveFilePath = _saveDirectoryPath + kFileName;
            _taskDBFilePath = $"Data Source={saveFilePath}";
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
        }

        private async UniTask InitHandlersAsync()
        {
            await _taskDataHandler.Init();
            await _skillPlanHandler.Init();
            await _keyValuehandler.Init();
            await _statisticHandler.Init();
        }
    }
}

