using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public class FramesCountTaskModel : BaseTaskModel, ICountToAmountTaskModel
    {
        private const int kCorrectIndex = 0;
        private const int kTotalVariantsIndexes = 2;

        private int countToShow;
        public int CountToShow => countToShow;


        public FramesCountTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            countToShow = random.Next(minValue, minLimit + 1);

            elements = new List<string>()
            {
                "?",
                countToShow.ToString(),
            };

            correctAnswersIndexes = new List<int>()
            {
                kCorrectIndex
            };

            operators = new List<string>()
            {
                "="
            };

            variants = new List<string>()
            {
                countToShow.ToString(),
            };
        }
    }
}