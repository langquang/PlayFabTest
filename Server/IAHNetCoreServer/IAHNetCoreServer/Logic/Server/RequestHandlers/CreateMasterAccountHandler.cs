using System.Collections.Generic;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.Setting;
using IAHNetCoreServer.Logic.Server.SGPlayFab;
using PlayFabCustom.Models;
using PlayFabShare;
using PlayFabShare.Models;
using SourceShare.Share.NetRequest;
using SourceShare.Share.NetRequest.Config;
using SourceShare.Share.NetworkV2.TransportData.Base;

namespace IAHNetCoreServer.Logic.Server.RequestHandlers
{
    public class CreateMasterAccountHandler
    {
        public static void Perform(CreateMasterAccountRequest request, DataPlayer player)
        {
            PerformAsync(request, player);
        }

        private static async Task<INetData> PerformAsync(CreateMasterAccountRequest request, DataPlayer player)
        {
            if (request.serverId != GameSetting.CURRENT_SERVER)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.WRONG_SERVER);
            }

            // ======================== the first check ================================================================
            if (player.IsMasterAccount())
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_A_MASTER_ACCOUNT);
            }

            if (player.IsNodeAccount())
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_A_MASTER_ACCOUNT);
            }

            // ================= Try to check with newest data from PlayFab ============================================
            var updateSuccess = await PFDriver.LoadUserData(player, PFPlayerDataFlag.ACCOUNT);
            if (!updateSuccess)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.FATAL_ERROR);
            }
            
            // ======================== the second check ===============================================================
            if (player.IsMasterAccount())
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_A_MASTER_ACCOUNT);
            }

            if (player.IsNodeAccount())
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_A_MASTER_ACCOUNT);
            }

            // ======================== EVERY THING IS OK ==============================================================
            var account = new NodeAccount()
            {
                serverID = request.serverId,
                playFabId = player.PlayerId,
                customId = player.PFCustomId,
                level = 1
            };
            player.ClusterAccount.isMaster = true;
            player.ClusterAccount.MasterId = player.PlayerId;
            player.ClusterAccount.accounts.Add(account);
            player.AddChangedDataFlag(PFPlayerDataFlag.ACCOUNT);
            player.Server = request.serverId;

            // save to PlayFab
            var success = await PFDriver.CommitChanged(player);
            if (!success)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.FATAL_ERROR);
            }
            else
            {
                return EntryHandler.ResponseSuccess(player, request, new CreateMasterAccountResponse());
            }
        }
    }
}