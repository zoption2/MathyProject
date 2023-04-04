using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public class ComparisonExpressionsTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;

        private List<ArithmeticSigns> signs;

        public ComparisonExpressionsTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var elementOne = random.Next(minValue, maxValue);
            var elementTwo = random.Next(minValue, maxValue);
            var result = ((char)MathOperations.Compare(elementOne, elementTwo)).ToString();

            string expressionOne = MathOperations.BuildExpressionFromValue(elementOne, minValue, maxValue);
            string expressionTwo = MathOperations.BuildExpressionFromValue(elementTwo, minValue, maxValue);
            
            signs = new List<ArithmeticSigns>()
            {
                ArithmeticSigns.LessThan,
                ArithmeticSigns.Equal,
                ArithmeticSigns.GreaterThan
            };

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, expressionOne),
                new ExpressionElement(TaskElementType.Operator, result, true),
                new ExpressionElement(TaskElementType.Value, expressionTwo)
            };

            elements = new List<string>()
            {
                expressionOne,
                expressionTwo
            };

            operators = new List<string>()
            {
                result
            };

            variants = GetVariants(result, signs.Count, out int indexOfCorrect);

            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }

        protected List<string> GetVariants(string correctValue, int amountOfVariants, out int correctValueIndex)
        {
            var results = new List<string>(amountOfVariants);
            for (int i = 0; i < amountOfVariants; i++)
            {
                var variant = signs[i];
                results.Add(((char)variant).ToString());
            }
            correctValueIndex = GetIndexOfValueFromList(correctValue, results);
            return results;
        }
    }
}