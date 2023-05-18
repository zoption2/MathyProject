using Mathy.Services.Data;

namespace Mathy.Services
{
    public interface IPlayerDataHolder
    {
        IPlayerProgressProvider Progress { get; }
        IPlayerAchievementsProvider Achievements { get; }
        IPlayerAccountInfoProvider Account { get; }
    }

    public class PlayerDataHolder : IPlayerDataHolder
    {
        private readonly IDataService _dataService;
        private readonly IPlayerAchievementsProvider _achievements;
        private readonly IPlayerProgressProvider _progress;
        private readonly IPlayerAccountInfoProvider _account;

        public PlayerDataHolder(IDataService dataService)
        {
            _dataService = dataService;
            _achievements = new PlayerAchievementsProvider(dataService);
            _progress = new PlayerProgressProvider(dataService);
            _account = new PlayerAccountInfoProvider(dataService);
        }

        public IPlayerProgressProvider Progress => _progress;
        public IPlayerAchievementsProvider Achievements => _achievements;
        public IPlayerAccountInfoProvider Account => _account;
    }
}


