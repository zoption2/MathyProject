using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IProgressHandler
    {
        UniTask<int> GetPlayerExperience();
        UniTask AddExperience(int addedValue);
        UniTask SetExpirience(int totalValue);
    }


    public class ProgressHandler : IProgressHandler
    {
        private readonly IDataService _dataService;

        public ProgressHandler(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async UniTask<int> GetPlayerExperience()
        {
            return await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.Experience);
        }

        public async UniTask AddExperience(int addedValue)
        {
            var current = await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.Experience);
            current += addedValue;
            await _dataService.KeyValueStorage.SaveIntValue(KeyValueIntegerKeys.Experience, current);
        }

        public async UniTask SetExpirience(int totalValue)
        {
            await _dataService.KeyValueStorage.SaveIntValue(KeyValueIntegerKeys.Experience, totalValue);
        }
    }

}


