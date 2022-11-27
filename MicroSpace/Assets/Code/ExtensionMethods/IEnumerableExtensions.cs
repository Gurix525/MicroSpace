using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionMethods
{
    public static class IEnumerableExtensions
    {
        private static Random _random = new();

        public static IEnumerable<T> MakeRandomPermutation<T>(this IEnumerable<T> sequence)
        {
            T[] returnArray = sequence.ToArray();

            for (int i = 0; i < returnArray.Length - 1; i++)
            {
                int swapIndex = _random.Next(i, returnArray.Length);
                if (swapIndex != i)
                {
                    T temp = returnArray[i];
                    returnArray[i] = returnArray[swapIndex];
                    returnArray[swapIndex] = temp;
                }
            }

            return returnArray;
        }
    }
}