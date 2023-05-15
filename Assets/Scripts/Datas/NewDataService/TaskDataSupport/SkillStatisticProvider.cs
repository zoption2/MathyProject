using System;
using Cysharp.Threading.Tasks;
using Mathy.Data;
using Mono.Data.Sqlite;

namespace Mathy.Services.Data
{
    public interface ISkillStatisticProvider : IDataProvider
    {
        UniTask<SkillStatisticData> GetSkillStatistic(SkillType skillType, int grade);
        UniTask UpdateSkillStatistic(SkillStatisticData data);
    }


    public class SkillStatisticProvider : BaseDataProvider, ISkillStatisticProvider
    {
        public SkillStatisticProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<SkillStatisticData> GetSkillStatistic(SkillType skillType, int grade)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestData = new SkillStatisticData() { Skill = skillType, Grade = grade };
                var requestModel = requestData.ConvertToModel();

                var query = SkillStatisticRequests.GetCountQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.SkillIndex), requestModel.SkillIndex);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Grade), requestModel.Grade);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);
               
                if (count == 0)
                {
                    return requestData;
                }

                query = SkillStatisticRequests.SelectBySkillAndGradeQuery;
                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.SkillIndex), requestModel.SkillIndex);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Grade), requestModel.Grade);
                var reader = await command.ExecuteReaderAsync();

                var resultModel = requestModel;
                while (await reader.ReadAsync())
                {
                    resultModel.ID = Convert.ToInt32(reader[0]);
                    resultModel.Skill = Convert.ToString(reader[1]);
                    resultModel.SkillIndex = Convert.ToInt32(reader[2]);
                    resultModel.Total = Convert.ToInt32(reader[3]);
                    resultModel.Correct = Convert.ToInt32(reader[4]);
                    resultModel.Rate = Convert.ToInt32(reader[5]);
                    resultModel.Duration = Convert.ToDouble(reader[6]);
                    resultModel.Grade = Convert.ToInt32(reader[7]);
                }
                reader.Close();
                connection.Close();
                connection.Dispose();

                var result = resultModel.ConvertToData();
                return result;
            }
        }

        //public async UniTask UpdateSkillStatistic(SkillType skillType, int grade, bool isCorrect)
        //{
        //    using (var connection = new SqliteConnection(_dbFilePath))
        //    {
        //        connection.Open();
        //        var requestData = new SkillStatisticData() { Skill = skillType, Grade = grade, };
        //        var requestModel = requestData.ConvertToModel();
        //        requestModel.IsCorrectRequest = isCorrect;

        //        var query = SkillStatisticRequests.GetCountQuery;
        //        SqliteCommand command = new SqliteCommand(query, connection);
        //        command.Parameters.AddWithValue(nameof(SkillStatisticModel.SkillIndex), requestModel.SkillIndex);
        //        command.Parameters.AddWithValue(nameof(SkillStatisticModel.Grade), requestModel.Grade);
        //        var scaler = await command.ExecuteScalarAsync();
        //        var count = Convert.ToInt32(scaler);

        //        if (count == 0)
        //        {
        //            query = SkillStatisticRequests.InsertQuery;
        //            command = new SqliteCommand(query, connection);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Skill), requestModel.Skill);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.SkillIndex), requestModel.SkillIndex);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Total), requestModel.Total);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Correct), requestModel.Correct);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Rate), requestModel.Rate);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Duration), requestModel.Duration);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Grade), requestModel.Grade);
        //            await command.ExecuteNonQueryAsync();
        //        }
        //        else
        //        {
        //            query = SkillStatisticRequests.UpdateIncrementQuery;
        //            command = new SqliteCommand(query, connection);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.SkillIndex), requestModel.SkillIndex);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Grade), requestModel.Grade);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.IsCorrectRequest), requestModel.IsCorrectRequest);
        //            command.Parameters.AddWithValue(nameof(SkillStatisticModel.Duration), requestModel.Duration);
        //            await command.ExecuteNonQueryAsync();
        //        }

        //        connection.Close();
        //        connection.Dispose();
        //    }
        //}

        public async UniTask UpdateSkillStatistic(SkillStatisticData data)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var requestModel = data.ConvertToModel();

                var query = SkillStatisticRequests.GetCountQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.SkillIndex), requestModel.SkillIndex);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Grade), requestModel.Grade);
                var scaler = await command.ExecuteScalarAsync();
                var count = Convert.ToInt32(scaler);

                query = count == 0
                    ? SkillStatisticRequests.InsertQuery
                    : SkillStatisticRequests.UpdateQuery;

                command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Skill), requestModel.Skill);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.SkillIndex), requestModel.SkillIndex);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Total), requestModel.Total);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Correct), requestModel.Correct);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Rate), requestModel.Rate);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Duration), requestModel.Duration);
                command.Parameters.AddWithValue(nameof(SkillStatisticModel.Grade), requestModel.Grade);
                await command.ExecuteNonQueryAsync();

                connection.Close();
                connection.Dispose();
            }
        }


        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                connection.Open();
                var query = SkillStatisticRequests.TryCreateTableQuery;
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
                var query = SkillStatisticRequests.DeleteTableQuery;
                SqliteCommand command = new SqliteCommand(query, connection);
                await command.ExecuteNonQueryAsync();
                connection.Close();
                connection.Dispose();
            }
        }
    }

}

