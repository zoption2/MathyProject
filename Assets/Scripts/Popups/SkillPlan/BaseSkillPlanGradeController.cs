using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mathy.Services;
using System;
using System.Collections.Generic;


namespace Mathy.UI
{
    public interface ISkillPlanGradeController : IBaseMediatedController, IView
    {
        int Grade { get; }
    }

    public abstract class BaseSkillPlanGradeController :
        BaseMediatedController<ISkillPlanPopupView, SkillPlanTabModel>
        , ISkillPlanGradeController
    {
        private const int kSettingsAppearDelay = 100;
        private const string kTableName = "Grades and Skills";
        private const string kUpToKey = "Skills Panel Up To";
        private const string kSkillFormat = "{0} Skill";
        private const string kTitleFormat = "{0} {1}";

        protected readonly ISkillPlanService _skillPlanService;
        protected readonly IAddressableRefsHolder _refsHolder;
        private Dictionary<SkillType, ISkillPlanSettingAdapter> _skillSettings;
        private Dictionary<SkillType, SkillSettingsData> _skillSettingsDatas;

        public abstract int Grade { get; }

        protected abstract SkillPlanPopupComponents SettingsComponent { get; }


        public BaseSkillPlanGradeController(ISkillPlanService skillPlanService
            , IAddressableRefsHolder refsHolder)
        {
            _skillPlanService = skillPlanService;
            _refsHolder = refsHolder;

            _skillSettingsDatas = new();
            _skillSettings = new();
        }

        public async void Show(Action onShow)
        {
            if(_isInited)
            {
                foreach (var settingsView in _skillSettings.Values)
                {
                    settingsView.Show(null);
                    await UniTask.Delay(kSettingsAppearDelay);
                    settingsView.ON_TOGGLE_CLICK += UpdateSkillActivityInternal;
                    settingsView.ON_VALUE_CHANGE += UpdateSkillValueInternal;
                }
            }
            onShow?.Invoke();
        }

        public void Hide(Action onHide)
        {
            if (_isInited)
            {
                foreach (var settingsView in _skillSettings.Values)
                {
                    settingsView.ON_TOGGLE_CLICK -= UpdateSkillActivityInternal;
                    settingsView.ON_VALUE_CHANGE -= UpdateSkillValueInternal;
                    settingsView.Hide(null);
                }
            }
            onHide?.Invoke();
        }

        public void Release()
        {
            if (_isInited)
            {
                SaveSkillsSettings();
            }
        }

        protected async override UniTask<SkillPlanTabModel> BuildModel()
        {
            var model = new SkillPlanTabModel();
            model.Grade = Grade;
            model.SkillsSettings = new List<BaseSkillSettingModel>();
            var skills = _skillPlanService.GetGradeSkillsData(Grade);
            FillDictionaryWithSkills(skills);
            for (int i = 0, j = skills.Count; i < j; i++)
            {
                var settingModel = new BaseSkillSettingModel();
                var settings = skills[i].Settings;
                var skillType = settings.Skill;
                settingModel.SkillType = skillType;
                settingModel.LocalizedTitle = GetSkillTitle(skillType);
                settingModel.Value = settings.Value;
                settingModel.IsEnable = settings.IsEnabled;

                model.SkillsSettings.Add(settingModel);
            }

            return await UniTask.FromResult(model);
        }

        protected override async UniTask DoOnInit(ISkillPlanPopupView view)
        {
            for (int i = 0, j = _model.SkillsSettings.Count; i < j; i++)
            {
                var settingsModel = _model.SkillsSettings[i];
                var skillType = settingsModel.SkillType;
                if (!_skillSettings.ContainsKey(skillType))
                {
                    var settingView = await _refsHolder.Popups.SkillPlanComponents
                        .InstantiateFromReference<ISkillPlanSettingAdapter>(SettingsComponent, view.SettingsHolder);
                    _skillSettings.Add(skillType, settingView);
                }

                var adapter = _skillSettings[skillType];
                adapter.SetTitle(settingsModel.LocalizedTitle);
                adapter.Init(skillType, settingsModel.IsEnable, settingsModel.Value);
            }
        }

        private void FillDictionaryWithSkills(List<SkillData> storedData)
        {
            _skillSettingsDatas.Clear();
            foreach (var skill in storedData)
            {
                _skillSettingsDatas.Add(skill.Settings.Skill, skill.Settings);
            }
        }

        private void UpdateSkillActivityInternal(SkillType skillType, bool isEnable)
        {
            if (_skillSettingsDatas.ContainsKey(skillType))
            {
                _skillSettingsDatas[skillType].IsEnabled = isEnable;
            }
        }

        private void UpdateSkillValueInternal(SkillType skillType, int value)
        {
            if (_skillSettingsDatas.ContainsKey(skillType))
            {
                _skillSettingsDatas[skillType].Value = value;
            }
        }

        private async void SaveSkillsSettings()
        {
            foreach (var skillData in _skillSettingsDatas.Values)
            {
                await _skillPlanService.SaveSkillSettings(skillData);
            }
            _skillSettingsDatas.Clear();
        }

        private string GetSkillTitle(SkillType type)
        {
            var skill = Enum.GetName(typeof(SkillType), type);
            string skillKey = string.Format(kSkillFormat, skill);
            string localizedSkill = LocalizationManager.GetLocalizedString(kTableName, skillKey);
            string localizedSufix = LocalizationManager.GetLocalizedString(kTableName, kUpToKey);
            string title = string.Format(kTitleFormat, localizedSkill, localizedSufix);
            return title;
        }
    }
}
