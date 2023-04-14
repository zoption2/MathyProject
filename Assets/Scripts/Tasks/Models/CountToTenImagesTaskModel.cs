using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public interface ICountToAmountTaskModel : ITaskModel
    {
        int CountToShow { get; }
    }


    public class CountToTenImagesTaskModel : BaseTaskModel, ICountToAmountTaskModel
    {
        private const int kMaxValueLimit = 10;

        private int countToShow;

        public int CountToShow => countToShow;


        public CountToTenImagesTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            countToShow = random.Next(1 , kMaxValueLimit + 1);

            elements = new List<string>()
            {
                "?",
                countToShow.ToString(),
            };

            correctAnswersIndexes = new List<int>()
            {
                countToShow - 1
            };

            operators = new List<string>()
            {
                "="
            };

            variants = new List<string>()
            {
                "1","2","3","4","5","6","7","8","9","10"
            };
        }
    }
}