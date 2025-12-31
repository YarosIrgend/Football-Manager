using System;
using System.Collections.Generic;

public static class ListExtensions
{
    private static Random random = new Random();

    public static T GetRandomItem<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new ArgumentException("List cannot be empty.", nameof(list));
        }

        int index = random.Next(list.Count);
        return list[index];
    }
}