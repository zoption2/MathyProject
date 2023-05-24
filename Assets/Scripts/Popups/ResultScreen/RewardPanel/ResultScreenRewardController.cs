using Cysharp.Threading.Tasks;
using Mathy.Services;

namespace Mathy.UI
{
    public interface IResultScreenRewardController : IBaseMediatedController
    {

    }

    public class ResultScreenRewardController
        : BaseMediatedController<IResultScreenRewardView, ResultScreenRewardModel>
        , IResultScreenRewardController
    {
        private const string kResultScreenTable = "ResultScreen";
        private const string kRewardTitleKey = "RewardKey";

        private const string kLastShowedExpFormat = "{0}LastShowed";

        private readonly IDataService _dataService;

        public ResultScreenRewardController(IDataService dataService)
        {
            _dataService = dataService;
        }

        protected async override UniTask DoOnInit(IResultScreenRewardView view)
        {
            _view.SetTitle(_model.LocalizedTitle);
            _view.SetExperience(_model.RewardValue, _model.PreviousValue, _model.NeedAnimation);
            var lastExpKey = string.Format(kLastShowedExpFormat, KeyValueIntegerKeys.Experience);
            await _dataService.KeyValueStorage.SaveIntValueAsync(lastExpKey, _model.RewardValue);
            _view.Show(null);
            await UniTask.CompletedTask;
        }

        protected override async UniTask<ResultScreenRewardModel> BuildModel()
        {
            var model = new ResultScreenRewardModel();
            model.LocalizedTitle = LocalizationManager.GetLocalizedString(kResultScreenTable, kRewardTitleKey);
            var expValue = await _dataService.PlayerData.Progress.GetPlayerExperienceAsync();
            model.RewardValue = expValue;
            var lastExpKey = string.Format(kLastShowedExpFormat, KeyValueIntegerKeys.Experience);
            var previousExp = await _dataService.KeyValueStorage.GetIntValueAsync(lastExpKey);
            model.PreviousValue = previousExp;
            bool needAnimation = expValue > previousExp;
            model.NeedAnimation = needAnimation;

            return model;
        }
    }
}


