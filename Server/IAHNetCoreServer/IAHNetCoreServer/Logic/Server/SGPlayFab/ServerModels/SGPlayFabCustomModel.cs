using System;
using System.Collections.Generic;
using PlayFab.ServerModels;

namespace IAHNetCoreServer.Logic.Server.SGPlayFab.CustomModels
{
    /// <summary>
    /// Class contains item CustomData required to update
    /// </summary>
    [Serializable]
    public class ItemUpdateCustomData
    {
        public ItemInstance itemInstance;
        public List<string> KeysToRemove; //use this to remove keys in CustomData
    }

    /// <summary>
    /// Core Class contains all data response from PlayFab after execute cloudScript "UpdateUserData" success
    /// </summary>
    [Serializable]
    public class UpdateUserDataResult
    {
        public int                       errorCode;
        public List<CustomVCResult>      VCResults;
        public List<GrantedItemInstance> itemsGrantResult;
    }

    [Serializable]
    public class CustomVCResult
    {
        public string VCName;
        public int    balanceChange;
        public int    balance;
    }

    [Serializable]
    public class CustomItemGrantResult
    {
        public string                     ItemId;
        public string                     ItemInstanceId;
        public string                     ItemClass;
        public int                        RemainingUses;
        public Dictionary<string, string> CustomData;
    }
}