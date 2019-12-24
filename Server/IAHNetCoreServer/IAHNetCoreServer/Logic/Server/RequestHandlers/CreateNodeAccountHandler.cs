using System.Collections.Generic;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.SGPlayFab;
using PlayFabCustom;
using PlayFabCustom.Models;
using PlayFabShare;
using PlayFabShare.Models;
using SourceShare.Share.NetRequest;
using SourceShare.Share.NetRequest.Config;
using SourceShare.Share.NetworkV2.TransportData.Base;

namespace IAHNetCoreServer.Logic.Server.RequestHandlers
{
    public class CreateNodeAccountHandler
    {
        public static async Task<INetData> Perform(CreateNodeAccountRequest request, DataPlayer player)
        {
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
            var result = await PFDriver.GetInternalData(player, new List<string> {PFPlayerDataKey.ACCOUNT});
            if (result.Error != null)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.INTERNAL_NETWORK_ERROR);
            }

            result.Result.Data.TryGetValue(PFPlayerDataKey.ACCOUNT, out var record);
            if (record == null)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.INTERNAL_NETWORK_ERROR);
            }

            player.UpdateDataFromPayload(result.Result.Data); // update new data from PlayFab

            // ======================== the second check ===============================================================
            if (player.IsMasterAccount())
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_A_MASTER_ACCOUNT);
            }

            if (player.IsNodeAccount())
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_A_MASTER_ACCOUNT);
            }

            if (player.ClusterAccount.FindAccountByServerId(request.serverId) != null)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_CREATED_ACCOUNT_IN_THIS_SERVER);
            }

            // ======================== EVERY THING IS OK ==============================================================
            var account = new NodeAccount()
            {
                serverID = player.Statistic.Server,
                playFabId = player.PlayerId,
                customId = player.PFCustomId,
                level = player.Statistic.Level
            };
            player.ClusterAccount.isMaster = false;
            player.ClusterAccount.accounts.Add(account);

            // save to PlayFab
            var success = await PFDriver.SaveInternalData(player, PFPlayerDataFlag.ACCOUNT);
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