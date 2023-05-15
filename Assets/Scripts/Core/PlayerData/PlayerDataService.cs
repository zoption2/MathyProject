using Mathy.Services.Data;

namespace Mathy.Services
{
    public interface IPlayerDataService
    {
        IProgressHandler Progress { get; }
        IAchievementsHandler Achievements { get; }
    }

    public class PlayerDataService : IPlayerDataService
    {
        private readonly IAchievementsHandler _achievements;
        private readonly IProgressHandler _progress;

        public PlayerDataService(IAchievementsHandler achievementsHandler, IProgressHandler progressHandler)
        {
            _achievements = achievementsHandler;
            _progress = progressHandler;
        }

        public IProgressHandler Progress => _progress;
        public IAchievementsHandler Achievements => _achievements;
    }
}


