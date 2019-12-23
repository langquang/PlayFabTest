using System;
using System.Collections.Concurrent;
using SourceShare.Share.NetworkV2;

namespace NetworkV2.Server
{
    public sealed class GroupPlayers<TKey>
    {
        private readonly ConcurrentDictionary<TKey, NetPlayer> _players;

        public GroupPlayers()
        {
            _players = new ConcurrentDictionary<TKey, NetPlayer>(Environment.ProcessorCount * 2, 100);
        }

        public NetPlayer FindPlayer(TKey key)
        {
            return _players.TryGetValue(key, out var result) ? result : null;
        }

        public bool AddPlayer(TKey key, NetPlayer player)
        {
            if (!_players.ContainsKey(key))
            {
                return _players.TryAdd(key, player);
            }
            else
            {
                return false;
            }
        }

        public NetPlayer RemovePlayer(TKey key)
        {
            return _players.TryRemove(key, out var result) ? result : null;
        }
    }
}