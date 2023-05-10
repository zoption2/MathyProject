using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IAchievementsHandler
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

    public class AchievementsHandler : IAchievementsHandler
    {
        private readonly IDataService _dataService;

        private string _goldKey => Achievements.GoldMedal.ToString();
        private string _silverKey => Achievements.SilverMedal.ToString();
        private string _bronzeKey => Achievements.BronzeMedal.ToString();
        private string _cupKey => Achievements.ChallengeCup.ToString();

        public AchievementsHandler(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async UniTask<int> GetAchievementValue(Achievements achievement)
        {
            var key = achievement.ToString();
            return await _dataService.KeyValueStorage.GetIntValue(key);
        }


        public async UniTask IncrementAchievementValue(Achievements achievement)
        {
            var key = achievement.ToString();
            await _dataService.KeyValueStorage.IncrementIntValue(key);
        }

        public async UniTask<int> GetBronzeMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValue(_bronzeKey);
        }

        public async UniTask<int> GetChallengeCups()
        {
            return await _dataService.KeyValueStorage.GetIntValue(_cupKey);
        }

        public async UniTask<int> GetGoldMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValue(_goldKey);
        }

        public async UniTask<int> GetSilverMedals()
        {
            return await _dataService.KeyValueStorage.GetIntValue(_silverKey);
        }

        public async UniTask IncrementBronzeMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(_bronzeKey);
        }

        public async UniTask IncrementCupsMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(_cupKey);
        }

        public async UniTask IncrementGoldMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(_goldKey);
        }

        public async UniTask IncrementSilverMedals()
        {
            await _dataService.KeyValueStorage.IncrementIntValue(_silverKey);
        }
    }

}


