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

        public async System.Threading.Tasks.Task<List<int>> ExclusiveNumericRange(int minValue, int maxValue, int count, List<int> exceptions)
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