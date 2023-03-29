using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mathy.UI.Tasks;
using Mathy.Data;
using System;
using Mathy;
using System.Linq;

namespace Mathy.Core.Tasks
{
    public interface ISumOfNumbersTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Elements { get; }
        List<string> Variants { get; }
        ExpressionElement CorrectElement { get; }
    }

    public class SumOfNumbersTaskModel : BaseTaskModel, ISumOfNumbersTaskModel
    {
        private List<ExpressionElement> expression;
        private List<string> variants;
        private List<string> elements;
        private List<string> operators;
        private ExpressionElement correctAnswer;
        private List<int> correctAnswerIndexes;

        public List<ExpressionElement> Expression => expression;
        public List<string> Elements => elements;
        public List<string> Variants => variants;
        public ExpressionElement CorrectElement => correctAnswer;

        public SumOfNumbersTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            var answer = random.Next(minValue, maxValue);
            var ñorrectVariantsValues = MathOperations.SplitNumberIntoAddends(answer, totalValues).ToList();

            expression = new List<ExpressionElement>();
            elements = new List<string>();
            operators = new List<string>();
            ExpressionElement expressionElement;
            ExpressionElement expressionOperator;

            for (int i = 0; i < totalValues; i++)
            {
                expressionElement = new ExpressionElement(TaskElementType.Value, ñorrectVariantsValues[i], true);
                expression.Add(expressionElement);
                elements.Add(expressionElement.Value);

                expressionOperator = new ExpressionElement(TaskElementType.Operator, 
                    i == totalValues - 1 ? (char)ArithmeticSigns.Equal : (char)ArithmeticSigns.Plus);
                expression.Add(expressionOperator);
                operators.Add(expressionOperator.Value);
            }
            expressionElement = new ExpressionElement(TaskElementType.Value, answer);
            expression.Add(expressionElement);

            correctAnswer = expressionElement;

            //variants = GetVariants(ñorrectVariantsValues, amountOfVariants, minValue, maxValue, out List<int> indexesOfCorrect);
            //correctAnswerIndexes = indexesOfCorrect;

            var randomizedVariantIndexes = Enumerable.Range(0, TaskSettings.BaseStats.VariantsAmount - 1).ToList().
                OrderBy(x => Guid.NewGuid()).ToList();
            var answerIndexes = randomizedVariantIndexes.Take(TaskSettings.BaseStats.ElementsAmount).ToList();

            correctAnswerIndexes = answerIndexes;

            List<int> variantsValues = ExclusiveNumericRange(
                minValue, maxValue, amountOfVariants, ñorrectVariantsValues);
            var correctValues = ñorrectVariantsValues;

            variants = new List<string>();

            for (int i = 0; i < TaskSettings.BaseStats.VariantsAmount; i++)
            {
                if (correctAnswerIndexes.Contains(i))
                {
                    this.variants.Add(correctValues.First().ToString());
                    correctValues.RemoveAt(0);
                }
                else
                {
                    this.variants.Add(variantsValues[i].ToString());
                }
            }
        }

        protected List<string> GetVariants(List<int> correctValues, int amountOfVariants, int minValue, int maxValue, out List<int> indexesOfCorrect)
        {
            var random = new System.Random();
            var correctStringValues = correctValues.ConvertAll<string>(x => x.ToString());
            var variantValues = new List<string>();

            variantValues.AddRange(correctStringValues);
            for (int i = variantValues.Count; i < amountOfVariants; i++)
            {
                var variant = random.Next(minValue, maxValue);
                while (correctValues.Contains(variant))
                {
                    variant = random.Next(minValue, maxValue);
                }
                variantValues.Add(variant.ToString());
            }

            ShakeResults(variantValues);

            indexesOfCorrect = GetIndexesOfValueFromList(correctStringValues, variantValues);

            return variantValues;
        }

        public List<int> ExclusiveNumericRange(int minValue, int maxValue, int count, List<int> exceptions)
        {
            System.Random random = new System.Random();

            if (count > (maxValue - minValue + 1 - exceptions.Count))
            {
                throw new ArgumentException("Cannot generate requested number of random integers within the given range and excluding the given exceptions.");
            }

            List<int> randomInts = new List<int>();

            while (randomInts.Count < count)
            {
                int randomInt = random.Next(minValue, maxValue + 1);
                if (!exceptions.Contains(randomInt) && !randomInts.Contains(randomInt))
                {
                    randomInts.Add(randomInt);
                }
            }

            return randomInts;
        }

        public override TaskData GetResult()
        {
            var result = new TaskData();
            result.TaskType = TaskType;
            result.ElementValues = elements;
            result.OperatorValues = operators;
            result.VariantValues = variants;
            result.CorrectAnswerIndexes = correctAnswerIndexes;
            return result;
        }
    }
}