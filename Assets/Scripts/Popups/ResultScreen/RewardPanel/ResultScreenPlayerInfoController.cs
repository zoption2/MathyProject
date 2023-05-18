using Cysharp.Threading.Tasks;
using Mathy.Services;
using System;

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
        private readonly IAddressableRefsHolder _refsHolder;

        public ResultScreenPlayerInfoController(IDataService dataService, IAddressableRefsHolder refsHolder)
        {
            _dataService = dataService;
            _refsHolder = refsHolder;
        }

        protected async override UniTask<ResultScreenPlayerInfoModel> BuildModel()
        {
            var model = new ResultScreenPlayerInfoModel();
            model.PlayerName = await _dataService.PlayerData.Account.GetPlayerName();
            int playerRank = await _dataService.PlayerData.Progress.GetRankAsynk();

            PlayerRankImageType imageType = PlayerRankImageType.none;
            if (Enum.IsDefined(typeof(PlayerRankImageType), playerRank))
            {
                imageType = (PlayerRankImageType)playerRank;
            }
            var sprite = await _refsHolder.Images.PlayerRank.GetSpriteByType(imageType);
            model.PlayerIcon = sprite;

            return model;
        }

        protected override void DoOnInit(IResultScreenPlayerInfoView view)
        {
            view.SetName(_model.PlayerName);
            view.SetIcon(_model.PlayerIcon);
        }
    }
}


