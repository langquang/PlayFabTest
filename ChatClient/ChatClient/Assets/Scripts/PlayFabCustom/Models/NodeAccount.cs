using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace PlayFabCustom.Models
{
    [MessagePackObject]
    public class NodeAccount
    {
        [JsonProperty("0")] [Key(0)] public string serverID;
        [JsonProperty("1")] [Key(1)] public string playFabId;
        [JsonProperty("2")] [Key(2)] public string customId; // use to login to PlayFab if node account, null if master
        [JsonProperty("3")] [Key(3)] public string level;
        [JsonProperty("4")] [Key(4)] public string avatar;
        [JsonProperty("5")] [Key(5)] public string sessionTicket; // client caches only
    }

    [MessagePackObject]
    public class ClusterAccount
    {
        [JsonProperty("0")] [Key(0)] public bool              isMaster;
        [JsonProperty("1")] [Key(1)] public string            MasterId;
        [JsonProperty("2")] [Key(2)] public List<NodeAccount> accounts;
        [JsonProperty("3")] [Key(3)] public string            startUpPlayFabId; // client caches only

        public ClusterAccount()
        {
            accounts = new List<NodeAccount>();
        }

        public NodeAccount GetStartUpAccount()
        {
            return accounts.Find(a => a.playFabId.Equals(startUpPlayFabId));
        }
        
        public NodeAccount FindAccountByServerId(string serverID)
        {
            return accounts.Find(a => a.serverID.Equals(serverID));
        }
    }
}