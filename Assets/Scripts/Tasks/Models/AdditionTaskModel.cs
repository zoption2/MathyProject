using System.Collections.Generic;
using CustomRandom;
using System.Linq;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public sealed class AdditionTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;


        public AdditionTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new FastRandom();
            var elementValues = random.GetRandomElementValues(totalValues, maxValue);
            var result = elementValues.Sum();

            elements = new List<string>();
            operators = new List<string>();
            expression = new List<ExpressionElement>();

            for (int i = 0; i < elementValues.Count; i++)
            {
                expression.Add(new ExpressionElement(TaskElementType.Value, elementValues[i]));
                expression.Add(new ExpressionElement(TaskElementType.Operator,
                    i == totalValues - 1 ? (char)ArithmeticSigns.Equal : (char)ArithmeticSigns.Plus));
            }
            expression.Add(new ExpressionElement(TaskElementType.Value, result, true));

            GetExpressionValues(expression, out elements, out operators);
            variants = GetVariants(result, amountOfVariants, minValue, maxValue, out int indexOfCorrect);

            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }
    }
}

