using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace PlayFabCustom.Models
{
    [MessagePackObject]
    public class NodeAccount
    {
        [JsonProperty("0")] [Key(0)] public string serverID;
        [JsonProperty("1")] [Key(1)] public string PlayFabId;
        [JsonProperty("2")] [Key(2)] public string Ticket;
        [JsonProperty("3")] [Key(3)] public string level;
        [JsonProperty("4")] [Key(4)] public string avatar;
    }

    [MessagePackObject]
    public class ClusterAccount
    {
        [JsonProperty("0")] [Key(0)] public bool isMaster;

        [JsonProperty("1")] [Key(1)] public Dictionary<string, NodeAccount> accounts;
    }
}