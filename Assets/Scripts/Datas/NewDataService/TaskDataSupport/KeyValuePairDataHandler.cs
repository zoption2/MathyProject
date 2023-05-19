using Cysharp.Threading.Tasks;
using System;


namespace Mathy.Services.Data
{
    public interface IKeyValuePairDataHandler
    {
        UniTask<KeyValueIntegerData> GetIntegerDataByKeyAsync(string key, int defaultValue = 0);
        UniTask<int> GetIntValueAsync(string key, int defaultValue = 0);
        UniTask<int> GetIntValueAsync(KeyValueIntegerKeys keyType, int defaultValue = 0);
        UniTask SaveIntValueAsync(string key, int value);
        UniTask SaveIntValueAsync(KeyValueIntegerKeys keyType, int value);
        UniTask IncrementIntValueAsync(string key);
        UniTask IncrementIntValueAsync(KeyValueIntegerKeys keyType);
        void SaveIntValue(string key, int value);
        int GetIntValue(string key, int defaultValue = 0);

        UniTask<KeyValueStringData> GetStringDataByKeyAsync(string key, string defaultValue = "");
        UniTask<string> GetStringOrDefaultAsync(string key, string defaultValue = "");
        UniTask SaveStringValueAsync(string key, string value);
    }


    public class KeyValuePairDataHandler : IKeyValuePairDataHandler
    {
        private IKeyValuePairIntegerProvider _intProvider;
        private IKeyValuePairStringProvider _stringProvider;
        private readonly DataService _dataService;

        public KeyValuePairDataHandler(DataService dataService)
        {
            _dataService = dataService;
            var filePath = dataService.DatabasePath;

            _intProvider = new KeyValuePairIntegerProvider(filePath);
            _stringProvider = new KeyValuePairStringProvider(filePath);
        }

        public async UniTask<KeyValueIntegerData> GetIntegerDataByKeyAsync(string key, int defaultValue = 0)
        {
            return await _intProvider.GetDataByKey(key, defaultValue);
        }

        public async UniTask<int> GetIntValueAsync(string key, int defaultValue = 0)
        {
            return await _intProvider.GetIntOrDefaultByKey(key, defaultValue);
        }

        public async UniTask<int> GetIntValueAsync(KeyValueIntegerKeys keyType, int defaultValue = 0)
        {
            var key = keyType.ToString();
            return await _intProvider.GetIntOrDefaultByKey(key, defaultValue);
        }

        public async UniTask SaveIntValueAsync(string key, int value)
        {
            var currentDateTime = DateTime.UtcNow;
            await _intProvider.SetValue(key, value, currentDateTime);
        }

        public async UniTask SaveIntValueAsync(KeyValueIntegerKeys keyType, int value)
        {
            var key = keyType.ToString();
            var currentDateTime = DateTime.UtcNow;
            await _intProvider.SetValue(key, value, currentDateTime);
        }

        public async UniTask IncrementIntValueAsync(string key)
        {
            var currentDateTime = DateTime.UtcNow;
            await _intProvider.IncrementValue(key, currentDateTime);
        }

        public async UniTask IncrementIntValueAsync(KeyValueIntegerKeys keyType)
        {
            var key = keyType.ToString();
            await IncrementIntValueAsync(key);
        }

        public async UniTask<KeyValueStringData> GetStringDataByKeyAsync(string key, string defaultValue = "")
        {
            return await _stringProvider.GetDataByKey(key, defaultValue);
        }

        public async UniTask<string> GetStringOrDefaultAsync(string key, string defaultValue = "")
        {
            return await _stringProvider.GetStringOrDefaultByKey(key, defaultValue);
        }

        public async UniTask SaveStringValueAsync(string key, string value)
        {
            var date = DateTime.UtcNow;
            await _stringProvider.SetValue(key, value, date);
        }

        public void SaveIntValue(string key, int value)
        {
            var date = DateTime.UtcNow;
            _intProvider.SaveIntValue(key, value, date);
        }

        public int GetIntValue(string key, int defaultValue = 0)
        {
             return _intProvider.GetIntValue(key, defaultValue);
        }

        public async UniTask Init()
        {
            await TryCreateTables();
        }

        public async UniTask ClearData()
        {
            await _intProvider.DeleteTable();
            await _stringProvider.DeleteTable();
        }

        protected async UniTask TryCreateTables()
        {
            await _intProvider.TryCreateTable();
            await _stringProvider.TryCreateTable();
        }
    }
}

