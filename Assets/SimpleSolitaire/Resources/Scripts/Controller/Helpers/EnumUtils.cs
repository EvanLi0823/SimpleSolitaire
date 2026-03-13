using System;

namespace SimpleSolitaire
{
    public static class EnumUtils
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            T[] array = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(array, src) + 1;
            return (array.Length == j) ? array[0] : array[j];
        }
    }
}