using System;
using System.Collections.Generic;

namespace Share.NetworkV2.Utils
{
    public class HashName
    {
        private static readonly Dictionary<string, ulong> _hashCache  = new Dictionary<string, ulong>();
        private static readonly char[]                    _hashBuffer = new char[1024];

        //FNV-1 64 bit hash
        public static ulong GetHash(Type type)
        {
            ulong hash;
            var typeName = type.FullName;
            if (_hashCache.TryGetValue(typeName, out hash)) return hash;
            hash = 14695981039346656037UL; //offset
            typeName.CopyTo(0, _hashBuffer, 0, typeName.Length);
            for (var i = 0; i < typeName.Length; i++)
            {
                hash = hash ^ _hashBuffer[i];
                hash *= 1099511628211UL; //prime
            }

            _hashCache.Add(typeName, hash);
            return hash;
        }
    }
}