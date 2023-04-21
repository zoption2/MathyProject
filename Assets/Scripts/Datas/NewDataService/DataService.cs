using UnityEngine;
using System.IO;
using Mathy.Services.Data;


namespace Mathy.Services
{
    public interface IDataService
    { 
        ITaskDataHandler TaskData { get; }
        ISkillPlanHandler SkillPlan { get; }
    }


    public class DataService : IDataService
    {
        private const string kFileName = "save.db";
        private readonly string dataPath = Application.persistentDataPath;

        private TaskDataHandler _taskDataHandler;
        private SkillPlanHandler _skillPlanHandler;
        private string _saveDirectoryPath;
        private string _taskDBFilePath;

        public ITaskDataHandler TaskData => _taskDataHandler;
        public ISkillPlanHandler SkillPlan => _skillPlanHandler;


        public DataService()
        {
            _saveDirectoryPath = dataPath + "/Saves/";
            var saveFilePath = _saveDirectoryPath + kFileName;
            _taskDBFilePath = $"Data Source={saveFilePath}";
            if (!Directory.Exists(_saveDirectoryPath))
            {
                Directory.CreateDirectory(_saveDirectoryPath);
            }

            _taskDataHandler = new TaskDataHandler(_taskDBFilePath);
            _skillPlanHandler = new SkillPlanHandler(_taskDBFilePath);
            InitProviders();
        }

        private async void InitProviders()
        {
            await _taskDataHandler.Init();
        }
    }
}

