using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using Mathy.Data;
using System;
using Mathy;

namespace Mathy.Core.Tasks
{
    public class ComparisonTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;

        public List<ArithmeticSigns> Signs =
                new List<ArithmeticSigns>() { ArithmeticSigns.LessThan, ArithmeticSigns.Equal, ArithmeticSigns.GreaterThan };

        public ComparisonTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var elementOne = random.Next(minValue, maxValue);
            var elementTwo = random.Next(minValue, maxValue);
            var result = ((char)MathOperations.Compare(elementOne, elementTwo)).ToString();

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, elementOne),
                new ExpressionElement(TaskElementType.Operator, result, true),
                new ExpressionElement(TaskElementType.Value, elementTwo)
            };

            elements = new List<string>()
            {
                expression[0].Value,
                expression[2].Value
            };

            operators = new List<string>()
            {
                "?"
            };

            variants = GetVariants(result, Signs.Count, out int indexOfCorrect);

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
                var variant = Signs[i];
                results.Add(((char)variant).ToString());
            }
            correctValueIndex = GetIndexOfValueFromList(correctValue, results);
            return results;
        }
    }
}