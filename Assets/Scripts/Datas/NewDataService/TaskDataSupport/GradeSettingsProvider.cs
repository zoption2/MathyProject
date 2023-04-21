using Cysharp.Threading.Tasks;
using Dapper;
using System.Data;

namespace Mathy.Services.Data
{
    public interface IGradeSettingsProvider : IDataProvider
    {
        UniTask<bool> IsGradeEnabled(int grade, IDbConnection connection, bool defaultIsEnable = true);
        UniTask SaveGradeSettings(int grade, bool isEnable, IDbConnection connection);
    }


    public class GradeSettingsProvider : IGradeSettingsProvider
    {
        public async UniTask<bool> IsGradeEnabled(int grade, IDbConnection connection, bool defaultIsEnable = true)
        {
            var requestModel = new GradeTableModel() { Grade = grade, IsEnable = defaultIsEnable };
            var model = await connection.QueryFirstOrDefaultAsync<GradeTableModel>(GradesTableRequests.SelectGradeQuery, requestModel);
            if (model == null)
            {
                await connection.ExecuteAsync(GradesTableRequests.InsertGradeQuery, requestModel);
                return requestModel.IsEnable;
            }

            return model.IsEnable;
        }

        public async UniTask SaveGradeSettings(int grade, bool isEnable, IDbConnection connection)
        {
            var requestModel = new GradeTableModel() { Grade = grade, IsEnable = isEnable };
            var model = await connection.QueryFirstOrDefaultAsync<GradeTableModel>(GradesTableRequests.SelectGradeQuery, requestModel);
            var query = model != null ? GradesTableRequests.UpdateGradeQuery : GradesTableRequests.InsertGradeQuery;
            await connection.ExecuteAsync(query, requestModel);
        }

        public async UniTask TryCreateTable(IDbConnection connection)
        {
            await connection.ExecuteAsync(GradesTableRequests.TryCreateTableQuery, connection);
        }
    }
}

