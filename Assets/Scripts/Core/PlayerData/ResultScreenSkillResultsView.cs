using Cysharp.Threading.Tasks;
using Mathy.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using Mathy.Core.Tasks.DailyTasks;
using System.Numerics;
using static UnityEditor.VersionControl.Asset;

namespace Mathy.Services
{
    public interface IResultScreenSkillResultsView : IView
    {
        void SetTitle(string title);
        ISkillResultProgressView[] ProgressViews { get; }
    }

    public class ResultScreenSkillResultsView : MonoBehaviour, IResultScreenSkillResultsView
    {
        [SerializeField] private TMP_Text _title;
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
    }


    public class ResultScreenSkillResultsModel
    {
        public string LocalizedTitle;
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
        void Init(IResultScreenSkillResultsView view);
    }

    public class ResultScreenSkillsController : IResultScreenSkillsController
    {
        private const int kGrade = 1;
        private const string kResultScreenTable = "ResultScreen";
        private const string kSkillsKey = "SkillsKey";
        private const string kGradesAndSkillsTable = "Grades and Skills";
        private const string kSkillNameFormat = "{0} Skill";
        private const string kSkillResultFormat = "{0}%  {1}/{2}";
        private const string kLastShowedSkillsFormat = "{0}LastShowed";

        private const string kCountKey = "Count Skill";
        private const string kAdditionKey = "Addition Skill";
        private const string kSubtructionKey = "Subtraction Skill";
        private const string kComparisonKey = "Comparison Skill";
        private const string kComplexKey = "Complex Skill";
        private const string kShapesKey = "Shapes Skill";
        private const string kSortingKey = "Sorting Skill";

        private readonly IDataService _dataService;
        private ResultScreenSkillResultsModel _model;
        private IResultScreenSkillResultsView _view;

        public ResultScreenSkillsController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async void Init(IResultScreenSkillResultsView view)
        {
            _view = view;
            _model = await BuildModel();
            _view.SetTitle(_model.LocalizedTitle);
            var skills = _view.ProgressViews;
            for (int i = 0, j = skills.Length; i < j; i++)
            {
                var skillView = skills[i];
                var skillModel = _model.SkillsProgressModels[skillView.Skill];
                skillView.SetProgressRate(skillModel.CorrectRate);
                var result = string.Format(kSkillResultFormat
                    , skillModel.CorrectRate
                    , skillModel.TotalCorrect
                    , skillModel.TotalPlayed);
                skillView.SetResults(result);
                var lastShowedKey = string.Format(kLastShowedSkillsFormat, skillView.Skill);
                var lastShowed = await _dataService.KeyValueStorage.GetIntValue(lastShowedKey);
                if (lastShowed < skillModel.TotalPlayed)
                {
                    var value = skillModel.TotalPlayed - lastShowed;
                    skillView.ShowChangedValue(value.ToString());
                    await _dataService.KeyValueStorage.SaveIntValue(lastShowedKey, value);
                }
            }
        }

        private async UniTask<ResultScreenSkillResultsModel> BuildModel()
        {
            var model = new ResultScreenSkillResultsModel();
            model.LocalizedTitle = LocalizationManager.GetLocalizedString(kResultScreenTable, kSkillsKey);
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
            }

            return model;
        }
    }



    public interface IResultScreenView : IView
    {
        IResultScreenSkillResultsView SkillResults { get; }
    }

    public class ResultScreenView : MonoBehaviour, IResultScreenView
    {
        [SerializeField] private ResultScreenSkillResultsView _skillResults;

        public IResultScreenSkillResultsView SkillResults => _skillResults;

        public void Show(Action onShow)
        {
            gameObject.SetActive(true);
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            gameObject.SetActive(false);
        }

        public void Release()
        {
            SkillResults.Release();
        }
    }
}


