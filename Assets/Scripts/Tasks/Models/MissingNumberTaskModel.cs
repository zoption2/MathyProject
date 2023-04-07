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
            int unknownIndex = random.Next(0, totalValues);
            int correctValue = 0;

            expression = new List<ExpressionElement>(totalValues);
            elements = new List<string>();
            operators = new List<string>();

            if (isPositive)
            {
                for (int i = 0; i < totalValues; i++)
                {
                    bool isUnknown = i == unknownIndex;
                    if (isUnknown)
                    {
                        correctValue = startValue + i;
                    }
                    expression.Add(new ExpressionElement(TaskElementType.Value, startValue + i, isUnknown));
                }
            }
            else
            {
                for (int i = 0; i < totalValues; i++)
                {
                    bool isUnknown = i == unknownIndex;
                    if (isUnknown)
                    {
                        correctValue = startValue - i;
                    }
                    expression.Add(new ExpressionElement(TaskElementType.Value, startValue - i, isUnknown));
                }
            }
            UnityEngine.Debug.Log("Start value: " + startValue.ToString());
            UnityEngine.Debug.Log("Correct value: " + correctValue.ToString());
            UnityEngine.Debug.Log("Unknown index: " + unknownIndex.ToString());
            UnityEngine.Debug.Log("Is positive: " + isPositive.ToString());

            GetExpressionValues(expression, out elements, out operators);

            variants = GetVariants(correctValue, amountOfVariants, minValue, maxValue, out int indexOfCorrect);

            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }
    }
}

