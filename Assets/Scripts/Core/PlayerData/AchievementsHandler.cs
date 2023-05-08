using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IAchievementsHandler
    {
        UniTask<int> GetGoldMedals();
        UniTask<int> GetSilverMedals();
        UniTask<int> GetBronzeMedals();
        UniTask<int> GetChallengeCups();
        UniTask IncrementGoldMedals();
        UniTask IncrementSilverMedals();
        UniTask IncrementBronzeMedals();
        UniTask IncrementCupsMedals();
    }

    public class AchievementsHandler : IAchievementsHandler
    {
        private readonly IDataService _dataService;

        public AchievementsHandler(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async UniTask<int> GetBronzeMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.BronzeMedals);
        }

        public async UniTask<int> GetChallengeCups()
        {
            return await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.ChallengeCups);
        }

        public async UniTask<int> GetGoldMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.GoldMedals);
        }

        public async UniTask<int> GetSilverMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValue(KeyValueIntegerKeys.SilverMedals);
        }

        public async UniTask IncrementBronzeMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(KeyValueIntegerKeys.BronzeMedals);
        }

        public async UniTask IncrementCupsMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(KeyValueIntegerKeys.ChallengeCups);
        }

        public async UniTask IncrementGoldMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(KeyValueIntegerKeys.GoldMedals);
        }

        public async UniTask IncrementSilverMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(KeyValueIntegerKeys.SilverMedals);
        }
    }

}


