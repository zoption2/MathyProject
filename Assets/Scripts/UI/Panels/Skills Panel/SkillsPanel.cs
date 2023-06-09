using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mathy.Data;
using System;
using TMPro;
using Cysharp.Threading.Tasks;

namespace Mathy.UI
{
    public class SkillsPanel : PopupPanel
    {
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

        private GradeData selectedGradeData;
        private int selectedTabIndex = -1;
        private int selectedSkillsCount = 0;
        private int availableSkillsCount;

        #endregion

        private void Awake()
        {
            Initialize();
        }

        private async void Start()
        {
            await UniTask.WaitUntil(() => GradeManager.Instance != null);
            SelectTab(defaultTabIndex);
        }

        private void Localize()
        {
            var skillDatas = selectedGradeData.SkillDatas;
            if (skillDatas.Count <= 0) return;
            for (int i = 0; i < availableSkillsCount; i++)
            {
                var skill = skillDatas[i];
                var settings = skillSettingsElements[i];
                settings.Localize(GetSkillTitle(skill.SkillType));
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
        }

        private void UpdateDisplayedSkills()
        {
            selectedGradeData = GradeManager.Instance.GradeDatas[selectedTabIndex];
            var skillDatas = selectedGradeData.SkillDatas;
            availableSkillsCount = skillDatas.Count;

            //Initialization of all skill settings GUI depending on the available skill datas
            for (int i = 0; i < availableSkillsCount; i++)
            {
                var skill = skillDatas[i];
                var settings = skillSettingsElements[i];

                settings.gameObject.SetActive(true);
                settings.Initialize(GetSkillTitle(skill.SkillType),
                    skill.MaxNumber, skill.IsActive);
                if (skill.IsActive)
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
                skillSettingsElements[i].OnTogglePressed.AddListener(
                (isAvailable) => GradeManager.Instance.SetSkillIsActive(
                    gradeIndex, skillIndex, isAvailable));
                skillSettingsElements[i].OnTogglePressed.AddListener((isEnabled) => this.SkillChecked(isEnabled));
                skillSettingsElements[i].OnSliderValueChanged.AddListener(
                (maxNumber) => GradeManager.Instance.SetSkillMaxNumber(
                    gradeIndex, skillIndex, maxNumber));
            }
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
            UpdateDisplayedSkills();
        }

        private void SkillChecked(bool isEnabled)
        {
            selectedSkillsCount += isEnabled ? 1 : -1;
            selectedSkillsCount = Mathf.Clamp(selectedSkillsCount, 0, availableSkillsCount);
            selectAllToggle.isOn = selectedSkillsCount == availableSkillsCount;
            _ = PlayButtonPanel.Instance.CheckSkills();
        }

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