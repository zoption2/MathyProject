﻿using Cysharp.Threading.Tasks;
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

        private readonly IPlayerDataService _playerService;
        private readonly IDataService _dataService;

        public ResultScreenRewardController(IPlayerDataService playerService, IDataService dataService)
        {
            _playerService = playerService;
            _dataService = dataService;
        }

        protected async override void DoOnInit(IResultScreenRewardView view)
        {
            _view.SetTitle(_model.LocalizedTitle);
            _view.SetExperience(_model.RewardValue, _model.PreviousValue, _model.NeedAnimation);
            var lastExpKey = string.Format(kLastShowedExpFormat, KeyValueIntegerKeys.Experience);
            await _dataService.KeyValueStorage.SaveIntValue(lastExpKey, _model.RewardValue);
            _view.Show(null);
        }

        protected override async UniTask<ResultScreenRewardModel> BuildModel()
        {
            var model = new ResultScreenRewardModel();
            model.LocalizedTitle = LocalizationManager.GetLocalizedString(kResultScreenTable, kRewardTitleKey);
            var expValue = await _playerService.Progress.GetPlayerExperienceAsync();
            model.RewardValue = expValue;
            var lastExpKey = string.Format(kLastShowedExpFormat, KeyValueIntegerKeys.Experience);
            var previousExp = await _dataService.KeyValueStorage.GetIntValue(lastExpKey);
            model.PreviousValue = previousExp;
            bool needAnimation = expValue > previousExp;
            model.NeedAnimation = needAnimation;

            return model;
        }
    }
}

