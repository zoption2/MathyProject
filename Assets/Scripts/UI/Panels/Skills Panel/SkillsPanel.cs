using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mathy.Data;
using System;
using TMPro;
using Cysharp.Threading.Tasks;
using Zenject;
using Mathy.Services;

namespace Mathy.UI
{
    public interface ISkillPlanPopupStub
    {
        UniTask DoSkillPlanUIWorkStub();
    }

    public class SkillsPanel : PopupPanel, ISkillPlanPopupStub
    {
        [Inject] private ISkillPlanService service;
        #region FIELDS

        [Header("COMPONENTS:")]
        [SerializeField] private List<SkillSettingsGUI> skillSettingsElements;
        [SerializeField] private List<GradeTabButton> gradeTabButtons;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button allSelectedButton;
        [SerializeField] private Toggle selectAllToggle;
        [SerializeField] private TMP_Text selectAllTitle;

        [Header("CONFIG:")]
        [SerializeField] private int defaultTabIndex;
        [SerializeField] private int availableGrades = 1;

        [Header("LOCALIZATION:")]
        [SerializeField] private string tableName = "Grades and Skills";
        [SerializeField] private string upToKey = "Skills Panel Up To";
        [SerializeField] private string selectAllKey = "Skills Panel Select All";
        [SerializeField] private string deselectAllKey = "Skills Panel Deselect All";

        private List<SkillData> selectedGradeData;
        private int selectedTabIndex = -1;
        private int selectedSkillsCount = 0;
        private int availableSkillsCount;
        private Dictionary<SkillType, SkillSettingsData> skillSettingsDatas = new();

        private UniTaskCompletionSource _tcs;
        #endregion

        public async UniTask DoSkillPlanUIWorkStub()
        {
            _tcs = new UniTaskCompletionSource();
            gameObject.SetActive(true);

            await _tcs.Task;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Initialize();
            var tab = selectedTabIndex == -1 ? defaultTabIndex : selectedTabIndex;
            SelectTab(tab);
            selectAllToggle.onValueChanged.AddListener(SelectAllSkills);
            closeButton.onClick.AddListener(ClosePanel);
        }

        private void OnDisable()
        {
            selectAllToggle.onValueChanged.RemoveListener(SelectAllSkills);
            closeButton.onClick.RemoveListener(ClosePanel);
            for (int i = 0; i < availableGrades; i++)
            {
                gradeTabButtons[i].Button.onClick.RemoveListener
                    (() => { SelectTab(i); });
            }
            TryUnsubscribeFromSkillSettings();
            _tcs?.TrySetCanceled();
        }

        private void Initialize()
        {
            if (availableGrades > gradeTabButtons.Count)
                availableGrades = gradeTabButtons.Count;

            for (int i = 0; i < availableGrades; i++)
            {
                gradeTabButtons[i].gameObject.SetActive(true);
                gradeTabButtons[i].UpdateDisplayStyle(availableGrades > 3);
                gradeTabButtons[i].Button.onClick.AddListener
                    (() => { SelectTab(i); });
            }
            for (int i = availableGrades; i < gradeTabButtons.Count; i++)
            {
                gradeTabButtons[i].gameObject.SetActive(false);
            }

            selectAllTitle.text = LocalizationManager.GetLocalizedString(tableName,
                selectAllToggle.isOn ? deselectAllKey : selectAllKey);
        }

        public void SelectTab(int tabToSelectIndex)
        {
            if (tabToSelectIndex < 0 || tabToSelectIndex >= gradeTabButtons.Count)
            {
                return;
            }

            // Set the State property of the selected tab to Selected.
            var selectedTab = gradeTabButtons[tabToSelectIndex];
            selectedTab.State = GradeTabState.Selected;

            // Set the State property of the previously selected tab to Default, if there was one.
            if (selectedTabIndex != -1 && selectedTabIndex != tabToSelectIndex)
            {
                var previousSelectedTab = gradeTabButtons[selectedTabIndex];
                previousSelectedTab.State = GradeTabState.Default;
            }

            selectedTabIndex = tabToSelectIndex;
            service.SelectedGrade = tabToSelectIndex + 1;
            UpdateDisplayedSkills();
        }

