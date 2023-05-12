using Mathy.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace Mathy.Testing
{
    public class TesterPanel : MonoBehaviour
    {
        [SerializeField] private int _experience;

        [SerializeField] private Achievements _achievement;


        [Inject] private IPlayerDataService _playerDataService;

        [ContextMenu("SetExperience")]
        public async void SetExperience()
        {
            await _playerDataService.Progress.AddExperienceAsync(_experience);

            var totalExp = await _playerDataService.Progress.GetPlayerExperienceAsync();
            var rank = PointsHelper.GetRankByExperience(totalExp);
            await _playerDataService.Progress.SaveRankAsynk(rank);
        }

        [ContextMenu("IncrementAchievements")]
        public async void IncrementAchievements()
        {
            await _playerDataService.Achievements.IncrementAchievementValue(_achievement);
        }
    }
}

