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
            elements = new List<string>();
            operators = new List<string>();

            if (isPositive)
            {
                for (int i = 0; i < totalValues; i++)
                {
                    var expressionValue = startValue + i;
                    bool isUnknown = i == unknownIndex;
                    if (isUnknown)
                    {
                        correctValue = expressionValue;
                    }
                    expression.Add(new ExpressionElement(TaskElementType.Value, expressionValue, isUnknown));
                }
            }
            else
            {
                for (int i = totalValues; i > 0; i--)
                {
                    var expressionValue = startValue + i;
                    bool isUnknown = i == unknownIndex;
                    if (isUnknown)
                    {
                        correctValue = expressionValue;
                    }
                    expression.Add(new ExpressionElement(TaskElementType.Value, expressionValue, isUnknown));
                }
            }


            GetExpressionValues(expression, out elements, out operators);

            variants = GetVariants(correctValue, amountOfVariants, minValue, maxValue, out int indexOfCorrect);

            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }
    }
}

