using System.Collections.Generic;
using System;
using Mathy.Data;

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

    public interface IDefaultTaskModel : ITaskModel
    {
        List<ExpressionElement> Expression { get; }
        List<string> Variants { get; }
        ExpressionElement CorrectElement { get; }
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

        protected virtual List<string> GetVariants(int correctValue, int amountOfVariants, int minValue, int maxValue, out int correctValueIndex)
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

        protected int GetIndexOfValueFromList(string value, List<string> fromList)
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

        public static List<int> GetIndexesOfValueFromList(List<string> values, List<string> fromList)
        {
            List<int> indexes = new List<int>();
            HashSet<int> addedIndexes = new HashSet<int>(); // to avoid duplicates
            for (int i = 0; i < fromList.Count; i++)
            {
                if (values.Contains(fromList[i]) && !addedIndexes.Contains(i))
                {
                    indexes.Add(i);
                    addedIndexes.Add(i);
                }
            }
            return indexes;
        }

        protected void ShakeResults(List<string> list)
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
}

