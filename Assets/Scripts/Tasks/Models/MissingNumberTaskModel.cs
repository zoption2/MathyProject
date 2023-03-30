using System.Collections.Generic;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public sealed class MissingNumberTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        private List<ExpressionElement> expression;
        private List<string> variants;
        private List<string> values;
        private List<string> operators;
        private int correctAnswerIndex;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public ExpressionElement CorrectElement => throw new System.NotImplementedException();


        public MissingNumberTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();

            int startValue = random.Next(minValue, maxValue);
            bool isPositive = startValue + (totalValues - 1) < maxValue;
            int unknownIndex = random.Next(0, totalValues);
            int correctValue = 0;

            expression = new List<ExpressionElement>(totalValues);
            values = new List<string>(totalValues);

            for (int i = 0; i < totalValues; )
            {
                bool isUnknown = i == unknownIndex;
                if (isUnknown)
                {
                    correctValue = startValue + i;
                }
                expression.Add(new ExpressionElement(TaskElementType.Value, startValue + i, isUnknown));
                values.Add((startValue + i).ToString());
                i = isPositive ? i + 1 : i - 1;
            }

            operators = new List<string>();

            variants = GetVariants(correctValue, amountOfVariants, minValue, maxValue, out int indexOfCorrect);
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

