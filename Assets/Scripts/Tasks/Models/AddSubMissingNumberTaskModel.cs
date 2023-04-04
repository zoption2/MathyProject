using System.Collections.Generic;
using CustomRandom;
using System.Linq;

namespace Mathy.Core.Tasks
{
    public sealed class AddSubMissingNumberTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;

        public AddSubMissingNumberTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var fastRandom = new FastRandom();
            var random = new System.Random();
            var elementValues = fastRandom.GetRandomElementValues(totalValues, maxValue);
            int result;
            int unknownIndex = random.Next(0, totalValues);
            bool isAddition = fastRandom.TossACoin();

            elements = new List<string>();
            operators = new List<string>();
            expression = new List<ExpressionElement>();

            if (isAddition)
            {
                result = elementValues.Sum();
                for (int i = 0; i < elementValues.Count; i++)
                {
                    expression.Add(new ExpressionElement(TaskElementType.Value, elementValues[i], i == unknownIndex));
                    expression.Add(new ExpressionElement(TaskElementType.Operator,
                        i == totalValues - 1 ? (char)ArithmeticSigns.Equal : (char)ArithmeticSigns.Plus));
                }
                expression.Add(new ExpressionElement(TaskElementType.Value, result));
            }
            else
            {
                int elementOne = random.Next(minValue, maxValue);
                int elementTwo = random.Next(minValue, elementOne);
                elementValues = new List<int>(2) { elementOne, elementTwo };
                result = elementOne - elementTwo;

                for (int i = 0; i < 2; i++)
                {
                    expression.Add(new ExpressionElement(TaskElementType.Value, elementValues[i], i == unknownIndex));
                    expression.Add(new ExpressionElement(TaskElementType.Operator,
                        i == totalValues - 1 ? (char)ArithmeticSigns.Equal : (char)ArithmeticSigns.Minus));
                }
                expression.Add(new ExpressionElement(TaskElementType.Value, result));
            }

            GetExpressionValues(expression, out elements, out operators);
            variants = GetVariants(elementValues[unknownIndex], amountOfVariants, minValue, maxValue, out int indexOfCorrect);
            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }
    }
}