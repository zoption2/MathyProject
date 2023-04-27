using Cysharp.Threading.Tasks;
using Dapper;
using Mathy.Data;
using Mono.Data.Sqlite;
using System;
using System.Data;

namespace Mathy.Services.Data
{
    public interface ISkillSettingsProvider : IDataProvider
    {
        UniTask<SkillSettingsData> GetSettingsByGradeAndSkillAsync(int grade, SkillType skillType);
        SkillSettingsData GetSettingsByGradeAndSkillTEST(int grade, SkillType skillType);
        SkillSettingsData GetSettingsByGradeAndSkill(int grade, SkillType skillType);
        UniTask SaveSkillSettings(SkillSettingsData data);
        UniTask SaveSkillPlan(SkillSettingsData[] data);
    }


    public class SkillSettingsProvider : BaseDataProvider, ISkillSettingsProvider, IDataProvider
    {
        public SkillSettingsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<SkillSettingsData> GetSettingsByGradeAndSkillAsync(int grade, SkillType skillType)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
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


        public SkillSettingsData GetSettingsByGradeAndSkillTEST(int grade, SkillType skillType)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                try
                {
                    connection.Open();
                    var requestData = new SkillSettingsData() { Grade = grade, Skill = skillType };
                    var requestModel = requestData.ConvertToTableModel();
                    SkillPlanTableModel resultModel = null;

                    string query = SkillPlanTableRequests.SelectByGradeAndSkillQuery;
                    SqliteCommand command = new SqliteCommand(query, connection);
                    var gradeParam = $@"@{nameof(SkillPlanTableModel.Grade)}";
                    command.Parameters.AddWithValue(gradeParam, requestModel.Grade);
                    var skillParam = $@"@{nameof(SkillPlanTableModel.Skill)}";
                    command.Parameters.AddWithValue(skillParam, requestModel.Skill);
                    IDataReader reader = command.ExecuteReader();
                    //resultModel = reader.DoRead();
                    resultModel = new SkillPlanTableModel();
                    while (reader.Read())
                    {
                        resultModel.Id = Convert.ToInt32(reader[0]);
                        resultModel.Grade = Convert.ToInt32(reader[1]);
                        resultModel.Skill = Convert.ToString(reader[2]);
                        resultModel.IsEnabled = Convert.ToBoolean(reader[3]);
                        resultModel.Value = Convert.ToInt32(reader[4]);
                        resultModel.MinValue = Convert.ToInt32(reader[5]);
                        resultModel.MaxValue = Convert.ToInt32(reader[6]);
                    }
                    reader.Close();
                    if (resultModel.Id == 0)
                    {
                        connection.Execute(SkillPlanTableRequests.InsertEntryQuery, requestModel);
                        return requestData;
                    }
                    
                    connection.Close();
                    connection.Dispose();

                    var result = resultModel.ConvertToData();
                    return result;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Some GetSettingsByGradeAndSkillTEST error: " + e.ToString());
                    throw;
                }

            }
        }

        public SkillSettingsData GetSettingsByGradeAndSkill(int grade, SkillType skillType)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new SkillSettingsData() { Grade = grade, Skill = skillType };
                var requestModel = requestData.ConvertToTableModel();
                var model = connection.QueryFirstOrDefault<SkillPlanTableModel>
                    (SkillPlanTableRequests.SelectByGradeAndSkillQuery, requestModel);
                if (model == null)
                {
                    connection.Execute(SkillPlanTableRequests.InsertEntryQuery, requestModel);
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
                connection.Open();
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
                connection.Open();
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
                connection.Open();
                await connection.ExecuteAsync(SkillPlanTableRequests.TryCreateTableQuery);
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                await connection.ExecuteAsync(SkillPlanTableRequests.DeleteTable);
            }
        }
    }
}

