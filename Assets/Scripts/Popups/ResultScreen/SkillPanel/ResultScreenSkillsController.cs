using Cysharp.Threading.Tasks;
using Mathy.Data;
using System.Collections.Generic;
using System.Linq;
using Mathy.Services;


namespace Mathy.UI
{
    public interface IResultScreenSkillsController : IBaseMediatedController
    {
        
    }


    public class ResultScreenSkillsController 
        : BaseMediatedController<IResultScreenSkillsPanelView, ResultScreenSkillPanelModel>
        , IResultScreenSkillsController
    {
        private const int kGrade = 1;
        private const string kResultScreenTable = "ResultScreen";
        private const string kSkillsKey = "SkillsKey";
        private const string kTotalKey = "Total";

        private const string kGradesAndSkillsTable = "Grades and Skills";
        private const string kSkillNameFormat = "{0} Skill";
        private const string kSkillResultFormat = "{0}%  {1}/{2}";
        private const string kLastShowedSkillsFormat = "{0}LastShowed";
        private const string kLastShowedRateFormat = "{0}LastShowedRate";
        private const string kUpdateFormat = "+{0}";

        private readonly IDataService _dataService;

        public ResultScreenSkillsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        protected async override void DoOnInit(IResultScreenSkillsPanelView view)
        {
            _view.SetTitle(_model.LocalizedTitle);
            _view.SetTotalLocalized(_model.TotalLocalized);
            var results = string.Format(kSkillResultFormat
                , _model.MiddleRate
                , _model.TotalCorrect
                , _model.TotalTasks);
            _view.SetTotalResults(results);
            var skills = _view.ProgressViews;
            for (int i = 0, j = skills.Length; i < j; i++)
            {
                var skillView = skills[i];
                skillView.Init();
                var skillModel = _model.SkillsProgressModels[skillView.Skill];
                var skillResult = string.Format(kSkillResultFormat
                    , skillModel.CorrectRate
                    , skillModel.TotalCorrect
                    , skillModel.TotalPlayed);
                skillView.SetResults(skillResult);
                var lastShowedKey = string.Format(kLastShowedSkillsFormat, skillView.Skill);
                var lastShowedTotalCorrect = await _dataService.KeyValueStorage.GetIntValue(lastShowedKey);
                if (lastShowedTotalCorrect < skillModel.TotalCorrect)
                {
                    var value = skillModel.TotalCorrect - lastShowedTotalCorrect;
                    var formatedValue = string.Format(kUpdateFormat, value);
                    skillView.ShowChangedValue(formatedValue);
                    await _dataService.KeyValueStorage.SaveIntValue(lastShowedKey, skillModel.TotalCorrect);
                }

                var lastRateKey = string.Format(kLastShowedRateFormat, skillView.Skill);
                var lastRateValue = await _dataService.KeyValueStorage.GetIntValue(lastRateKey);

                var needAnimate = false;
                if (skillModel.CorrectRate != lastRateValue)
                {
                    await _dataService.KeyValueStorage.SaveIntValue(lastRateKey, skillModel.CorrectRate);
                    needAnimate = true;
                }
                skillView.SetProgressBar(skillModel.CorrectRate, lastRateValue, needAnimate);
            }

            _view.Show(null);
        }

        protected async override UniTask<ResultScreenSkillPanelModel> BuildModel()
        {
            int totalTasks = 0;
            int totalCorrect = 0;
            var model = new ResultScreenSkillPanelModel();
            model.LocalizedTitle = LocalizationManager.GetLocalizedString(kResultScreenTable, kSkillsKey);
            model.TotalLocalized = LocalizationManager.GetLocalizedString(kResultScreenTable, kTotalKey);
            model.SkillsProgressModels = new Dictionary<SkillType, SkillResultProgressModel>();


            var skills = _view.ProgressViews.Select(x => x.Skill).ToList();
            for (int i = 0, j = skills.Count; i < j; i++)
            {
                var progressModel = new SkillResultProgressModel();
                var key = string.Format(kSkillNameFormat, skills[i]);
                progressModel.LocalizedTitle = LocalizationManager.GetLocalizedString(kGradesAndSkillsTable, key);
                var skillInfo = await _dataService.TaskData.GetSkillStatistic(skills[i], kGrade);
                progressModel.TotalPlayed = skillInfo.Total;
                progressModel.TotalCorrect = skillInfo.Correct;
                progressModel.CorrectRate = skillInfo.Rate;
                model.SkillsProgressModels.Add(skills[i], progressModel);

                totalTasks += skillInfo.Total;
                totalCorrect += skillInfo.Correct;
            }

            model.TotalTasks = totalTasks;
            model.TotalCorrect = totalCorrect;
            model.MiddleRate = totalTasks > 0
                ? (totalCorrect * 100) / totalTasks
                : 0;

            return model;
        }
    }
}


