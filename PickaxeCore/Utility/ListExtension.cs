﻿using System.Collections.Generic;
using System.Linq;

namespace PickaxeCore.Utility.ListExtension
{
    public static class ListExtension
    {
        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            int count = list.Count;
            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                    list.Capacity = size;
                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
    }
}
