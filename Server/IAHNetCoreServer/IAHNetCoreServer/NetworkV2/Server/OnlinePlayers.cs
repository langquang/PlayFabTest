using System;

namespace NetworkV2.Server
{
    public sealed class OnlinePlayers
    {
        #region SINGLETON
        
        /// <summary>
        /// Requires .NET 4 and C# 6.0 (VS2015) or newer.
        /// </summary>
        private static readonly Lazy<OnlinePlayers> lazy = new Lazy<OnlinePlayers>(() => new OnlinePlayers());
        
        public static OnlinePlayers Instance => lazy.Value;
        
        #endregion

        public GroupPlayers<string> Players { get; }

        private OnlinePlayers()
        {
            Players = new GroupPlayers<string>();
        }
    }
}