using System.Collections.Generic;
using System.Linq;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public interface IComparisonBothMissingElementsTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Variants { get; }
        List<string> CorrectVariants { get; }
        bool TryUpdateModelBasedOnPlayerChoice(string inputedValue, int inputedIndex, out TaskResultData taskData);
        int GetWrongIndex();
        void SetAllIndexesCorrect();
    }


    public class ComparisonBothMissingElementsTaskModel : BaseTaskModel, IComparisonBothMissingElementsTaskModel
    {
        private List<string> correctVariants;
        private ArithmeticSigns selectedSign;
        private List<int> integerVariants;
        private TaskResultData taskData;
        private string dublicatedValue;
        private int dublicatedIndex;

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
            variants = MathOperations.DublicateRandomValueAndShake(variants, out dublicatedValue, out dublicatedIndex);
            integerVariants = MathOperations.ConvertStringsToInt(variants);

            expression = new List<ExpressionElement>()
            {
                new ExpressionElement(TaskElementType.Value, 0, true),
                new ExpressionElement(TaskElementType.Operator, selectedSignText),
                new ExpressionElement(TaskElementType.Value, 0, true)
            };

            GetExpressionValues(expression, out elements, out operators);
        }

        public override TaskResultData GetResult()
        {
            var result = new TaskResultData();
            result.TaskType = TaskType;
            result.ElementValues = elements;
            result.OperatorValues = operators;
            result.VariantValues = variants;
            result.MaxValue = maxValue;
            taskData = result;
            return result;
        }

        public bool TryUpdateModelBasedOnPlayerChoice(string inputedValue, int inputedIndex, out TaskResultData data)
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

            bool isCorrectIndexesAvailable = false;
            //if there is no correct value, equal to first value selected by user,
            //we will use "fakeValue", that guaranteed us existing of correct variants
            int fakeValue = 0;
            switch (selectedSign)
            {
                case ArithmeticSigns.GreaterThan:
                    isCorrectIndexesAvailable 
                        = MathOperations.TryGetCorrectIndexesWithLessThen(intValue, integerVariants, out correctAnswersIndexes);
                    fakeValue = integerVariants.Max();
                    if (!isCorrectIndexesAvailable)
                    {
                        MathOperations.TryGetCorrectIndexesWithLessThen(fakeValue, integerVariants, out correctAnswersIndexes);
                    }

                    break;

                case ArithmeticSigns.LessThan:
                    isCorrectIndexesAvailable 
                        = MathOperations.TryGetCorrectIndexesWithGreaterThen(intValue, integerVariants, out correctAnswersIndexes);
                    fakeValue = integerVariants.Min();
                    if (!isCorrectIndexesAvailable)
                    {
                        MathOperations.TryGetCorrectIndexesWithGreaterThen(fakeValue, integerVariants, out correctAnswersIndexes);
                    }

                    break;

                case ArithmeticSigns.Equal:
                    isCorrectIndexesAvailable 
                        = MathOperations.TryGetCorrectIndexesWithEqualTo(intValue, integerVariants, inputedIndex, out correctAnswersIndexes);
                    fakeValue = int.Parse(dublicatedValue);
                    if (!isCorrectIndexesAvailable)
                    {
                        MathOperations.TryGetCorrectIndexesWithEqualTo(fakeValue, integerVariants, dublicatedIndex, out correctAnswersIndexes);
                    }
                    break;
            }

            correctVariants = new List<string>();

            for (int i = 0, j = correctAnswersIndexes.Count; i < j; i++)
            {
                var index = correctAnswersIndexes[i];
                correctVariants.Add(variants[index]);
            }

            if (!isCorrectIndexesAvailable)
            {
                SetAllIndexesCorrect();
            }

            UpdateTaskData();
            data = taskData;
            return isCorrectIndexesAvailable;
        }

        public void SetAllIndexesCorrect()
        {
            correctAnswersIndexes.Clear();
            for (int i = 0; i < amountOfVariants; i++)
            {
                correctAnswersIndexes.Add(i);
            }
        }

        public int GetWrongIndex()
        {
            var index = Enumerable.Range(0, integerVariants.Count).Except(correctAnswersIndexes).FirstOrDefault();
            return index;
        }

        private void UpdateTaskData()
        {
            taskData.CorrectAnswerIndexes = correctAnswersIndexes;
        }
    }
}

