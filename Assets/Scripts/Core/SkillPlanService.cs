using Cysharp.Threading.Tasks;
using Mathy.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mathy.Services
{
    public interface ISkillPlanService
    { 

    }


    public class SkillPlanService : ISkillPlanService
    {
        private readonly IDataService _dataService;
        private List<GradeSettings> _gradeSettings;

        //public bool IsAnySkillActivated
        //{
        //    get
        //    {
        //        return gradeDatas
        //        .Where(g => g.IsActive)
        //        .SelectMany(g => g.SkillDatas)
        //        .Any(s => s.IsActive);
        //    }
        //}

        public SkillPlanService(IDataService dataService, List<GradeSettings> gradeSettings)
        {
            _dataService = dataService;
            _gradeSettings = gradeSettings;
        }

        public async UniTask<bool> IsGradeEnable(int grade)
        {
            return await _dataService.SkillPlan.IsGradeEnabled(grade);
        }

        public async UniTask<bool> IsSkillTypeEnable(int grade, SkillType skillType)
        {
            var settings = await _dataService.SkillPlan.GetSkillSettings(grade, skillType);
            return settings.IsEnabled;
        }
    }
}

