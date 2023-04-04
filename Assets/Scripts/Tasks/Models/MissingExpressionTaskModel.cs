using System.Collections.Generic;
using CustomRandom;
using System.Linq;
using System;

namespace Mathy.Core.Tasks
{
    public sealed class MissingExpressionTaskModel : BaseTaskModel, IDefaultTaskModel
    {
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;

        public MissingExpressionTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var fastRandom = new FastRandom();
            var random = new System.Random();
            int elementOne, elementTwo, result;
            string unknownElementValue;
            bool isAddition = fastRandom.TossACoin();

            elements = new List<string>();
            operators = new List<string>();
            expression = new List<ExpressionElement>();

            if (isAddition)
            {
                elementOne = random.Next(minValue, maxValue + 1);
                elementTwo = random.Next(minValue, (maxValue - elementOne) + 1);
                result = elementOne + elementTwo;
                unknownElementValue = $"{elementOne} + {elementTwo}";
            }
            else
            {
                elementOne = random.Next(minValue, maxValue);
                elementTwo = random.Next(minValue, elementOne);
                result = elementOne - elementTwo;
                unknownElementValue = $"{elementOne} - {elementTwo}";
            }

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, unknownElementValue, true),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Equal),
                new ExpressionElement(TaskElementType.Value, result)
            };

            GetExpressionValues(expression, out elements, out operators);
            var variantsNumbers = GetVariants(result, amountOfVariants, minValue, maxValue, out int indexOfCorrect);
            variants = new List<string>(amountOfVariants);
            for (int i = 0; i < variantsNumbers.Count; i++)
            {
                if(i != indexOfCorrect)
                {
                    string splitedExpression = MathOperations.GetSumDiffPair(Int32.Parse(variantsNumbers[i]), maxValue);
                    variants.Add(splitedExpression);
                }
                else
                {
                    variants.Add(unknownElementValue);
                }                
            }
            correctAnswersIndexes = new List<int>() { indexOfCorrect };
        }
    }
}