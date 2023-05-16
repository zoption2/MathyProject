using Mathy.Services.Data;

namespace Mathy.Services
{
    public interface IPlayerDataService
    {
        IProgressHandler Progress { get; }
        IAchievementsHandler Achievements { get; }
        IAccountHandler Account { get; }
    }

    public class PlayerDataService : IPlayerDataService
    {
        private readonly IAchievementsHandler _achievements;
        private readonly IProgressHandler _progress;
        private readonly IAccountHandler _account;

        public PlayerDataService(IAchievementsHandler achievementsHandler
            , IProgressHandler progressHandler
            , IAccountHandler accountHandler)
        {
            _achievements = achievementsHandler;
            _progress = progressHandler;
            _account = accountHandler;
        }

        public IProgressHandler Progress => _progress;
        public IAchievementsHandler Achievements => _achievements;
        public IAccountHandler Account => _account;
    }
}


