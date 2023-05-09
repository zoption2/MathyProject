using Cysharp.Threading.Tasks;
using Mathy.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;


namespace Mathy.Services
{
    public interface IResultScreenSkillsPanelView : IView
    {
        void SetTitle(string title);
        void SetTotalLocalized(string text);
        void SetTotalResults(string text);
        ISkillResultProgressView[] ProgressViews { get; }
    }

    public class ResultScreenSkillsPanelView : MonoBehaviour, IResultScreenSkillsPanelView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _totalText;
        [SerializeField] private TMP_Text _totalResultText;
        [SerializeField] private SkillResultProgressView[] _progressViews;

        public ISkillResultProgressView[] ProgressViews => _progressViews;

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
        }

        public void Hide(Action onHide)
        {
            gameObject.SetActive(false);
        }

        public void Release()
        {
            
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public void SetTotalLocalized(string text)
        {
            _totalText.text = text;
        }

        public void SetTotalResults(string text)
        {
            _totalResultText.text = text;
        }
    }


    public class ResultScreenSkillPanelModel
    {
        public string LocalizedTitle;
        public string TotalLocalized;
        public int TotalTasks;
        public int TotalCorrect;
        public int MiddleRate;
        public Dictionary<SkillType, SkillResultProgressModel> SkillsProgressModels;
    }

    public class SkillResultProgressModel : IModel
    {
        public SkillType SkillType;
        public string LocalizedTitle;
        public int TotalPlayed;
        public int TotalCorrect;
        public int CorrectRate;
    }



    public interface IResultScreenSkillsController
    {
        void Init(IResultScreenSkillsPanelView view);
    }

    public class ResultScreenSkillsController : IResultScreenSkillsController
    {
        private const int kGrade = 1;
        private const string kResultScreenTable = "ResultScreen";
        private const string kSkillsKey = "SkillsKey";
        private const string kTotalKey = "Total";

        private const string kGradesAndSkillsTable = "Grades and Skills";
        private const string kSkillNameFormat = "{0} Skill";
        private const string kSkillResultFormat = "{0}%  {1}/{2}";
        private const string kLastShowedSkillsFormat = "{0}LastShowed";
        private const string kUpdateFormat = "+{0}";

        private readonly IDataService _dataService;
        private ResultScreenSkillPanelModel _model;
        private IResultScreenSkillsPanelView _view;

        public ResultScreenSkillsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async void Init(IResultScreenSkillsPanelView view)
        {
            _view = view;
            _model = await BuildModel();
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
                skillView.SetProgressRate(skillModel.CorrectRate);
                var skillResult = string.Format(kSkillResultFormat
                    , skillModel.CorrectRate
                    , skillModel.TotalCorrect
                    , skillModel.TotalPlayed);
                skillView.SetResults(skillResult);
                var lastShowedKey = string.Format(kLastShowedSkillsFormat, skillView.Skill);
                var lastShowed = await _dataService.KeyValueStorage.GetIntValue(lastShowedKey);
                if (lastShowed < skillModel.TotalCorrect)
                {
                    var value = skillModel.TotalCorrect - lastShowed;
                    var formatedValue = string.Format(kUpdateFormat, value);
                    skillView.ShowChangedValue(formatedValue);
                    await _dataService.KeyValueStorage.SaveIntValue(lastShowedKey, skillModel.TotalCorrect);
                }
            }
        }

        private async UniTask<ResultScreenSkillPanelModel> BuildModel()
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
            model.MiddleRate = (totalCorrect * 100) / totalTasks;

            return model;
        }
    }
}


