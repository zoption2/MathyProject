using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Mathy;
using Mathy.Data;
using System.Text;
using System.Linq;
using System.Drawing.Text;

namespace Mathy.Core.Tasks
{
    public interface ITaskModel : IModel
    {
        string TitleKey { get; }
        string DescriptionKey { get;}
        TaskType TaskType { get;}
        TaskData GetResult();
        void Release();
    }

    public abstract class BaseTaskModel : IModel
    {
        public virtual string TitleKey => TaskSettings.Title;
        public virtual string DescriptionKey => TaskSettings.Description;
        public TaskType TaskType => TaskSettings.TaskType;

        protected readonly ScriptableTask TaskSettings;
        protected int totalValues;
        protected int totalOperators;
        protected int minValue;
        protected int maxValue;
        protected int amountOfVariants;

        public BaseTaskModel(ScriptableTask taskSettings)
        {
            TaskSettings = taskSettings;
            totalValues = taskSettings.BaseStats.ElementsAmount;
            totalOperators = taskSettings.BaseStats.OperatorsAmount;
            minValue = taskSettings.BaseStats.MinNumber;
            maxValue = taskSettings.BaseStats.MaxNumber;
            amountOfVariants = taskSettings.BaseStats.VariantsAmount;
        }

        public abstract TaskData GetResult();

        protected List<string> GetVariants(int correctValue, int amountOfVariants, int minValue, int maxValue, out int correctValueIndex)
        {
            var random = new System.Random();
            var results = new List<string>(amountOfVariants);
            results.Add(correctValue.ToString());

            for (int i = 1; i < amountOfVariants; i++)
            {
                var variant = random.Next(minValue, maxValue);
                while (variant == correctValue)
                {
                    variant = random.Next(minValue, maxValue);
                }
                results.Add(variant.ToString());
            }
            ShakeResults(results);
            correctValueIndex = GetIndexOfValueFromList(correctValue.ToString(), results);
            return results;
        }

        private int GetIndexOfValueFromList(string value, List<string> fromList)
        {
            for (int i = 0, j = fromList.Count; i < j; i++)
            {
                if (fromList[i].Equals(value))
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException(
                string.Format("Looking value {0} not found at list {1}", value, fromList)
                );
        }

        private void ShakeResults(List<string> list)
        {
            var random = new System.Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public virtual void Release()
        {
            
        }
    }

    public interface IAdditionTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Variants { get; }
        ExpressionElement CorrectElement { get;}
    }

    public sealed class AdditionTaskModel : BaseTaskModel, IAdditionTaskModel
    {
        private List<ExpressionElement> expression;
        private List<string> variants;
        private List<string> values;
        private List<string> operators;
        private ExpressionElement correctAnswer;
        private int correctAnswerIndex;

        public List<ExpressionElement> Expression => expression;
        public List<string> Variants => variants;
        public ExpressionElement CorrectElement => correctAnswer;

        public AdditionTaskModel(ScriptableTask taskSettings) : base(taskSettings)
        {
            var random = new System.Random();
            var valueOne = random.Next(minValue, maxValue);
            var valueTwo = random.Next(minValue, maxValue - valueOne);
            var result = valueOne + valueTwo;

            expression = new List<ExpressionElement>
            {
                new ExpressionElement(TaskElementType.Value, valueOne),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Plus),
                new ExpressionElement(TaskElementType.Value, valueTwo),
                new ExpressionElement(TaskElementType.Operator, (char)ArithmeticSigns.Equal),
                new ExpressionElement(TaskElementType.Value, result, true)
            };

            values = new List<string>(3);
            values.Add(expression[0].Value);
            values.Add(expression[2].Value);
            values.Add(expression[4].Value);

            operators = new List<string>(2);
            operators.Add(expression[1].Value);
            operators.Add(expression[3].Value);

            correctAnswer = expression[4];

            variants = GetVariants(result, amountOfVariants, minValue, maxValue, out int indexOfCorrect);
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

    [Serializable]
    public class ExpressionElement
    {
        public TaskElementType Type;
        public string Value;
        public bool IsUnknown;

        public ExpressionElement(TaskElementType type, object value, bool isUnknown = false)
        {
            Type = type;
            Value = value.ToString();
            IsUnknown = isUnknown;
        }
    }
}

