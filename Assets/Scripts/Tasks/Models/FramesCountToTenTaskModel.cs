using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public interface IFramesCountToTenTaskModel : ITaskModel
    {
        int CorrectValue { get; }
        int CorrectIndex { get; }
        int FramesToShow { get; }
    }


    public class FramesCountToTenTaskModel : BaseTaskModel, IFramesCountToTenTaskModel
    {
        private const int kMaxValueLimit = 10;
        private const int kMinValueLimit = 1;
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;

        public int CorrectValue { get; private set; }
        public int FramesToShow { get; private set; }
        public int CorrectIndex { get; private set; }

        public FramesCountToTenTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            CorrectValue = random.Next(kMinValueLimit, kMaxValueLimit + 1);
            FramesToShow = kMaxValueLimit - CorrectValue;

            CorrectIndex = CorrectValue - 1;
            correctAnswersIndexes = new List<int>()
            {
                CorrectIndex
            };

            elements = new List<string>()
            {
                FramesToShow.ToString(),
                "?",
                kMaxValueLimit.ToString()
            };

            operators = new List<string>()
            {
                "+", "="
            };

            variants = new List<string>()
            {
                "1","2","3","4","5","6","7","8","9","10"
            };
        }
    }
}