using System.Collections.Generic;


namespace Mathy.Core.Tasks
{
    public sealed class MissingNumberTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;


        public MissingNumberTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();

            int startValue = random.Next(minValue, maxValue + 1);
            bool isPositive = startValue + (totalValues - 1) < maxValue;
            int unknownIndex = random.Next(0, totalValues + 1);
            int correctValue = 0;

            expression = new List<ExpressionElement>(totalValues);
            elements = new List<string>(totalValues);

            for (int i = 0; i < totalValues; )
            {
                bool isUnknown = i == unknownIndex;
                if (isUnknown)
                {
                    correctValue = startValue + i;
                }
                expression.Add(new ExpressionElement(TaskElementType.Value, startValue + i, isUnknown));
                elements.Add((startValue + i).ToString());
                i = isPositive ? i + 1 : i - 1;
            }

            operators = new List<string>();

            variants = GetVariants(correctValue, amountOfVariants, minValue, maxValue, out int indexOfCorrect);

            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }
    }
}

