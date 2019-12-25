using System.Collections.Generic;

namespace PlayFabShare
{
    public class PFPlayerDataFlag
    {
        public const int EMPTY      = 0;
        public const int ACCOUNT    = 1; // INTERNAL
        public const int TOURNAMENT = 1 << 1;

        public const int GROUP_INTERNAL = ACCOUNT;

        public static List<string> ConvertToUserDataNames(int flag)
        {
            var keys = new List<string>();
            if ((flag & ACCOUNT) != 0)
            {
                keys.Add(PFPlayerDataKey.ACCOUNT);
            }

            return keys;
        }

        /// <summary>
        /// This func is not good, but it's work!
        /// </summary>
        public static bool IsContainsInternalData(int flag)
        {
            if ((flag & ACCOUNT) != 0)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// This func is not good, but it's work!
        /// </summary>
        public static bool IsContainsPublicData(int flag)
        {
            return false;
        }
        
        /// <summary>
        /// This func is not good, but it's work!
        /// </summary>
        public static bool IsContainsReadOnlyData(int flag)
        {
            return false;
        }
    }
}