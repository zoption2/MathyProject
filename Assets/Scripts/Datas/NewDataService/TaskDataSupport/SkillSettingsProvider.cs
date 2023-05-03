using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mono.Data.Sqlite;
using System;
using System.Data;

namespace Mathy.Services.Data
{
    public interface ISkillSettingsProvider : IDataProvider
    {
        UniTask<SkillSettingsData> GetSettingsByGradeAndSkillAsync(int grade, SkillType skillType);
        SkillSettingsData GetSettingsByGradeAndSkill(int grade, SkillType skillType);
        UniTask SaveSkillSettings(SkillSettingsData data);
        UniTask SaveSkillPlan(SkillSettingsData[] data);
    }


    public class SkillSettingsProvider : BaseDataProvider, ISkillSettingsProvider, IDataProvider
    {
        public SkillSettingsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public SkillSettingsData GetSettingsByGradeAndSkill(int grade, SkillType skillType)
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
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                    IDataReader reader = command.ExecuteReader();

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
                        query = SkillPlanTableRequests.InsertEntryQuery;
                        command = new SqliteCommand(query, connection);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.IsEnabled), requestModel.IsEnabled);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Value), requestModel.Value);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MinValue), requestModel.MinValue);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MaxValue), requestModel.MaxValue);
                        command.ExecuteNonQuery();

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

        public async UniTask<SkillSettingsData> GetSettingsByGradeAndSkillAsync(int grade, SkillType skillType)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                try
                {
                    connection.Open();
                    var requestData = new SkillSettingsData() { Grade = grade, Skill = skillType };
                    var requestModel = requestData.ConvertToTableModel();

                    string query = SkillPlanTableRequests.GetCountQuery;
                    SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                    var scaler = await command.ExecuteScalarAsync();
                    var count = Convert.ToInt32(scaler);

                    var result = requestData;

                    if (count == 0)
                    {
                        query = SkillPlanTableRequests.InsertEntryQuery;
                        command = new SqliteCommand(query, connection);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.IsEnabled), requestModel.IsEnabled);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Value), requestModel.Value);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MinValue), requestModel.MinValue);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MaxValue), requestModel.MaxValue);
                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        query = SkillPlanTableRequests.SelectByGradeAndSkillQuery;
                        command = new SqliteCommand(query, connection);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                        command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);

                        var reader = await command.ExecuteReaderAsync();
                        var resultModel = new SkillPlanTableModel();
                        while (await reader.ReadAsync())
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
                        result = resultModel.ConvertToData();

                    }
                    connection.Close();
                    connection.Dispose();
                    return result;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Some GetSettingsByGradeAndSkillTEST error: " + e.ToString());
                    throw;
                }
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
                    var query = SkillPlanTableRequests.GetCountQuery;
                    SqliteCommand command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                    var scaler = await command.ExecuteScalarAsync();
                    var count = Convert.ToInt32(scaler);

                    query = count == 0
                        ? SkillPlanTableRequests.InsertEntryQuery
                        : SkillPlanTableRequests.UpdateSkillQuery;

                    command = new SqliteCommand(query, connection);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.IsEnabled), requestModel.IsEnabled);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Value), requestModel.Value);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MinValue), requestModel.MinValue);
                    command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MaxValue), requestModel.MaxValue);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async UniTask SaveSkillSettings(SkillSettingsData data)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestModel = data.ConvertToTableModel();
                var query = SkillPlanTableRequests.GetCountQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                query = count == 0
                    ? SkillPlanTableRequests.InsertEntryQuery
                    : SkillPlanTableRequests.UpdateSkillQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Grade), requestModel.Grade);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Skill), requestModel.Skill);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.IsEnabled), requestModel.IsEnabled);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.Value), requestModel.Value);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MinValue), requestModel.MinValue);
                command.Parameters.AddWithValue(nameof(SkillPlanTableModel.MaxValue), requestModel.MaxValue);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = SkillPlanTableRequests.TryCreateTableQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = SkillPlanTableRequests.DeleteTable;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }
}

