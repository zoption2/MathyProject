using Cysharp.Threading.Tasks;
using Mathy.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Mathy.Services
{
    public interface ISkillPlanService
    {
        event Action ON_SKILL_PLAN_UPDATED;
        bool IsAnySkillActivated { get; }
        public int SelectedGrade { get; set; }
        List<ScriptableTask> GetAvailableTaskSettings();
        List<SkillData> GetSelectedGradeSkillDatas();
        UniTask SaveGradeState(int grade, bool isEnable);
        UniTask<bool> IsGradeEnable(int grade, bool defaultState = true);
        UniTask SaveSkillPlan(SkillSettingsData[] settings);
        UniTask SaveSkillSettings(SkillSettingsData settings);
    }


    public class SkillPlanService : ISkillPlanService
    {
        public event Action ON_SKILL_PLAN_UPDATED;

        private readonly IDataService _dataService;
        private List<GradeSettings> _gradeSettings;
        private List<GradeData> _gradeDatas;
        private int _selectedGrade = 1;

        public int SelectedGrade
        {
            get => _selectedGrade;
            set => _selectedGrade = value;
        }

        public bool IsAnySkillActivated
        {
            get
            {
                return _gradeDatas
                .Where(g => g.IsActive)
                .SelectMany(g => g.SkillDatas)
                .Any(s => s.Settings.IsEnabled);
            }
        }

        public SkillPlanService(IDataService dataService, List<GradeSettings> gradeSettings)
        {
            _dataService = dataService;
            _gradeSettings = gradeSettings;
            InitInternal();
        }

        private async void InitInternal()
        {
            _gradeDatas = await LoadSkillPlan(_gradeSettings);
        }


        public async UniTask<bool> IsGradeEnable(int grade, bool defaultState = true)
        {
            return await _dataService.SkillPlan.IsGradeEnabled(grade, defaultState);
        }

        public async UniTask<bool> IsSkillTypeEnable(int grade, SkillType skillType)
        {
            var settings = await _dataService.SkillPlan.GetSkillSettings(grade, skillType);
            return settings.IsEnabled;
        }

        public List<ScriptableTask> GetAvailableTaskSettings()
        {
            List<ScriptableTask> taskSettings = new List<ScriptableTask>();

            taskSettings = _gradeDatas
                .Where(g => g.IsActive)
                .SelectMany(g => g.SkillDatas)
                .Where(s => s.Settings.IsEnabled)
                .SelectMany(s => s.Tasks.Where(t => t.MaxNumber >= t.MinLimit && t.MaxNumber <= t.MaxLimit))
                .ToList();
            return taskSettings;
        }

        public List<SkillData> GetSelectedGradeSkillDatas()
        {
            var result = _gradeDatas
                .Where(x => x.GradeIndex == SelectedGrade)
                .SelectMany(g => g.SkillDatas)
                .ToList();
            return result;
        }

        public async UniTask SaveSkillSettings(SkillSettingsData settings)
        {
            settings.Grade = SelectedGrade;
            await _dataService.SkillPlan.SaveSkillSettings(settings);

            //_gradeDatas.Where(x => x.GradeIndex == settings.Grade)
            //    .SelectMany(t => t.SkillDatas.Where(s => s.Settings.Skill == settings.Skill))
            //    .SelectMany(x => x.Tasks.ForEach(g => g.MaxNumber = settings.Value));

            _gradeDatas.Where(x => x.GradeIndex == settings.Grade)
                       .SelectMany(t => t.SkillDatas.Where(s => s.Settings.Skill == settings.Skill))
                       .ToList()
                       .ForEach(x => x.Settings.IsEnabled = settings.IsEnabled);

           _gradeDatas.Where(x => x.GradeIndex == settings.Grade)
                        .SelectMany(t => t.SkillDatas.Where(s => s.Settings.Skill == settings.Skill))
                        .SelectMany(x => x.Tasks)
                        .ToList()
                        .ForEach(g =>
                        {
                            g.MaxNumber = settings.Value;
                        });

            ON_SKILL_PLAN_UPDATED?.Invoke();
        }

        public async UniTask SaveGradeState(int grade, bool isEnable)
        {
            await _dataService.SkillPlan.SaveGradeState(grade, isEnable);
        }

        public async UniTask SaveSkillPlan(SkillSettingsData[] settings)
        {
            await _dataService.SkillPlan.SaveSkillPlan(settings);
        }

        private async UniTask<List<GradeData>> LoadSkillPlan(List<GradeSettings> settings)
        {
            List<GradeData> result = new();
            foreach (var gradeSetting in settings)
            {
                var gradeData = new GradeData();
                int gradeIndex = settings.IndexOf(gradeSetting) + 1;
                gradeData.GradeIndex = gradeIndex;
                gradeData.IsActive = true;
                gradeData.SkillDatas = new List<SkillData>();
                var skillSettingsList = gradeSetting.SkillSettings;
                for (int i = 0, j = skillSettingsList.Count; i < j; i++)
                {
                    SkillData skillData = new SkillData();
                    SkillSettingsData skillSettings = await _dataService.SkillPlan.GetSkillSettings(gradeIndex, skillSettingsList[i].SkillType);
                    skillData.Settings = skillSettings;
                    var tasks = skillSettingsList[i].TaskSettings;
                    tasks.ForEach(x =>
                    {
                        x.MaxNumber = skillSettings.Value;
                    })
;
                    skillData.Tasks = tasks;
                    gradeData.SkillDatas.Add(skillData);
                }
                result.Add(gradeData);
            }

            return result;
        }
    }
}

