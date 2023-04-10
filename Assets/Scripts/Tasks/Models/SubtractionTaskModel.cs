using System.Collections.Generic;
using CustomRandom;
using System.Linq;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public class SubtractionTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;


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

            elements = new List<string>()
            {
                expression[0].Value,
                expression[2].Value,
                "?"
            };

            operators = new List<string>()
            {
                expression[1].Value,
                expression[3].Value
            };

            variants = GetVariants(result, amountOfVariants, minValue, maxValue, out int indexOfCorrect);
            correctAnswersIndexes = new List<int>()
            {
                indexOfCorrect
            };
        }
    }
}