using Cysharp.Threading.Tasks;
using Mathy.Services;

namespace Mathy.UI
{
    public interface IResultScreenPlayerInfoController : IBaseMediatedController
    {

    }


    public class ResultScreenPlayerInfoController
        : BaseMediatedController<IResultScreenPlayerInfoView, ResultScreenPlayerInfoModel>
        , IResultScreenPlayerInfoController
    {

        private readonly IDataService _dataService;

        public ResultScreenPlayerInfoController(IDataService dataService)
        {
            _dataService = dataService;
        }

        protected async override UniTask<ResultScreenPlayerInfoModel> BuildModel()
        {
            var model = new ResultScreenPlayerInfoModel();
            model.PlayerName = await _dataService.PlayerData.Account.GetPlayerName();
            // here should be logic for getting PlayerIcon in future. Icons not exist for now.

            return model;
        }

        protected override void DoOnInit(IResultScreenPlayerInfoView view)
        {
            view.SetName(_model.PlayerName);
        }
    }
}


