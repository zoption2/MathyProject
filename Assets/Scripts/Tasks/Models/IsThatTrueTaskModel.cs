using System.Collections.Generic;
using CustomRandom;
using System.Linq;
using Mathy.Data;

namespace Mathy.Core.Tasks
{
    public interface IIsThatTrueTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Variants { get; }
        int CorrectVariantIndex { get; }
    }

    public sealed class IsThatTrueTaskModel : BaseTaskModel, IIsThatTrueTaskModel
    {
        private const string kTableLocalize = "GUI Elements";
        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        private const string kTrueKey = "True";
        private const string kFalseKey = "False";
        public int CorrectVariantIndex { get; }

        public IsThatTrueTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var fastRandom = new FastRandom();
            var random = new System.Random();
            var elementValues = fastRandom.GetRandomElementValues(totalValues, maxValue);
            int result;
            bool isAddition = fastRandom.TossACoin();

            elements = new List<string>();
            operators = new List<string>();
            expression = new List<ExpressionElement>();

            if (isAddition)
            {
                result = elementValues.Sum();
                for (int i = 0; i < elementValues.Count; i++)
                {
                    expression.Add(new ExpressionElement(TaskElementType.Value, elementValues[i]));
                    expression.Add(new ExpressionElement(TaskElementType.Operator,
                        i == totalValues - 1 ? (char)ArithmeticSigns.Equal : (char)ArithmeticSigns.Plus));
                }
            }
            else
            {
                int elementOne = random.Next(minValue, maxValue);
                int elementTwo = random.Next(minValue, elementOne);
                elementValues = new List<int>(2) { elementOne, elementTwo };
                result = elementOne - elementTwo;

                for (int i = 0; i < 2; i++)
                {
                    expression.Add(new ExpressionElement(TaskElementType.Value, elementValues[i]));
                    expression.Add(new ExpressionElement(TaskElementType.Operator,
                        i == totalValues - 1 ? (char)ArithmeticSigns.Equal : (char)ArithmeticSigns.Minus));
                }
            }

            int taskAnswer;
            if (fastRandom.TossACoin())
            {
                taskAnswer = result;
                CorrectVariantIndex = 0;
            }
            else
            {
                taskAnswer = random.Next(minValue, maxValue);
                CorrectVariantIndex = 1;
            }

            expression.Add(new ExpressionElement(TaskElementType.Value, taskAnswer));
            GetExpressionValues(expression, out elements, out operators);
            variants = new List<string>()
            {
                LocalizationManager.GetLocalizedString(kTableLocalize, kTrueKey),
                LocalizationManager.GetLocalizedString(kTableLocalize, kFalseKey),
            };

            correctAnswersIndexes = new List<int>()
            {
                CorrectVariantIndex
            };
        }
    }
}