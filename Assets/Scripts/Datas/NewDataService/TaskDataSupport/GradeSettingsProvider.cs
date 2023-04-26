using Cysharp.Threading.Tasks;
using Dapper;
using Mono.Data.Sqlite;


namespace Mathy.Services.Data
{
    public interface IGradeSettingsProvider : IDataProvider
    {
        UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true);
        UniTask SaveGradeSettings(int grade, bool isEnable);
    }


    public class GradeSettingsProvider : BaseDataProvider, IGradeSettingsProvider
    {
        public GradeSettingsProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public async UniTask<bool> IsGradeEnabled(int grade, bool defaultIsEnable = true)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
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
        }

        public async UniTask SaveGradeSettings(int grade, bool isEnable)
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                var requestModel = new GradeTableModel() { Grade = grade, IsEnable = isEnable };
                var model = await connection.QueryFirstOrDefaultAsync<GradeTableModel>(GradesTableRequests.SelectGradeQuery, requestModel);
                var query = model != null ? GradesTableRequests.UpdateGradeQuery : GradesTableRequests.InsertGradeQuery;
                await connection.ExecuteAsync(query, requestModel);
            }
        }

        public async override UniTask TryCreateTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(GradesTableRequests.TryCreateTableQuery);
            }
        }

        public async override UniTask DeleteTable()
        {
            using (var connection = new SqliteConnection(_dbFilePath))
            {
                await connection.ExecuteAsync(GradesTableRequests.DeleteTable);
            }
        }
    }
}

