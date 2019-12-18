using System;
using System.Collections.Concurrent;

namespace IAHNetCoreServer.Server
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

        private readonly ConcurrentDictionary<string, NetPlayer> allPlayers;

        private OnlinePlayers()
        {
            allPlayers = new ConcurrentDictionary<string, NetPlayer>(Environment.ProcessorCount * 2, 100);
        }

        public NetPlayer FindPlayer(string playerId)
        {
            return allPlayers.TryGetValue(playerId, out var result) ? result : null;
        }

        public bool AddPlayer(string playerId, NetPlayer player)
        {
            if (allPlayers.ContainsKey(playerId))
            {
                return allPlayers.TryAdd(playerId, player);
            }
            else
            {
                return false;
            }
        }

        public NetPlayer RemovePlayer(string playerId)
        {
            return allPlayers.TryRemove(playerId, out var result) ? result : null;
        }
    }
}