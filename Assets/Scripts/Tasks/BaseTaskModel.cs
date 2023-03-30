using System.Collections.Generic;
using System;
using Mathy.Data;
using System.Linq;

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

        protected const string kUnknownElement = "?";

        public BaseTaskModel(ScriptableTask taskSettings)
        {
            TaskSettings = taskSettings;
            totalValues = taskSettings.ElementsAmount;
            minValue = taskSettings.MinNumber;
            maxValue = taskSettings.MaxNumber;
            amountOfVariants = taskSettings.VariantsAmount;
        }

        public abstract TaskData GetResult();

        //protected virtual List<string> GetVariants(int correctValue, int amountOfVariants, int minValue, int maxValue, out int correctValueIndex)
        //{
        //    var random = new System.Random();
        //    var results = new List<string>(amountOfVariants);
        //    results.Add(correctValue.ToString());

        //    for (int i = 1; i < amountOfVariants; i++)
        //    {
        //        var variant = random.Next(minValue, maxValue);
        //        while (variant == correctValue)
        //        {
        //            variant = random.Next(minValue, maxValue);
        //        }
        //        results.Add(variant.ToString());
        //    }
        //    ShakeResults(results);
        //    correctValueIndex = GetIndexOfValueFromList(correctValue.ToString(), results);
        //    return results;
        //}

        protected virtual List<string> GetVariants(int correctValue, int amountOfVariants, int minValue, int maxValue, out int correctValueIndex)
        {
            var random = new System.Random();
            var results = new List<string>(amountOfVariants);
            results.Add(correctValue.ToString());

            var variants = new SortedSet<int>();
            variants.Add(correctValue);

            int maxAttempts = 100;
            int attempts = 0;

            while (variants.Count < amountOfVariants && attempts < maxAttempts)
            {
                int range = (maxValue - minValue) / 2;
                int minVariantValue = Math.Max(minValue, correctValue - range);
                int maxVariantValue = Math.Min(maxValue, correctValue + range);
                int variant = random.Next(minVariantValue, maxVariantValue + 1);
                if (!variants.Contains(variant))
                {
                    variants.Add(variant);
                }
                attempts++;
            }

            if (variants.Count < amountOfVariants)
            {
                var duplicates = new List<int>();
                foreach (var variant in variants)
                {
                    if (variant != correctValue && duplicates.Count < amountOfVariants - variants.Count)
                    {
                        duplicates.Add(variant);
                    }
                }
                results.AddRange(variants.Select(v => v.ToString()).Where(v => v != correctValue.ToString()));
                results.AddRange(duplicates.Select(v => v.ToString()));
                ShakeResults(results);
                correctValueIndex = GetIndexOfValueFromList(correctValue.ToString(), results);
                throw new Exception($"Could not generate enough unique variants for {correctValue}. Generated {variants.Count} unique variants, but needed {amountOfVariants}. Generated {duplicates.Count} duplicates instead.");
            }
            else
            {
                results.AddRange(variants.Select(v => v.ToString()).Where(v => v != correctValue.ToString()));
                ShakeResults(results);
                correctValueIndex = GetIndexOfValueFromList(correctValue.ToString(), results);
                return results;
            }
        }

        /// <summary>
        /// Extracts values and operators from a list of expression elements.
        /// </summary>
        /// <param name="expression">The list of expression elements to extract values and operators from.</param>
        /// <param name="elements">When the method returns, contains a list of strings representing the extracted values.</param>
        /// <param name="operators">When the method returns, contains a list of strings representing the extracted operators.</param>
        /// <remarks>
        /// Values are extracted from expression elements with type "Value". If an element is marked as unknown, the kUnknownElement string is used instead of its value.
        /// Operators are extracted from expression elements with type "Operator".
        /// </remarks>
        protected virtual void GetExpressionValues(List<ExpressionElement> expression, out List<string> elements, out List<string> operators)
        {
            elements = expression
                .Where(e => e.Type == TaskElementType.Value)
                .Select(e => e.IsUnknown ? kUnknownElement : e.Value)
                .ToList();

            operators = expression
                .Where(e => e.Type == TaskElementType.Operator)
                .Select(e => e.Value)
                .ToList();
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

