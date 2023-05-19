using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IPlayerAccountInfoProvider
    {
        UniTask<string> GetPlayerName();
        UniTask SetPlayerName(string name);
    }


    public class PlayerAccountInfoProvider : IPlayerAccountInfoProvider
    {
        private const string kPlayerNameKey = "PlayerName";

        private readonly IDataService _dataService;

        public PlayerAccountInfoProvider(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async UniTask<string> GetPlayerName()
        {
            return await _dataService.KeyValueStorage.GetStringOrDefaultAsync(kPlayerNameKey);
        }

        public async UniTask SetPlayerName(string name)
        {
            await _dataService.KeyValueStorage.SaveStringValueAsync(kPlayerNameKey, name);
        }
    }
}


