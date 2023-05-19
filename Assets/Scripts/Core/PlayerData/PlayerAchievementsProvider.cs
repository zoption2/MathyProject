using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IPlayerAchievementsProvider
    {
        UniTask<int> GetAchievementValue(Achievements achievement);
        UniTask IncrementAchievementValue(Achievements achievement);
        UniTask<int> GetGoldMedals();
        UniTask<int> GetSilverMedals();
        UniTask<int> GetBronzeMedals();
        UniTask<int> GetChallengeCups();
        UniTask IncrementGoldMedals();
        UniTask IncrementSilverMedals();
        UniTask IncrementBronzeMedals();
        UniTask IncrementCupsMedals();
    }

    public class PlayerAchievementsProvider : IPlayerAchievementsProvider
    {
        private readonly IDataService _dataService;

        private string _goldKey => Achievements.GoldMedal.ToString();
        private string _silverKey => Achievements.SilverMedal.ToString();
        private string _bronzeKey => Achievements.BronzeMedal.ToString();
        private string _cupKey => Achievements.ChallengeCup.ToString();

        public PlayerAchievementsProvider(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async UniTask<int> GetAchievementValue(Achievements achievement)
        {
            var key = achievement.ToString();
            return await _dataService.KeyValueStorage.GetIntValueAsync(key);
        }


        public async UniTask IncrementAchievementValue(Achievements achievement)
        {
            var key = achievement.ToString();
            await _dataService.KeyValueStorage.IncrementIntValueAsync(key);
        }

        public async UniTask<int> GetBronzeMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValueAsync(_bronzeKey);
        }

        public async UniTask<int> GetChallengeCups()
        {
            return await _dataService.KeyValueStorage.GetIntValueAsync(_cupKey);
        }

        public async UniTask<int> GetGoldMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValueAsync(_goldKey);
        }

        public async UniTask<int> GetSilverMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValueAsync(_silverKey);
        }

        public async UniTask IncrementBronzeMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValueAsync(_bronzeKey);
        }

        public async UniTask IncrementCupsMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValueAsync(_cupKey);
        }

        public async UniTask IncrementGoldMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValueAsync(_goldKey);
        }

        public async UniTask IncrementSilverMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValueAsync(_silverKey);
        }
    }
}


