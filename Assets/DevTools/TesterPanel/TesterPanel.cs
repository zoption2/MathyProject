using Mathy.Services;
using UnityEngine;
using Zenject;

namespace Mathy.Testing
{
    public class TesterPanel : MonoBehaviour
    {
        [SerializeField] private int _experience;

        [SerializeField] private Achievements _achievement;


        [Inject] private IDataService _dataService;

        [ContextMenu("SetExperience")]
        public async void SetExperience()
        {
            await _dataService.PlayerData.Progress.AddExperienceAsync(_experience);

            var totalExp = await _dataService.PlayerData.Progress.GetPlayerExperienceAsync();
            var rank = PointsHelper.GetRankByExperience(totalExp);
            await _dataService.PlayerData.Progress.SaveRankAsynk(rank);
        }

        [ContextMenu("IncrementAchievements")]
        public async void IncrementAchievements()
        {
            await _dataService.PlayerData.Achievements.IncrementAchievementValue(_achievement);
        }
    }
}

