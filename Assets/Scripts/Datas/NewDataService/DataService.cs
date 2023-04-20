using UnityEngine;
using System.IO;


namespace Mathy.Services
{
    public interface IDataService
    { 
        ITaskDataHandler TaskData { get; }
    }


    public class DataService : IDataService
    {
        private readonly string dataPath = Application.persistentDataPath;
        private TaskDataHandler _taskData;
        private string databasePath;

        public ITaskDataHandler TaskData => _taskData;


        public DataService()
        {
            databasePath = dataPath + "/Saves/";
            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
            }

            _taskData = new TaskDataHandler(databasePath);
            InitProviders();
        }

        private async void InitProviders()
        {
            await _taskData.Init();
        }
    }
}

