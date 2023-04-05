using System.Collections.Generic;
using CustomRandom;
using System.Linq;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public class SubtractionTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        private int correctAnswerIndex;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public ExpressionElement CorrectElement => throw new System.NotImplementedException();

        public SubtractionTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            var elementOne = random.Next(minValue, maxValue);
            var elementTwo = random.Next(minValue, elementOne);
            var result = elementOne - elementTwo;

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, elementOne),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Minus),
                new ExpressionElement(TaskElementType.Value, elementTwo),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Equal),
                new ExpressionElement(TaskElementType.Value, result, true)
            };

            elements = new List<string>(3);
            elements.Add(expression[0].Value);
            elements.Add(expression[2].Value);
            elements.Add(expression[4].Value);

            operators = new List<string>(2);
            operators.Add(expression[1].Value);
            operators.Add(expression[3].Value);

            variants = GetVariants(result, amountOfVariants, minValue, maxValue, out int indexOfCorrect);
            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }
    }
}