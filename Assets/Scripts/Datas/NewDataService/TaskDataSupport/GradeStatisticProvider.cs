using System;
using Cysharp.Threading.Tasks;

namespace Mathy.Services.Data
{
    public class GradeStatisticProvider : BaseDataProvider
    {
        public GradeStatisticProvider(string dbFilePath) : base(dbFilePath)
        {
        }

        public override UniTask TryCreateTable()
        {
            throw new NotImplementedException();
        }
    }
}

