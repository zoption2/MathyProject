using Cysharp.Threading.Tasks;
using Mathy.Data;
using System.Data;

namespace Mathy.Services.Data
{
    public interface ISkillPlanHandler
    {
        UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true);
        UniTask SaveGradeState(int grade, bool isEnable);
        UniTask<SkillSettingsData> GetSkillSettings(int grade, SkillType skillType);
        UniTask SaveSkillSettings(SkillSettingsData settings);
        UniTask SaveSkillPlan(SkillSettingsData[] settings);
    }


    public class SkillPlanHandler : ISkillPlanHandler
    {
        private ISkillSettingsProvider _skillSettingsProvider;
        private IGradeSettingsProvider _gradeSettingsProvider;

        private string _dbFilePath;

        public SkillPlanHandler(string dbFilePath)
        {
            _dbFilePath = dbFilePath;

            _skillSettingsProvider = new SkillSettingsProvider(dbFilePath);
            _gradeSettingsProvider = new GradeSettingsProvider(dbFilePath);
        }

        public async UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true)
        {
            var result = await _gradeSettingsProvider.IsGradeEnabled(grade, defaultIsEnable);
            return result;
        }

        public async UniTask SaveGradeState(int grade, bool isEnable)
        {
            await _gradeSettingsProvider.SaveGradeSettings(grade, isEnable);
        }

        public async UniTask<SkillSettingsData> GetSkillSettings(int grade, SkillType skillType)
        {
            var result = await _skillSettingsProvider.GetSettingsByGradeAndSkill(grade, skillType);
            return result;
        }

        public async UniTask SaveSkillSettings(SkillSettingsData settings)
        {
            await _skillSettingsProvider.SaveSkillSettings(settings);
        }

        public async UniTask SaveSkillPlan(SkillSettingsData[] settings)
        {
            await _skillSettingsProvider.SaveSkillPlan(settings);
        }

        protected async UniTask TryCreateTables()
        {
            await _skillSettingsProvider.TryCreateTable();
        }
    }






}

