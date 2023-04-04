using System.Collections.Generic;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public interface IComparisonBothMissingElementsTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Variants { get; }
        List<string> CorrectVariants { get; }
        TaskData UpdateModelBasedOnPlayerChoice(string inputedValue);
    }


    public class ComparisonBothMissingElementsTaskModel : BaseTaskModel, IComparisonBothMissingElementsTaskModel
    {
        private List<string> correctVariants;
        private ArithmeticSigns selectedSign;
        private List<int> integerVariants;
        private TaskData taskData;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public List<string> CorrectVariants => correctVariants;


        public ComparisonBothMissingElementsTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var signs = new ArithmeticSigns[]
            {
                ArithmeticSigns.LessThan,
                ArithmeticSigns.GreaterThan,
                ArithmeticSigns.Equal
            };

            selectedSign = signs[random.Next(signs.Length)];
            var selectedSignText = ((char)selectedSign).ToString();

            variants = MathOperations.GetRandomVariants(minValue, maxValue, amountOfVariants);
            variants = MathOperations.DublicateRandomValueAndShake(variants);
            integerVariants = MathOperations.ConvertStringsToInt(variants);

            expression = new List<ExpressionElement>()
            {
                new ExpressionElement(TaskElementType.Value, 0, true),
                new ExpressionElement(TaskElementType.Operator, selectedSignText),
                new ExpressionElement(TaskElementType.Value, 0, true)
            };

            GetExpressionValues(expression, out elements, out operators);
        }

        public override TaskData GetResult()
        {
            var result = new TaskData();
            result.TaskType = TaskType;
            result.ElementValues = elements;
            result.OperatorValues = operators;
            result.VariantValues = variants;
            taskData = result;
            return result;
        }

        public TaskData UpdateModelBasedOnPlayerChoice(string inputedValue)
        {
            var inputedValueIndex = -1;
            var intValue = int.Parse(inputedValue);

            for (int i = 0, j = variants.Count; i < j; i++)
            {
                if (variants[i].Equals(inputedValue))
                {
                    inputedValueIndex = i;
                    break;
                }
            }

            if (inputedValueIndex == -1)
            {
                throw new System.ArgumentOutOfRangeException(
                    string.Format("There is no variant {0} detected among Variants", inputedValue)
                    );
            }

            switch (selectedSign)
            {
                case ArithmeticSigns.GreaterThan:
                    correctAnswersIndexes = MathOperations.GetCorrectIndexesWithLessThen(intValue, integerVariants);
                    break;
                case ArithmeticSigns.LessThan:
                    correctAnswersIndexes = MathOperations.GetCorrectIndexesWithGreaterThen(intValue, integerVariants);
                    break;
                case ArithmeticSigns.Equal:
                    correctAnswersIndexes = MathOperations.GetCorrectIndexesWithEqualTo(intValue, integerVariants);
                    break;
            }

            correctVariants = new List<string>();
            for (int i = 0, j = correctAnswersIndexes.Count; i < j; i++)
            {
                var index = correctAnswersIndexes[i];
                correctVariants.Add(variants[index]);
            }

            UpdateTaskData();
            return taskData;
        }

        private void UpdateTaskData()
        {
            taskData.CorrectAnswerIndexes = correctAnswersIndexes;
        }

    }
}

