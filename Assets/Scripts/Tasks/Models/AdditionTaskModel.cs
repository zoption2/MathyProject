using System.Collections.Generic;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public sealed class AdditionTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        private List<ExpressionElement> expression;
        private List<string> variants;
        private List<string> values;
        private List<string> operators;
        private ExpressionElement correctAnswer;
        private int correctAnswerIndex;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public ExpressionElement CorrectElement => correctAnswer;

        public AdditionTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            var valueOne = random.Next(minValue, maxValue);
            var valueTwo = random.Next(minValue, maxValue - valueOne);
            var result = valueOne + valueTwo;

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, valueOne),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Plus),
                new ExpressionElement(TaskElementType.Value, valueTwo),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Equal),
                new ExpressionElement(TaskElementType.Value, result, true)
            };

            values = new List<string>(3);
            values.Add(expression[0].Value);
            values.Add(expression[2].Value);
            values.Add(expression[4].Value);

            operators = new List<string>(2);
            operators.Add(expression[1].Value);
            operators.Add(expression[3].Value);

            correctAnswer = expression[4];

            variants = GetVariants(result, amountOfVariants, minValue, maxValue, out int indexOfCorrect);
            correctAnswerIndex = indexOfCorrect;
        }

        public override TaskData GetResult()
        {
            var result = new TaskData();
            result.TaskType = TaskType;
            result.ElementValues = values;
            result.OperatorValues = operators;
            result.VariantValues = variants;

            result.CorrectAnswerIndexes = new List<int>(1);
            result.CorrectAnswerIndexes.Add(correctAnswerIndex);
            return result;
        }
    }
}

