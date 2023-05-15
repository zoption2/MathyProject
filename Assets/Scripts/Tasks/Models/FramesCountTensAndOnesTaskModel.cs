using System;
using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public class FramesCountTensAndOnesTaskModel : BaseTaskModel, ICountToAmountTaskModel
    {
        private const int kCorrectIndex = 0;
        private const int kTotalVariantsIndexes = 2;

        private int countToShow;
        private string countToShowStr;
        public int CountToShow => countToShow;


        public FramesCountTensAndOnesTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            countToShow = random.Next(minValue, minLimit + 1);
            
            countToShowStr = countToShow >= 10
                ? $"{countToShow / 10}+{countToShow % 10}"
                : $"{countToShow / 10}+{(countToShow % 10).ToString().PadLeft(1, '0')}";

            elements = new List<string>()
            {
                "?",
                countToShowStr,
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
                countToShowStr,
            };
        }
    }
}