using System.Collections.Generic;
using CustomRandom;
using System.Linq;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public sealed class AdditionTaskModel : BaseTaskModel, IDefaultTaskModel
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

            correctAnswer = expression.Last();

            variants = GetVariants(result, amountOfVariants, minValue, maxValue, out int indexOfCorrect);
            correctAnswerIndex = indexOfCorrect;
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

