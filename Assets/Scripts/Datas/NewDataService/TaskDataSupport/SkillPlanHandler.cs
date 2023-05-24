using Cysharp.Threading.Tasks;
using Mathy.Data;


namespace Mathy.Services.Data
{
    public interface ISkillPlanHandler
    {
        UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true);
        UniTask SaveGradeState(int grade, bool isEnable);
        UniTask<SkillSettingsData> GetSkillSettingsAsync(int grade, SkillType skillType);
        SkillSettingsData GetSkillSettings(int grade, SkillType skillType);
        SkillSettingsData GetSavedOrInputed(SkillSettingsData input);
        UniTask SaveSkillSettings(SkillSettingsData settings);
        UniTask SaveSkillPlan(SkillSettingsData[] settings);
    }


    public class SkillPlanHandler : ISkillPlanHandler
    {
        private ISkillSettingsProvider _skillSettingsProvider;
        private IGradeSettingsProvider _gradeSettingsProvider;
        private readonly DataService _dataService;


        public SkillPlanHandler(DataService dataService)
        {
            _dataService = dataService;
            var filePath = dataService.DatabasePath;

            _skillSettingsProvider = new SkillSettingsProvider(filePath);
            _gradeSettingsProvider = new GradeSettingsProvider(filePath);
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

        public async UniTask<SkillSettingsData> GetSkillSettingsAsync(int grade, SkillType skillType)
        {
            var result = await _skillSettingsProvider.GetSettingsByGradeAndSkillAsync(grade, skillType);
            return result;
        }

        public SkillSettingsData GetSkillSettings(int grade, SkillType skillType)
        {
            var result = _skillSettingsProvider.GetFirstOrDefaultByGradeAndSkill(grade, skillType);
            return result;
        }

        public SkillSettingsData GetSavedOrInputed(SkillSettingsData input)
        {
            var resilt = _skillSettingsProvider.GetSavedOrInputed(input);
            return resilt;
        }

        public async UniTask SaveSkillSettings(SkillSettingsData settings)
        {
            await _skillSettingsProvider.SaveSkillSettings(settings);
        }

        public async UniTask SaveSkillPlan(SkillSettingsData[] settings)
        {
            await _skillSettingsProvider.SaveSkillPlan(settings);
        }

        public async UniTask Init()
        {
            await TryCreateTables();
        }

        public async UniTask ClearData()
        {
            await _skillSettingsProvider.DeleteTable();
            await _gradeSettingsProvider.DeleteTable();
        }

        protected async UniTask TryCreateTables()
        {
            await _skillSettingsProvider.TryCreateTable();
            await _gradeSettingsProvider.TryCreateTable();
        }
    }
}

