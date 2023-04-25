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
    public class SkillsPanel : PopupPanel
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

        #endregion

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            SelectTab(defaultTabIndex);
        }

        private void Localize()
        {
            var skillDatas = selectedGradeData;
            if (skillDatas.Count <= 0) return;
            for (int i = 0; i < availableSkillsCount; i++)
            {
                var skill = skillDatas[i];
                var settings = skillSettingsElements[i];
                settings.Localize(GetSkillTitle(skill.Settings.Skill));
            }
            selectAllTitle.text = LocalizationManager.GetLocalizedString(tableName,
                selectAllToggle.isOn ? deselectAllKey : selectAllKey);
        }

        private void Initialize()
        {
            if (availableGrades > gradeTabButtons.Count)
                availableGrades = gradeTabButtons.Count;

            for (int i = 0; i < availableGrades; i++)
            {
                //var grade = i + 1;
                //var isGradeEnabled = await service.IsGradeEnable(grade, true);
                int tabIndex = i;
                gradeTabButtons[i].gameObject.SetActive(true);
                gradeTabButtons[i].UpdateDisplayStyle(availableGrades > 3);
                gradeTabButtons[i].Button.onClick.AddListener
                    (() => { SelectTab(tabIndex); });
            }
            for (int i = availableGrades; i < gradeTabButtons.Count; i++)
            {
                gradeTabButtons[i].gameObject.SetActive(false);
            }
            
            selectAllToggle.onValueChanged.AddListener(SelectAllSkills);
            closeButton.onClick.AddListener(ClosePanel);
            LocalizationManager.OnLanguageChanged.AddListener(Localize);

            selectAllTitle.text = LocalizationManager.GetLocalizedString(tableName,
                selectAllToggle.isOn ? deselectAllKey : selectAllKey);
        }

        public override void ClosePanel()
        {
            SaveSkillsSettings();
            base.ClosePanel();
        }

        private void UpdateDisplayedSkills()
        {
            selectedGradeData = service.GetSelectedGradeSkillDatas();
            availableSkillsCount = selectedGradeData.Count;

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
            SubscribeToSkillSettings();
        }

        private void SubscribeToSkillSettings()
        {
            for (int i = 0; i < availableSkillsCount; i++)
            //for (int i = 0; i < 5; i++)
            {
                int gradeIndex = selectedTabIndex + 1;
                int skillIndex = i;
                //skillSettingsElements[i].OnTogglePressed.AddListener(
                //(isAvailable) => GradeManager.Instance.SetSkillIsActive(
                //    gradeIndex, skillIndex, isAvailable));
                skillSettingsElements[i].OnTogglePressed.AddListener(UpdateSkillActivityInternal);
                skillSettingsElements[i].OnSliderValueChanged.AddListener(UpdateSkillValueInternal);
            }
        }

        private void UpdateSkillActivityInternal(SkillType skillType, bool isEnable)
        {
            if (!skillSettingsDatas.ContainsKey(skillType))
            {
                var data = new SkillSettingsData();
                data.Skill = skillType;
                skillSettingsDatas.Add(skillType, data);
            }
            skillSettingsDatas[skillType].IsEnabled = isEnable;
        }

        private void UpdateSkillValueInternal(SkillType skillType, int value)
        {
            if (!skillSettingsDatas.ContainsKey(skillType))
            {
                var data = new SkillSettingsData();
                data.Skill = skillType;
                skillSettingsDatas.Add(skillType, data);
            }
            skillSettingsDatas[skillType].Value = value;
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
            string skillKey = $"{Enum.GetName(typeof(SkillType), type)} Skill";
            string title = $"{LocalizationManager.GetLocalizedString(tableName, skillKey)} " +
                $"{LocalizationManager.GetLocalizedString(tableName, upToKey)}";
            return title;
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

        //private void CheckSkill(bool isEnabled)
        //{
        //    selectedSkillsCount += isEnabled ? 1 : -1;
        //    selectedSkillsCount = Mathf.Clamp(selectedSkillsCount, 0, availableSkillsCount);
        //    selectAllToggle.isOn = selectedSkillsCount == availableSkillsCount;
        //    _ = PlayButtonPanel.Instance.CheckSkills();
        //}

        private void SelectAllSkills(bool isActive)
        {
            if (isActive)
            {
                for (int i = 0; i < availableSkillsCount; i++)
                {
                    skillSettingsElements[i].SetActive(true);
                }
            }
            else if (selectedSkillsCount == availableSkillsCount)
            {
                for (int i = 0; i < availableSkillsCount; i++)
                {
                    skillSettingsElements[i].SetActive(false);
                }
            }
            selectAllTitle.text = LocalizationManager.GetLocalizedString(tableName,
                isActive ? deselectAllKey: selectAllKey);
        }
    }
}