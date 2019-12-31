using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.SGPlayFab;
using NLog;
using PlayFabCustom.Models;
using PlayFabShare;
using SourceShare.Share.APIServer.Data;
using SourceShare.Share.NetRequest;
using SourceShare.Share.NetRequest.Config;
using SourceShare.Share.NetworkV2.TransportData.Base;

namespace IAHNetCoreServer.Logic.Server.RequestHandlers
{
    public class TestHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<INetData> Perform(TestRequest request, DataPlayer player)
        {
            Logger.Debug($"Server receive a Test Command with content={request.msg}");

            if (!request.IsValid()) // check require params
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.WRONG_REQUEST);
            }

            //----------------------------------------------------------------------------------------------------------
            //---------------- EDIT DATA -------------------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------------------
            // increase currency
            player.IncreaseGem(500); // auto save to PlayFab and auto sync to client
            // decrease Gold
            player.DecreaseGold(100); // auto save to PlayFab and auto sync to client
            // change statistic
            player.Level++; // auto save to PlayFab and auto sync to client
            // grant item
            IncreaseInventoryItemState state = player.IncreaseInventoryItem("HealthPotion", 5); // grant new if not exist or not stackable
            switch (state)
            {
                case IncreaseInventoryItemState.FAIL:
                    Logger.Debug("Wrong logic here, increase negative value");
                    break;
                case IncreaseInventoryItemState.UPDATE_CACHES_IN_LATER:
                    Logger.Debug("Increase successfull but need to wait for response from PlayFab <instanceId> to update cache Player");
                    break;
                case IncreaseInventoryItemState.UPDATE_CACHES_IN_IMMEDIATE:
                    Logger.Debug("Increase successfull");
                    break;
            }

            // revoke item
            var itemInstance = player.FindFirstInventoryItemByItemId("HealthPotion");
            if (itemInstance != null)
            {
                player.DecreaseInventoryItem(itemInstance, 2);

                // edit instance custom data
                if (itemInstance.CustomData == null)
                    itemInstance.CustomData = new Dictionary<string, string>();
                if (itemInstance.CustomData.TryGetValue("star", out var star))
                {
                    itemInstance.CustomData["star"] = (int.Parse(star) + 1).ToString();
                }
                else
                {
                    itemInstance.CustomData["star"] = "1";
                }

                player.UpdateInventoryItemCustomData(itemInstance); // call this function to save new CustomData to PlayFab
            }

            // edit data
            player.KeyReward.OnlineReward.nextAt = DateTime.UtcNow;                       // online reward is a property inside KeyReward
            player.AddSyncEntities(SyncEntityName.ONLINE_REWARD);             // mask sync entity OnlineReward to client
            player.AddChangedDataFlag(PFPlayerDataFlag.REWARD | PFPlayerDataFlag.REWARD); // mask save key REWARD to PlayFab

            //----------------------------------------------------------------------------------------------------------
            //---------------- COMMIT EDITED DATA ----------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------------------
            // sync to client
            SyncHelper.SyncDataToClient(player); // send a SyncData message to client
            // save to PlayFab
            await PFDriver.CommitChangedToPlayFab(player); // asynchronous

            // Response for current request
            var response = new TestResponse(request) {msg = "A response of test command from server"};
            player.Send(response);
            return response;
        }
    }
}