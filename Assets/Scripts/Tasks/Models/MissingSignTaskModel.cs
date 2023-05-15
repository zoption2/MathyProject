using System.Collections.Generic;

namespace Mathy.Core.Tasks
{
    public class MissingSignTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        private const int kVariantsCount = 2;
        private const int kPlusVariantIndex = 0;
        private const int kMinusVariantIndex = 1;

        private int correctVariantIndex;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;


        public MissingSignTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            int firstValue = random.Next(minValue, maxValue + 1);
            int secondValue;
            int result;

            bool isPlus;
            isPlus = random.Next(0, 2).ToBool();
            if (isPlus)
            {
                secondValue = random.Next(minValue, (maxValue - firstValue) + 1);
                result = firstValue + secondValue;
                correctVariantIndex = kPlusVariantIndex;
            }
            else
            {
                secondValue = random.Next(minValue, firstValue);
                result = firstValue - secondValue;
                correctVariantIndex = kMinusVariantIndex;
            }

            correctAnswersIndexes = new List<int>()
            {
                correctVariantIndex
            };

            variants = new List<string>(kVariantsCount)
            {
                ((char)ArithmeticSigns.Plus).ToString(),
                ((char)ArithmeticSigns.Minus).ToString()
            };

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, firstValue),
                new ExpressionElement(TaskElementType.Operator, variants[correctVariantIndex], true),
                new ExpressionElement(TaskElementType.Value, secondValue),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Equal),
                new ExpressionElement(TaskElementType.Value, result)
            };

            elements = new List<string>()
            {
                expression[0].Value,
                expression[2].Value,
                expression[4].Value
            };

            operators = new List<string>()
            {
                kUnknownSign,
                expression[3].Value
            };
        }
    }
}