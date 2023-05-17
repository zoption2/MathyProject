using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IPlayerProgressProvider
    {
        UniTask<int> GetPlayerExperienceAsync();
        UniTask AddExperienceAsync(int addedValue);
        UniTask SetExpirienceAsync(int totalValue);
        UniTask<int> GetRankAsynk();
        UniTask SaveRankAsynk(int rank);
    }


    public class PlayerProgressProvider : IPlayerProgressProvider
    {
        private readonly IDataService _dataService;

        public PlayerProgressProvider(IDataService dataService)
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

        public async UniTask<int> GetRankAsynk()
        {
            return await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.PlayerRank);
        }

        public async UniTask SaveRankAsynk(int rank)
        {
            await _dataService.KeyValueStorage.SaveIntValue(KeyValueIntegerKeys.PlayerRank, rank);
        }
    }

}


