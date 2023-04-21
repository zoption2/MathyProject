using Cysharp.Threading.Tasks;
using Dapper;
using Mathy.Data;
using System.Data;
using UnityEditor.MemoryProfiler;

namespace Mathy.Services.Data
{
    public interface ISkillSettingsProvider : IDataProvider
    {
        UniTask<SkillSettingsData> GetSettingsByGradeAndSkill(int grade, SkillType skillType, IDbConnection connection);
        UniTask SaveSkillSettings(SkillSettingsData data, IDbConnection connection);
        UniTask SaveSkillPlan(SkillSettingsData[] data, IDbConnection connection);
    }


    public class SkillSettingsProvider : ISkillSettingsProvider, IDataProvider
    {
        public async UniTask<SkillSettingsData> GetSettingsByGradeAndSkill(int grade, SkillType skillType, IDbConnection connection)
        {
            var requestData = new SkillSettingsData() {Grade = grade, Skill = skillType};
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

        public async UniTask SaveSkillPlan(SkillSettingsData[] data, IDbConnection connection)
        {
            UnityEngine.Debug.Log("SaveSkillPlan started!");
            for (int i = 0, j = data.Length; i < j; i++)
            {
                var requestData = data[i];
                var requestModel = requestData.ConvertToTableModel();
                var exists = await connection.QueryFirstOrDefaultAsync<SkillPlanTableModel>(SkillPlanTableRequests.SelectByGradeAndSkillQuery, requestModel);
                var query = exists != null ? DailyModeTableRequests.UpdateDailyQuery : DailyModeTableRequests.InsertDailyQuery;
                await connection.ExecuteAsync(query, requestModel);
            }
            UnityEngine.Debug.Log("SaveSkillPlan completed!");
        }

        public async UniTask SaveSkillSettings(SkillSettingsData data, IDbConnection connection)
        {
            var requestModel = data.ConvertToTableModel();
            var exists = await connection.QueryFirstOrDefaultAsync<SkillPlanTableModel>(SkillPlanTableRequests.SelectByGradeAndSkillQuery, requestModel);
            var query = exists != null ? DailyModeTableRequests.UpdateDailyQuery : DailyModeTableRequests.InsertDailyQuery;
            await connection.ExecuteAsync(query, requestModel);
        }

        public async UniTask TryCreateTable(IDbConnection connection)
        {
            await connection.ExecuteAsync(SkillPlanTableRequests.TryCreateTableQuery, connection);
        }
    }
}

