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
        private readonly string dataPath = Application.persistentDataPath;

        private TaskDataHandler _taskDataHandler;
        private SkillPlanHandler _skillPlanHandler;
        private string databasePath;

        public ITaskDataHandler TaskData => _taskDataHandler;
        public ISkillPlanHandler SkillPlan => _skillPlanHandler;


        public DataService()
        {
            databasePath = dataPath + "/Saves/";
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
            }

            _taskDataHandler = new TaskDataHandler(databasePath);
            _skillPlanHandler = new SkillPlanHandler(databasePath);
            InitProviders();
        }

        private async void InitProviders()
        {
            await _taskDataHandler.Init();
            await _skillPlanHandler.Init();
        }
    }
}

