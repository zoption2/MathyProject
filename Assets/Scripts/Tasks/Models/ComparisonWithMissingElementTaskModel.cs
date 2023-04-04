using System.Collections.Generic;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public interface IComparisonMissingElementTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Variants { get; }
        List<string> CorrectVariants { get; }
    }


    public class ComparisonWithMissingElementTaskModel : BaseTaskModel, IComparisonMissingElementTaskModel
    {
        private List<string> correctVariants;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public List<string> CorrectVariants => correctVariants;


        public ComparisonWithMissingElementTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            int firstValue = random.Next(minValue, maxValue + 1);
            int secondValue = random.Next(minValue, maxValue + 1);
            string operatorSign;

            if (firstValue.CompareTo(secondValue) < 0)
            {
                operatorSign = ((char)ArithmeticSigns.LessThan).ToString();
                variants = MathOperations.GetComparableVariants(firstValue, secondValue, amountOfVariants, minValue, maxValue, out List<int> indexesOfCorrect, ComparisonType.LessThen);
                correctAnswersIndexes = indexesOfCorrect;
            }
            else if (firstValue.CompareTo(secondValue) > 0)
            {
                operatorSign = ((char)ArithmeticSigns.GreaterThan).ToString();
                variants = MathOperations.GetComparableVariants(firstValue, secondValue, amountOfVariants, minValue, maxValue, out List<int> indexesOfCorrect, ComparisonType.GreaterThen);
                correctAnswersIndexes = indexesOfCorrect;
            }
            else
            {
                operatorSign = ((char)ArithmeticSigns.Equal).ToString();
                variants = MathOperations.GetComparableVariants(firstValue, secondValue, amountOfVariants, minValue, maxValue, out List<int> indexesOfCorrect, ComparisonType.Equal);
                correctAnswersIndexes = indexesOfCorrect;
            }

            correctVariants = new List<string>();
            for (int i = 0, j = correctAnswersIndexes.Count; i < j; i++)
            {
                correctVariants.Add(variants[correctAnswersIndexes[i]]);
            }

            expression = new List<ExpressionElement>()
            {
                new ExpressionElement(TaskElementType.Value, firstValue, true),
                new ExpressionElement(TaskElementType.Operator, operatorSign),
                new ExpressionElement(TaskElementType.Value, secondValue)
            };

            GetExpressionValues(expression, out elements, out operators);
        }
    }


    public class ComparisonBothMissingElementsTaskModel : BaseTaskModel, IComparisonMissingElementTaskModel
    {
        public List<ExpressionElement> Expression => throw new System.NotImplementedException();
        public List<string> Variants => throw new System.NotImplementedException();
        public List<string> CorrectVariants => throw new System.NotImplementedException();

        public ComparisonBothMissingElementsTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
        }



        public override TaskData GetResult()
        {
            throw new System.NotImplementedException();
        }
    }
}

