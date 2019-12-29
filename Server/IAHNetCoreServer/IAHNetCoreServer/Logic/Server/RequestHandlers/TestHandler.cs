using System;
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
            
            // revoke item
            
            // edit data
            player.KeyReward.OnlineReward.nextAt = DateTime.UtcNow;                       // online reward is a property inside KeyReward
            player.AddSyncEntities(SyncEntityName.ONLINE_REWARD);                         // mask sync entity OnlineReward to client
            player.AddChangedDataFlag(PFPlayerDataFlag.REWARD | PFPlayerDataFlag.REWARD); // mask save key REWARD to PlayFab

            //----------------------------------------------------------------------------------------------------------
            //---------------- COMMIT EDITED DATA ----------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------------------
            // save to PlayFab
            await PFDriver.CommitChangedToPlayFab(player); // asynchronous
            // sync to client
            SyncHelper.SyncDataToClient(player); // send a SyncData message to client

            // Response for current request
            var response = new TestResponse(request) {msg = "A response of test command from server"};
            player.Send(response);
            return response;
        }
    }
}