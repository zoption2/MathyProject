using UnityEngine;
using System.IO;
using Mathy.Services.Data;
using System;
using Cysharp.Threading.Tasks;
using Dapper;

namespace Mathy.Services
{
    public interface IDataService
    {
        event Action ON_RESET;
        ITaskDataHandler TaskData { get; }
        ISkillPlanHandler SkillPlan { get; }
        UniTask ResetProgress();
    }


    public class DataService : IDataService
    {
        public event Action ON_RESET;

        private const string kFileName = "save.db";


        private TaskDataHandler _taskDataHandler;
        private SkillPlanHandler _skillPlanHandler;
        private KeyValuePairDataHandler _keyValuehandler;
        private string _saveDirectoryPath;
        private string _taskDBFilePath;

        public string DatabasePath => _taskDBFilePath;

        public ITaskDataHandler TaskData => _taskDataHandler;
        public ISkillPlanHandler SkillPlan => _skillPlanHandler;
        public IKeyValuePairDataHandler KeyValueHandler => _keyValuehandler;


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
            InitHandlers();
        }

        public async UniTask ResetProgress()
        {
            await _taskDataHandler.ClearData();
            await _skillPlanHandler.ClearData();

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
        }
    }
}

