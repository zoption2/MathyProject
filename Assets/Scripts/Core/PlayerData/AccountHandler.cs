using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public interface IAccountHandler
    {
        UniTask<string> GetPlayerName();
        UniTask SetPlayerName(string name);
    }

    public class AccountHandler : IAccountHandler
    {
        private const string kPlayerNameKey = "PlayerName";

        private readonly IDataService _dataService;

        public AccountHandler(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async UniTask<string> GetPlayerName()
        {
            return await _dataService.KeyValueStorage.GetStringOrDefaultByKey(kPlayerNameKey);
        }

        public async UniTask SetPlayerName(string name)
        {
            await _dataService.KeyValueStorage.SetStringValue(kPlayerNameKey, name);
        }
    }
}


