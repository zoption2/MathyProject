using Cysharp.Threading.Tasks;
using System;


namespace Mathy.Services.Data
{
    public interface IKeyValuePairDataHandler
    {
        UniTask<KeyValueIntegerData> GetKeyValueIntegerData(string key, int defaultValue = 0);
        UniTask<int> GetIntValue(string key, int defaultValue = 0);
        UniTask<int> GetIntValue(KeyValueIntegerKeys keyType, int defaultValue = 0);
        UniTask SaveIntValue(string key, int value);
        UniTask SaveIntValue(KeyValueIntegerKeys keyType, int value);
        UniTask IncrementIntValue(string key);
    }


    public class KeyValuePairDataHandler : IKeyValuePairDataHandler
    {
        private IKeyValuePairIntegerProvider _intProvider;
        private readonly DataService _dataService;

        public KeyValuePairDataHandler(DataService dataService)
        {
            _dataService = dataService;
            var filePath = dataService.DatabasePath;

            _intProvider = new KeyValuePairIntegerProvider(filePath);
        }

        public async UniTask<KeyValueIntegerData> GetKeyValueIntegerData(string key, int defaultValue = 0)
        {
            return await _intProvider.GetDataByKey(key, defaultValue);
        }

        public async UniTask<int> GetIntValue(string key, int defaultValue = 0)
        {
            return await _intProvider.GetIntOrDefaultByKey(key, defaultValue);
        }

        public async UniTask<int> GetIntValue(KeyValueIntegerKeys keyType, int defaultValue = 0)
        {
            var key = keyType.ToString();
            return await _intProvider.GetIntOrDefaultByKey(key, defaultValue);
        }

        public async UniTask SaveIntValue(string key, int value)
        {
            var currentDateTime = DateTime.UtcNow;
            await _intProvider.SetValue(key, value, currentDateTime);
        }

        public async UniTask SaveIntValue(KeyValueIntegerKeys keyType, int value)
        {
            var key = keyType.ToString();
            var currentDateTime = DateTime.UtcNow;
            await _intProvider.SetValue(key, value, currentDateTime);
        }

        public async UniTask IncrementIntValue(string key)
        {
            var currentDateTime = DateTime.UtcNow;
            await _intProvider.IncrementValue(key, currentDateTime);
        }

        public async UniTask Init()
        {
            await TryCreateTables();
        }

        public async UniTask ClearData()
        {
            await _intProvider.DeleteTable();
        }

        protected async UniTask TryCreateTables()
        {
            await _intProvider.TryCreateTable();
        }
    }
}

