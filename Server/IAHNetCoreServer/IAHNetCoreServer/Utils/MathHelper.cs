using System;

namespace SourceShare.Share.Utils
{
    public class MathHelper
    {
        public static int SafeIncreaseIntValue(int curValue, int incValue)
        {
            long beginValue = curValue;             // cast to long
            var afterValue = beginValue + incValue; // auto cast to long
            // clamp
            return (int) Math.Clamp(afterValue, 0, int.MaxValue); // save to cast to int
        }

        public static int SafeDecreaseIntValue(int curValue, int decValue)
        {
            long beginValue = curValue;             // cast to long
            var afterValue = beginValue - decValue; // auto cast to long
            // clamp
            return (int) Math.Clamp(afterValue, 0, int.MaxValue); // save to cast to int
        }
    }
}