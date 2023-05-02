using Cysharp.Threading.Tasks;
using System;


namespace Mathy.Services.Data
{
    public interface IKeyValuePairDataHandler
    {
        UniTask<KeyValueIntegerData> GetKeyValueIntegerData(KeyValuePairKeys key, int defaultValue = 0);
        UniTask<int> GetIntValue(KeyValuePairKeys key, int defaultValue = 0);
        UniTask SaveIntValue(KeyValuePairKeys key, int value);
        UniTask IncrementIntValue(KeyValuePairKeys key);
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

        public async UniTask<KeyValueIntegerData> GetKeyValueIntegerData(KeyValuePairKeys key, int defaultValue = 0)
        {
            return await _intProvider.GetDataByKey(key, defaultValue);
        }

        public async UniTask<int> GetIntValue(KeyValuePairKeys key, int defaultValue = 0)
        {
            return await _intProvider.GetIntOrDefaultByKey(key, defaultValue);
        }

        public async UniTask SaveIntValue(KeyValuePairKeys key, int value)
        {
            var currentDateTime = DateTime.UtcNow;
            await _intProvider.SetValue(key, value, currentDateTime);
        }

        public async UniTask IncrementIntValue(KeyValuePairKeys key)
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

