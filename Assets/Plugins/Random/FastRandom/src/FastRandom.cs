using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomRandom
{
    /// <summary>
    /// Multiplicative Congruence generator using a modulus of 2^31
    /// </summary>
    public sealed class FastRandom : IRandom
    {
        public int Seed { get; private set; }

        private const ulong Modulus = 2147483647; //2^31
        private const ulong Multiplier = 1132489760;
        private const double ModulusReciprocal = 1.0 / Modulus;

        private ulong _next;

        public FastRandom()
            : this(RandomSeed.Crypto()) { }

        public FastRandom(int seed)
        {
            NewSeed(seed);
        }

        public void NewSeed()
        {
            NewSeed(RandomSeed.Crypto());
        }

        /// <inheritdoc />
        /// <remarks>If the seed value is zero, it is set to one.</remarks>
        public void NewSeed(int seed)
        {
            if (seed == 0)
                seed = 1;

            Seed = seed;
            _next = (ulong)seed % Modulus;
        }

        public float GetFloat()
        {
            return (float)InternalSample();
        }

        public int GetInt()
        {
            return Range(int.MinValue, int.MaxValue);
        }

        public float Range(float min, float max)
        {
            return (float)(InternalSample() * (max - min) + min);
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="min">Minimum value included</param>
        /// <param name="max">Maximum value excluded</param>
        /// <returns></returns>
        public int Range(int min, int max)
        {
            return (int)(InternalSample() * (max - min) + min);
        }

        public async System.Threading.Tasks.Task<List<int>> ExclusiveNumericRange(int min, int max, int elementsAmount, int removedAnswer)
        {
            List<int> arr = Enumerable.Range(min, max).ToList();
            arr.Remove(removedAnswer);
            List<int> result = new List<int>();
            for (int i = 0; i < elementsAmount; i++)
            {
                int returnValueIndex = this.Range(0, arr.Count-1);
                result.Add(arr[returnValueIndex]);
                arr.RemoveAt(returnValueIndex);
            }
            return result;
        }

        /// <summary>
        /// Generates a specified number of random integers within a given range while excluding a specified set of integers.
        /// </summary>
        /// <param name="minValue">The minimum value of the range.</param>
        /// <param name="maxValue">The maximum value of the range.</param>
        /// <param name="count">The number of random integers to generate.</param>
        /// <param name="exceptions">A list of integers to exclude from the generated random numbers.</param>
        /// <returns>A list of integers containing the generated random numbers.</returns>
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

        /// <summary>
        /// Generates a list of random positive integers whose sum is less than or equal to a specified maximum number.
        /// If the sum of all generated random integers already equals the maximum number, the rest of the integers will be equal to 0.
        /// </summary>
        /// <param name="amount">The number of integers to generate.</param>
        /// <param name="maxNumber">The maximum possible sum of the generated integers.</param>
        /// <returns>A List<int> containing the generated integers.</returns>
        public List<int> GetRandomElementValues(int amount, int maxNumber)
        {
            List<int> result = new List<int>();
            int sum = 0;
            int remainingAmount = amount;
            for (int i = 0; i < amount; i++)
            {
                if (sum == maxNumber)
                {
                    result.Add(0);
                    remainingAmount--;
                }
                else
                {
                    int maxPossible = maxNumber - sum - remainingAmount + 1;
                    int randomInt = (new System.Random()).Next(1, Math.Min(maxPossible, maxNumber) + 1);
                    result.Add(randomInt);
                    sum += randomInt;
                    remainingAmount--;
                }
            }
            while (remainingAmount > 0)
            {
                result.Add(0);
                remainingAmount--;
            }
            return result;
        }

        public bool TossACoin()
        {
            return Convert.ToBoolean(this.Range(0, 2));      
        }

        public Vector2 GetInsideCircle(float radius = 1)
        {
            var x = Range(-1f, 1f) * radius;
            var y = Range(-1f, 1f) * radius;
            return new Vector2(x, y);
        }

        public Vector3 GetInsideSphere(float radius = 1)
        {
            var x = Range(-1f, 1f) * radius;
            var y = Range(-1f, 1f) * radius;
            var z = Range(-1f, 1f) * radius;
            return new Vector3(x, y, z);
        }

        public Quaternion GetRotation()
        {
            return GetRotationOnSurface(GetInsideSphere());
        }

        public Quaternion GetRotationOnSurface(Vector3 surface)
        {
            return new Quaternion(surface.x, surface.y, surface.z, GetFloat());
        }

        private double InternalSample()
        {
            var ret = _next * ModulusReciprocal;
            _next = _next * Multiplier % Modulus;
            return ret;
        }
    }
}