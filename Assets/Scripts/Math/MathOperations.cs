using Mathy.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Mathy.Core
{
    /// <summary>
    /// A collection of methods based on math functions.
    /// </summary>
    public static class MathOperations
    {
        #region BASIC ARITHMETIC

        // Basic mathematical methods
        public static float Add(float a, float b) { return a + b; }
        public static float Subtract(float a, float b) { return a - b; }
        public static float Multiply(float a, float b) { return a * b; }
        public static float Divide(float a, float b) { return a / b; }

        #endregion

        #region COMPLEX CALCULATION

        // Calculation of the result by given expression string
        //public static double Evaluate(string expression)
        //{
        //    var doc = new System.Xml.XPath.XPathDocument(new System.IO.StringReader("<r/>"));
        //    var nav = doc.CreateNavigator();
        //    var newString = expression;
        //    newString = (new System.Text.RegularExpressions.Regex(@"([\+\-\*])")).Replace(newString, " ${1} ");
        //    newString = newString.Replace("/", " div ").Replace("%", " mod ");
        //    newString = newString.Replace(":", " div ");
        //    return (double)nav.Evaluate("number(" + newString + ")");
        //}

        public static int EvaluateInt(string expression)
        {
            return Mathf.RoundToInt((float)Evaluate(expression));
        }

        public static double Evaluate(string expression)
        {
            // Create a new DataTable.
            DataTable table = new DataTable();

            // Use the Evaluate method to execute the expression.
            var editedExpression = expression.Replace("x", "*").Replace(":", "/");
            object result = table.Compute(editedExpression, "");

            return Convert.ToDouble(result);
        }

        #endregion

        #region COMPARISON

        // Operations for comparing numbers
        public static bool IsEqual(float a, float b) { return Mathf.Approximately(a, b); }
        public static bool IsGreaterThan(float a, float b) { return a > b; }
        public static bool IsLessThan(float a, float b) { return a < b; }

        public static ArithmeticSigns Compare(float a, float b)
        {
            if (Mathf.Approximately(a, b))
            {
                return ArithmeticSigns.Equal;
            }
            else if (a < b)
            {
                return ArithmeticSigns.LessThan;
            }
            else
            {
                return ArithmeticSigns.GreaterThan;
            }
        }

        #endregion

        #region SPLITTING

        public static int[] SplitNumberIntoAddends(int numToSplit, int amount)
        {
            int[] results = new int[amount];
            int sum = 0;

            for (int i = 0; i < amount - 1; i++)
            {
                int maxVal = numToSplit - sum - (amount - i - 1); // Maximum value for this integer
                int value = 0;
                if (maxVal > 0) // Only generate positive integers
                {
                    value = new System.Random().Next(1, maxVal + 1); // Generate random integer between 1 and maxVal
                }
                results[i] = value;
                sum += value;
            }

            results[amount - 1] = numToSplit - sum;

            if (results[amount - 1] < 0) // If sum is greater than numToSplit, set all values to 0
            {
                for (int i = 0; i < amount - 1; i++)
                {
                    results[i] = 0;
                }
                results[amount - 1] = numToSplit;
            }

            return results;
        }

        public static (int, int, string) GetSumDiffPair(int n)
        {
            System.Random rnd = new System.Random();
            int x, y;
            string expression;

            if (rnd.Next(2) == 0)
            {
                // Return a pair that gives n when x + y is calculated
                if (n >= 0)
                {
                    x = rnd.Next(n + 1);
                    y = n - x;
                    expression = $"{x} + {y}";
                }
                else
                {
                    x = rnd.Next(-n + 1);
                    y = x - n;
                    expression = $"{x} + {y}";
                }
            }
            else
            {
                // Return a pair that gives n when x - y is calculated
                if (n >= 0)
                {
                    y = rnd.Next(n + 1);
                    x = n + y;
                    expression = $"{x} - {y}";
                }
                else
                {
                    y = rnd.Next(-n + 1);
                    x = y - n;
                    expression = $"{x} - {y}";
                }
            }

            return (x, y, expression);
        }

        public static string GetSumDiffPair(int numToSplit, int maxNumber)
        {
            System.Random random = new System.Random();
            int x, y;
            string expression;

            if (random.Next(2) == 0)
            {
                // Return a pair that gives numToSplit when x + y is calculated
                if (numToSplit >= 0)
                {
                    x = random.Next(numToSplit + 1);
                    y = numToSplit - x;
                    while (x > maxNumber || y > maxNumber)
                    {
                        x = random.Next(numToSplit + 1);
                        y = numToSplit - x;
                    }
                    expression = $"{x} + {y}";
                }
                else
                {
                    x = random.Next(-numToSplit + 1);
                    y = x - numToSplit;
                    while (x > maxNumber || y > maxNumber)
                    {
                        x = random.Next(-numToSplit + 1);
                        y = x - numToSplit;
                    }
                    expression = $"{x} + {y}";
                }
            }
            else
            {
                // Return a pair that gives numToSplit when x - y is calculated
                if (numToSplit >= 0)
                {
                    y = random.Next(numToSplit + 1);
                    x = numToSplit + y;
                    while (x > maxNumber || y > maxNumber)
                    {
                        y = random.Next(numToSplit + 1);
                        x = numToSplit + y;
                    }
                    expression = $"{x} - {y}";
                }
                else
                {
                    y = random.Next(-numToSplit + 1);
                    x = y - numToSplit;
                    while (x > maxNumber || y > maxNumber)
                    {
                        y = random.Next(-numToSplit + 1);
                        x = y - numToSplit;
                    }
                    expression = $"{x} - {y}";
                }
            }

            return expression;
        }

        #endregion

        //Generate a List of random integers, where their values and sum are less than or equal to maxNumber
        public static List<int> RandomNumbersForSum(int amount, int maxNumber)
        {
            List<int> randomNumbers = new List<int>();
            int sum = 0;
            for (int i = 0; i < amount; i++)
            {
                if (sum >= maxNumber)
                {
                    randomNumbers.Add(0);
                }
                else
                {
                    int nextNumber = UnityEngine.Random.Range(0, maxNumber + 1 - sum);
                    randomNumbers.Add(nextNumber);
                    sum += nextNumber;
                }
            }
            return randomNumbers;
        }

        //Generate a List of random integers, where their values and difference are less than or equal to maxNumber
        public static List<int> RandomNumbersForDiff(int amount, int maxNumber)
        {
            List<int> randomNumbers = new List<int>();
            int difference = maxNumber;
            for (int i = 0; i < amount; i++)
            {
                if (difference <= 0)
                {
                    randomNumbers.Add(0);
                }
                else
                {
                    int nextNumber = UnityEngine.Random.Range(0, Mathf.Min(maxNumber + 1, difference + 1));
                    randomNumbers.Add(nextNumber);
                    difference -= nextNumber;
                }
            }
            return randomNumbers;
        }

        public static string BuildExpressionFromValue(int value, int minLimit, int maxLimit)
        {
            var random = new System.Random();
            var randomValue = random.Next(minLimit, maxLimit + 1);
            while (randomValue == value)
            {
                randomValue = random.Next(minLimit, maxLimit + 1);
            }

            int firstValue;
            int secondValue;
            string sign;

            if (randomValue < value)
            {
                firstValue = randomValue;
                secondValue = value - firstValue;
                sign = "+";
            }
            else
            {
                firstValue = randomValue;
                secondValue = firstValue - value;
                sign = "-";
            }

            var result = string.Format($"{firstValue}{sign}{secondValue}");
            return result;
        }

        public static List<string> GetRandomVariants(int minValue, int maxValue, int countOfVariants)
        {
            var random = new System.Random();
            var result = new List<string>(countOfVariants);

            for (int i = 0; i < countOfVariants; i++)
            {
                var value = random.Next(minValue, maxValue + 1);
                result.Add(value.ToString());
            }

            return result;
        }

        public static List<string> DublicateRandomValueAndShake(List<string> input)
        {
            var random = new System.Random();
            var randomValue = input[random.Next(input.Count)];
            for (int i = 0, j = input.Count; i < j; i++)
            {
                if (input[i] != randomValue)
                {
                    input[i] = randomValue;
                    break;
                }
            }
            ShakeResults(input);
            return input;
        }

        public static List<string> GetComparableVariants(
              int firstValue
            , int secondValue
            , int amountOfVariants
            , int minValue
            , int maxValue
            , out List<int> correctIndexes
            , ComparisonType type)
        {
            var variants = GenerateVariants(firstValue, amountOfVariants, minValue, maxValue);
            var resultsInt = new List<int>(variants);

            ShakeResults(resultsInt);

            correctIndexes = GetCorrectIndexes(firstValue, secondValue, resultsInt, type);

            return ConvertIntToStrings(resultsInt);
        }

        #region InternalCalculationsHelpers
        private static SortedSet<int> GenerateVariants(int firstValue, int amountOfVariants, int minValue, int maxValue)
        {
            var random = new System.Random();
            var variants = new SortedSet<int> { firstValue };
            while (variants.Count < amountOfVariants)
            {
                int variant = random.Next(minValue, maxValue + 1);
                if (!variants.Contains(variant))
                {
                    variants.Add(variant);
                }
            }
            if (variants.Count < amountOfVariants)
            {
                throw new Exception($"Could not generate enough unique variants for {firstValue}. " +
                                    $"Generated {variants.Count} unique variants, but needed {amountOfVariants}.");
            }
            return variants;
        }

        private static List<int> GetCorrectIndexes(int firstValue, int secondValue, List<int> resultsInt, ComparisonType type)
        {
            switch (type)
            {
                case ComparisonType.Equal:
                    return GetCorrectIndexesWithEqualTo(firstValue, resultsInt);
                case ComparisonType.GreaterThen:
                    return GetCorrectIndexesWithGreaterThen(secondValue, resultsInt);
                case ComparisonType.LessThen:
                    return GetCorrectIndexesWithLessThen(secondValue, resultsInt);
                default:
                    throw new ArgumentException($"Invalid ComparisonType value: {type}", nameof(type));
            }
        }

        public static List<int> GetCorrectIndexesWithEqualTo(int firstValue, List<int> resultsInt)
        {
            var correctIndexes = new List<int>();
            for (int i = 0; i < resultsInt.Count; i++)
            {
                if (resultsInt[i] == firstValue)
                {
                    correctIndexes.Add(i);
                }
            }
            return correctIndexes;
        }

        public static List<int> GetCorrectIndexesWithGreaterThen(int secondValue, List<int> resultsInt)
        {
            var correctIndexes = new List<int>();
            for (int i = 0; i < resultsInt.Count; i++)
            {
                if (resultsInt[i] > secondValue)
                {
                    correctIndexes.Add(i);
                }
            }
            return correctIndexes;
        }

        public static List<int> GetCorrectIndexesWithLessThen(int secondValue, List<int> resultsInt)
        {
            var correctIndexes = new List<int>();
            for (int i = 0; i < resultsInt.Count; i++)
            {
                if (resultsInt[i] < secondValue)
                {
                    correctIndexes.Add(i);
                }
            }
            return correctIndexes;
        }

        public static List<string> ConvertIntToStrings(List<int> resultsInt)
        {
            return resultsInt.Select(v => v.ToString()).ToList();
        }

        public static List<int> ConvertStringsToInt(List<string> resultsInt)
        {
            return resultsInt.Select(v => int.Parse(v)).ToList();
        }

        private static void ShakeResults(List<int> resultsInt)
        {
            var random = new System.Random();
            for (int i = resultsInt.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = resultsInt[i];
                resultsInt[i] = resultsInt[j];
                resultsInt[j] = temp;
            }
        }

        private static void ShakeResults(List<string> resultsInt)
        {
            var random = new System.Random();
            for (int i = resultsInt.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                string temp = resultsInt[i];
                resultsInt[i] = resultsInt[j];
                resultsInt[j] = temp;
            }
        }
        #endregion
    }

    public enum ComparisonType
    {
        Equal,
        GreaterThen,
        LessThen
    }
}