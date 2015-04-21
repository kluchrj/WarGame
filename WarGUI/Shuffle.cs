using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;

namespace WarGUI
{
    /*
     * Knuth-Fisher-Yates shuffle
     */
    static class MyExtensions
    {
        static RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int i = list.Count;
            while (i > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < i * (Byte.MaxValue / i)));
                i--;
                list.Swap(i, (box[0] % i));
            }
        }

        public static void FastShuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rng.Next(i, list.Count));
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
