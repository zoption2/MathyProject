using Cysharp.Threading.Tasks;
using Dapper;
using Mathy.Data;
using Mono.Data.Sqlite;


namespace Mathy.Services.Data
{
    public interface ISkillSettingsProvider : IDataProvider
    {
        UniTask<SkillSettingsData> GetSettingsByGradeAndSkill(int grade, SkillType skillType);
        UniTask SaveSkillSettings(SkillSettingsData data);
        UniTask SaveSkillPlan(SkillSettingsData[] data);
    }


    public class SkillSettingsProvider : BaseDataProvider, ISkillSettingsProvider, IDataProvider
    {
        public SkillSettingsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<SkillSettingsData> GetSettingsByGradeAndSkill(int grade, SkillType skillType)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var requestData = new SkillSettingsData() { Grade = grade, Skill = skillType };
                var requestModel = requestData.ConvertToTableModel();
                var model = await connection.QueryFirstOrDefaultAsync<SkillPlanTableModel>
                    (SkillPlanTableRequests.SelectByGradeAndSkillQuery, requestModel);
                if (model == null)
                {
                    await connection.ExecuteAsync(SkillPlanTableRequests.InsertEntryQuery, requestModel);
                    return requestData;
                }
                var result = model.ConvertToData();
                return result;
            }
        }

        public async UniTask SaveSkillPlan(SkillSettingsData[] data)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                for (int i = 0, j = data.Length; i < j; i++)
                {
                    var requestData = data[i];
                    var requestModel = requestData.ConvertToTableModel();
                    var exists = await connection.QueryFirstOrDefaultAsync<SkillPlanTableModel>(SkillPlanTableRequests.SelectByGradeAndSkillQuery, requestModel);
                    var query = exists != null ? SkillPlanTableRequests.UpdateSkillQuery : SkillPlanTableRequests.InsertEntryQuery;
                    await connection.ExecuteAsync(query, requestModel);
                }
            }
        }

        public async UniTask SaveSkillSettings(SkillSettingsData data)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var requestModel = data.ConvertToTableModel();
                var exists = await connection.QueryFirstOrDefaultAsync<SkillPlanTableModel>(SkillPlanTableRequests.SelectByGradeAndSkillQuery, requestModel);
                var query = exists != null ? SkillPlanTableRequests.UpdateSkillQuery : SkillPlanTableRequests.InsertEntryQuery;
                await connection.ExecuteAsync(query, requestModel);
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(SkillPlanTableRequests.TryCreateTableQuery);
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(SkillPlanTableRequests.DeleteTable);
            }
        }
    }
}

