using System;
using System.Linq;

namespace MoovyCopy
{
    class Utility
    {
        private static readonly string[] bytesToStringSuffixes = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
        public static string BytesToString(long bytes)
        {
            double result = bytes;
            int suffix = 0;
            while (suffix < bytesToStringSuffixes.Length && result >= 1000)
            {
                result /= 1000;
                ++suffix;
            }
            if (result >= 100)
            {
                return result.ToString("0") + " " + bytesToStringSuffixes[suffix];
            }
            else
            {
                return result.ToString("0.0") + " " + bytesToStringSuffixes[suffix];
            }
        }
    }
}
