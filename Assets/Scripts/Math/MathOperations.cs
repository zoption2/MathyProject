using Mathy.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
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

    }
}