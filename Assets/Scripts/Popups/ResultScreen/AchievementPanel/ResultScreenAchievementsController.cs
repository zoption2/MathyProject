using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mathy.Services;


namespace Mathy.UI
{
    public interface IResultScreenAchievementsController : IBaseMediatedController
    {

    }


    public class ResultScreenAchievementsController 
        : BaseMediatedController<IResultScreenAchievementsView, ResultScreenAchievementsModel>
        , IResultScreenAchievementsController
    {
        private const string kResultScreenTable = "ResultScreen";
        private const string kAchievementsKey = "AchievementsKey";
        private readonly IDataService _dataService;

        public ResultScreenAchievementsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        protected override void DoOnInit(IResultScreenAchievementsView view)
        {
            _view.SetTitle(_model.LocalizedTitle);
            var achievements = _view.AchievementViews;
            for (int i = 0, j = achievements.Length; i < j; i++)
            {
                var achievementView = achievements[i];
                var achievementModel = _model.Achievements[achievementView.Achievement];
                var textValue = achievementModel.Value.ToString();
                achievementView.SetValue(textValue);
            }
            _view.Show(null);
        }

        protected async override UniTask<ResultScreenAchievementsModel> BuildModel()
        {
            var model = new ResultScreenAchievementsModel();
            model.LocalizedTitle = LocalizationManager.GetLocalizedString(kResultScreenTable, kAchievementsKey);
            model.Achievements = new Dictionary<Achievements, AchievementModel>();

            var achievements = _view.AchievementViews.Select(x => x.Achievement).ToList();
            for (int i = 0, j = achievements.Count; i < j; i++)
            {
                var achievementModel = new AchievementModel();
                var name = achievements[i];
                var value = await _dataService.PlayerData.Achievements.GetAchievementValue(name);
                achievementModel.Achievement = name;
                achievementModel.Value = value;

                model.Achievements.Add(name, achievementModel);
            }

            return model;
        }
    }
}


