using System.Collections.Generic;
using System;
using System.Linq;
using CustomRandom;

namespace Mathy.Core.Tasks
{
    public interface ISumOfNumbersTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Elements { get; }
        List<string> Variants { get; }
        int UnknowntElementsAmount { get; }
    }

    public class SumOfNumbersTaskModel : BaseTaskModel, ISumOfNumbersTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Elements => elements;
        public List<string> Variants => variants;
        public int UnknowntElementsAmount => totalValues;

        public SumOfNumbersTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            var answer = random.Next(minValue, maxValue);
            var ñorrectVariantsValues = MathOperations.SplitNumberIntoAddends(answer, totalValues).ToList();

            elements = new List<string>();
            operators = new List<string>();
            expression = new List<ExpressionElement>();

            for (int i = 0; i < totalValues; i++)
            {
                expression.Add(new ExpressionElement(TaskElementType.Value, ñorrectVariantsValues[i], true));
                expression.Add(new ExpressionElement(TaskElementType.Operator,
                    i == totalValues - 1 ? (char)ArithmeticSigns.Equal : (char)ArithmeticSigns.Plus));
            }
            expression.Add(new ExpressionElement(TaskElementType.Value, answer));

            GetExpressionValues(expression, out elements, out operators);

            var randomizedVariantIndexes = Enumerable.Range(0, TaskSettings.VariantsAmount - 1).ToList().
                OrderBy(x => Guid.NewGuid()).ToList();
            var answerIndexes = randomizedVariantIndexes.Take(TaskSettings.ElementsAmount).ToList();

            correctAnswersIndexes = answerIndexes;

            var fastFandom = new FastRandom();
            List<int> variantsValues = fastFandom.ExclusiveNumericRange(
                minValue, maxValue, amountOfVariants, ñorrectVariantsValues);
            var correctValues = ñorrectVariantsValues;

            variants = new List<string>();

            for (int i = 0; i < TaskSettings.VariantsAmount; i++)
            {
                if (correctAnswersIndexes.Contains(i))
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
    }
}