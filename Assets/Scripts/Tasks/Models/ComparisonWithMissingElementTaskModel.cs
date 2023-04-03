using System.Collections.Generic;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public interface IComparisonWithMissingElementTaskModel : IDefaultTaskModel
    {
        List<string> CorrectVariants { get; }
    }


    public class ComparisonWithMissingElementTaskModel : BaseTaskModel, IComparisonWithMissingElementTaskModel
    {
        private List<ExpressionElement> expression;
        private List<string> elements;
        private List<string> operators;
        private List<string> variants;
        private List<string> correctVariants;
        private List<int> correctIndexes;
        private ExpressionElement correctElement;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public List<string> CorrectVariants => correctVariants;
        public ExpressionElement CorrectElement => correctElement;


        public ComparisonWithMissingElementTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            int firstValue = random.Next(minValue, maxValue + 1);
            int secondValue = random.Next(minValue, maxValue + 1);
            string operatorSign;

            if (firstValue.CompareTo(secondValue) < 0)
            {
                operatorSign = ((char)ArithmeticSigns.LessThan).ToString();
                variants = MathOperations.GetComparableVariants(firstValue, secondValue, amountOfVariants, minValue, maxValue, out List<int> indexesOfCorrect, ComparisonType.LessThen);
                correctIndexes = indexesOfCorrect;
            }
            else if (firstValue.CompareTo(secondValue) > 0)
            {
                operatorSign = ((char)ArithmeticSigns.GreaterThan).ToString();
                variants = MathOperations.GetComparableVariants(firstValue, secondValue, amountOfVariants, minValue, maxValue, out List<int> indexesOfCorrect, ComparisonType.GreaterThen);
                correctIndexes = indexesOfCorrect;
            }
            else
            {
                operatorSign = ((char)ArithmeticSigns.Equal).ToString();
                variants = MathOperations.GetComparableVariants(firstValue, secondValue, amountOfVariants, minValue, maxValue, out List<int> indexesOfCorrect, ComparisonType.Equal);
                correctIndexes = indexesOfCorrect;
            }

            correctVariants = new List<string>();
            for (int i = 0, j = correctIndexes.Count; i < j; i++)
            {
                correctVariants.Add(variants[correctIndexes[i]]);
            }

            expression = new List<ExpressionElement>()
            {
                new ExpressionElement(TaskElementType.Value, firstValue, true),
                new ExpressionElement(TaskElementType.Operator, operatorSign),
                new ExpressionElement(TaskElementType.Value, secondValue)
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
            result.CorrectAnswerIndexes = correctIndexes;
            return result;
        }
    }
}

