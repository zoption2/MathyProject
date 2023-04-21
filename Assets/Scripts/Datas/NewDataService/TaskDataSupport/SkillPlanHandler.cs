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


    public class SkillPlanHandler : BaseDataHandler, ISkillPlanHandler
    {
        private const string kGeneralFileName = "general_account_save.db";

        private ISkillSettingsProvider _skillSettingsProvider;
        private IGradeSettingsProvider _gradeSettingsProvider;

        private string _saveDirectoryPath;
        private string _dbFilePath;

        private static IDbConnection _dbConnection;

        public SkillPlanHandler(string directoryPath)
        {
            _saveDirectoryPath = directoryPath;
            var saveFilePath = directoryPath + kGeneralFileName;
            _dbFilePath = $"Data Source={saveFilePath}";

            _skillSettingsProvider = new SkillSettingsProvider();
            _gradeSettingsProvider = new GradeSettingsProvider();
        }

        public async UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true)
        {
            _dbConnection = OpenConnection(_dbFilePath);
            var result = await _gradeSettingsProvider.IsGradeEnabled(grade, _dbConnection, defaultIsEnable);
            CloseConnection(_dbConnection);
            return result;
        }

        public async UniTask SaveGradeState(int grade, bool isEnable)
        {
            _dbConnection = OpenConnection(_dbFilePath);
            await _gradeSettingsProvider.SaveGradeSettings(grade, isEnable, _dbConnection);
            CloseConnection(_dbConnection);
        }

        public async UniTask<SkillSettingsData> GetSkillSettings(int grade, SkillType skillType)
        {
            _dbConnection = OpenConnection(_dbFilePath);
            var result = await _skillSettingsProvider.GetSettingsByGradeAndSkill(grade, skillType, _dbConnection);
            CloseConnection(_dbConnection);
            return result;
        }

        public async UniTask SaveSkillSettings(SkillSettingsData settings)
        {
            _dbConnection = OpenConnection(_dbFilePath);
            await _skillSettingsProvider.SaveSkillSettings(settings, _dbConnection);
            CloseConnection(_dbConnection);
        }

        public async UniTask SaveSkillPlan(SkillSettingsData[] settings)
        {
            _dbConnection = OpenConnection(_dbFilePath);
            await _skillSettingsProvider.SaveSkillPlan(settings, _dbConnection);
            CloseConnection(_dbConnection);
        }

        protected async override UniTask TryCreateTables()
        {
            _dbConnection = OpenConnection(_dbFilePath);
            await _skillSettingsProvider.TryCreateTable(_dbConnection);
            CloseConnection (_dbConnection);
        }
    }






}

