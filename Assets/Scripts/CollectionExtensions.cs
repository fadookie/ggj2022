using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    private static readonly Random random = new Random();

    public static T[] Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + random.Next(n - i);
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
        return array;
    }
    
    /// <summary>
    /// Shuffle this list
    /// </summary>
    public static void Shuffle<T>(this List<T> thisList)
    {
        for (int i = thisList.Count - 1; i >= 0; i--)
        {
            int j = random.Next(0, i);
            T tmp = thisList[i];
            thisList[i] = thisList[j];
            thisList[j] = tmp;
        }
 
    }
 
    /// <summary>
    /// Return a shuffled copy of this list (leaves this list as it was)
    /// </summary>
    public static List<T> ShuffleAndCopy<T>(this List<T> thisList)
    {
        T[] shuffled = new T[thisList.Count];
        thisList.CopyTo(shuffled);
        thisList.Shuffle();
        return shuffled.ToList();
    }
}
