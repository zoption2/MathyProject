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
        private List<ExpressionElement> expression;
        private List<string> variants;
        private List<string> elements;
        private List<string> operators;
        private ExpressionElement correctAnswer;
        private int correctAnswerIndex;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public ExpressionElement CorrectElement => correctAnswer;
        public List<ArithmeticSigns> Signs =
                new List<ArithmeticSigns>() { ArithmeticSigns.LessThan, ArithmeticSigns.Equal, ArithmeticSigns.GreaterThan };

        public ComparisonTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            var elementOne = random.Next(minValue, maxValue);
            var elementTwo = random.Next(minValue, maxValue);
            var result = ((char)MathOperations.Compare(elementOne, elementTwo)).ToString();

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, elementOne),
                new ExpressionElement(TaskElementType.Operator, result, true),
                new ExpressionElement(TaskElementType.Value, elementTwo)
            };

            elements = new List<string>(2);
            elements.Add(expression[0].Value);
            elements.Add(expression[2].Value);

            operators = new List<string>(1);
            operators.Add(expression[1].Value);

            correctAnswer = expression[1];

            variants = GetVariants(result, Signs.Count, out int indexOfCorrect);
            correctAnswerIndex = indexOfCorrect;
        }

        protected List<string> GetVariants(string correctValue, int amountOfVariants, out int correctValueIndex)
        {
            var results = new List<string>(amountOfVariants);
            //results.Add(correctValue.ToString());
            for (int i = 0; i < amountOfVariants; i++)
            {
                var variant = Signs[i];
                results.Add(((char)variant).ToString());
            }
            correctValueIndex = GetIndexOfValueFromList(correctValue, results);
            return results;
        }

        public override TaskData GetResult()
        {
            var result = new TaskData();
            result.TaskType = TaskType;
            result.ElementValues = elements;
            result.OperatorValues = operators;
            result.VariantValues = variants;

            result.CorrectAnswerIndexes = new List<int>(1);
            result.CorrectAnswerIndexes.Add(correctAnswerIndex);
            return result;
        }
    }
}