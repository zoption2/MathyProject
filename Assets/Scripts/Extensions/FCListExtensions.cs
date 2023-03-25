using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Fallencake.Tools
{
    /// <summary>
    /// List extensions
    /// </summary>
    public static class ListExtensions
    {
        private static System.Random random = new System.Random();
        /// <summary>
        /// Return a random item from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T FCRandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
            return list[random.Next(0, list.Count)];
        }

        /// <summary>
        /// Removes a random item from the list, returning that item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T FCRemoveRandom<T>(this IList<T> list)
        {
            if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
            int index = random.Next(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Swaps two items in a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="itemA"></param>
        /// <param name="itemB"></param>
        public static void FCSwap<T>(this IList<T> list, int itemA, int itemB)
        {
            var temporary = list[itemA];
            list[itemA] = list[itemB];
            list[itemB] = temporary;
        }

        /// <summary>
        /// Shuffles a list randomly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void FCShuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list.FCSwap(i, random.Next(i, list.Count));
            }
        }


        //Temporary use FastRandom unstead of Unity Random
        public static int UniqueRandom(this List<int> list, int min, int max)
        {
            int value = UnityEngine.Random.Range(min, max);
            while (list.Contains(value))
            {
                value = UnityEngine.Random.Range(min, max);
            }
            return value;
        }

        public static List<int> UniqueRandomInts(int maxRange, int totalRandomnoCount)
        {
            List<int> uniqueIntList = new List<int>();
            int count = 0;
            System.Random r = new System.Random();
            List<int> listRange = new List<int>();
            for (int i = 0; i < totalRandomnoCount; i++)
            {
                listRange.Add(i);
            }
            while (listRange.Count > 0)
            {
                int item = r.Next(maxRange);    
                if (!uniqueIntList.Contains(item) && listRange.Count > 0)
                {
                    uniqueIntList.Add(item);
                    listRange.Remove(count);
                    count++;
                }
            }
            return uniqueIntList;
        }

        public static string UniqueRandom(this List<string> list, List<string> listToCheck)
        {
            string value = listToCheck[random.Next(0, listToCheck.Count)];
            while (list.Contains(value))
            {
                value = listToCheck[random.Next(0, listToCheck.Count)];
            }
            return value;
        }
    }
}