        private void UpdateDisplayedSkills()
        {
            selectedGradeData = service.GetSelectedGradeSkillDatas();
            FillDictionaryWithSkills(selectedGradeData);
            availableSkillsCount = selectedGradeData.Count;

            selectedSkillsCount = 0;
            //Initialization of all skill settings GUI depending on the available skill datas
            for (int i = 0; i < availableSkillsCount; i++)
            {
                var skill = selectedGradeData[i];
                var settings = skillSettingsElements[i];

                settings.gameObject.SetActive(true);
                var skillSettings = skill.Settings;
                var localizedName = GetSkillTitle(skillSettings.Skill);
                settings.Initialize(skillSettings.Skill, localizedName
                                    , skillSettings.Value, skillSettings.IsEnabled);
                if (skillSettings.IsEnabled)
                    selectedSkillsCount++;
            }

            //Deactivation of the rest skill settings GUI
            for (int i = availableSkillsCount; i < skillSettingsElements.Count; i++)
            {
                skillSettingsElements[i].gameObject.SetActive(false);
            }

            selectAllToggle.isOn = selectedSkillsCount == availableSkillsCount;
            
            selectAllTitle.text = LocalizationManager.GetLocalizedString(tableName,
                                 selectAllToggle.isOn ? deselectAllKey : selectAllKey);
            
            TryUnsubscribeFromSkillSettings();
            SubscribeToSkillSettings();
        }

        private void SubscribeToSkillSettings()
        {
            for (int i = 0; i < availableSkillsCount; i++)
            {
                skillSettingsElements[i].OnTogglePressed.AddListener(UpdateSkillActivityInternal);
                skillSettingsElements[i].OnSliderValueChanged.AddListener(UpdateSkillValueInternal);
            }
        }

        private void TryUnsubscribeFromSkillSettings()
        {
            for (int i = 0; i < availableSkillsCount; i++)
            {
                skillSettingsElements[i].OnTogglePressed.RemoveListener(UpdateSkillActivityInternal);
                skillSettingsElements[i].OnSliderValueChanged.RemoveListener(UpdateSkillValueInternal);
                skillSettingsElements[i].Release();
            }
        }

        private void FillDictionaryWithSkills(List<SkillData> storedData)
        {
            skillSettingsDatas.Clear();
            foreach (var skill in storedData)
            {
                skillSettingsDatas.Add(skill.Settings.Skill, skill.Settings);
            }
        }

        private void UpdateSkillActivityInternal(SkillType skillType, bool isEnable)
        {
            if (isEnable != skillSettingsDatas[skillType].IsEnabled)
            {
                selectedSkillsCount = isEnable
                    ? ++selectedSkillsCount
                    : --selectedSkillsCount;
            }

            if (skillSettingsDatas.ContainsKey(skillType))
            {
                skillSettingsDatas[skillType].IsEnabled = isEnable;
            }
        }

        private void UpdateSkillValueInternal(SkillType skillType, int value)
        {
            if (skillSettingsDatas.ContainsKey(skillType))
            {
                skillSettingsDatas[skillType].Value = value;
            }
        }

        public override void ClosePanel()
        {
            SaveSkillsSettings();
            base.ClosePanel();
            _tcs?.TrySetResult();
        }

        private async void SaveSkillsSettings()
        {
            foreach (var skillData in skillSettingsDatas.Values)
            {
                await service.SaveSkillSettings(skillData);
            }
            skillSettingsDatas.Clear();
        }

        /// <summary>
        /// Returns a localized title for a skill GUI element based on the specified SkillType.
        /// </summary>
        /// <param name="type">The type of the skill.</param>
        /// <returns>The localized title of the skill.</returns>
        private string GetSkillTitle(SkillType type)
        {
            var skill = Enum.GetName(typeof(SkillType), type);
            string skillKey = string.Format("{0} Skill", skill);
            string localizedSkill = LocalizationManager.GetLocalizedString(tableName, skillKey);
            string localizedSufix = LocalizationManager.GetLocalizedString(tableName, upToKey);
            string title = string.Format("{0} {1}", localizedSkill, localizedSufix);
            return title;
        }

        private void SelectAllSkills(bool isActive)
        {
            for (int i = 0; i < availableSkillsCount; i++)
            {
                skillSettingsElements[i].SetActive(isActive);
            }

            selectAllTitle.text = LocalizationManager.GetLocalizedString(tableName,
                isActive ? deselectAllKey: selectAllKey);
        }
    }
}