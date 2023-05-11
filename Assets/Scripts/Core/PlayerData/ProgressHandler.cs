using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IProgressHandler
    {
        UniTask<int> GetPlayerExperienceAsync();
        UniTask AddExperienceAsync(int addedValue);
        UniTask SetExpirienceAsync(int totalValue);
    }


    public class ProgressHandler : IProgressHandler
    {
        private readonly IDataService _dataService;

        public ProgressHandler(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async UniTask<int> GetPlayerExperienceAsync()
        {
            return await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.Experience);
        }

        public async UniTask AddExperienceAsync(int addedValue)
        {
            var current = await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.Experience);
            current += addedValue;
            await _dataService.KeyValueStorage.SaveIntValue(KeyValueIntegerKeys.Experience, current);
        }

        public async UniTask SetExpirienceAsync(int totalValue)
        {
            await _dataService.KeyValueStorage.SaveIntValue(KeyValueIntegerKeys.Experience, totalValue);
        }
    }

}


