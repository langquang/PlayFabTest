using System;
using System.Threading.Tasks;
using IAHNetCoreServer.Logic.Server.SGPlayFab;
using NetworkV2.Server;
using PlayFabCustom.Models;
using PlayFabShare;
using SourceShare.Share.NetRequest;
using SourceShare.Share.NetRequest.Config;
using SourceShare.Share.NetworkV2.TransportData.Base;
using SourceShare.Share.NetworkV2.Utils;

namespace IAHNetCoreServer.Logic.Server.RequestHandlers
{
    public class CheckCreateNodeAccountHandler
    {
        public static async Task<INetData> Perform(CheckCreateNodeAccountRequest request, DataPlayer player)
        {
            if (player.IsMasterAccount())
            {
                return PerformWithMasterAccountRole(request, player);
            }
            else
            {
                return await PerformWithNodeAccountRole(request, player);
            }
        }

        private static INetData PerformWithMasterAccountRole(CheckCreateNodeAccountRequest request, DataPlayer player)
        {
            var existAccount = player.ClusterAccount.FindAccountByServerId(request.serverId);
            if (existAccount != null)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_CREATED_ACCOUNT_IN_THIS_SERVER);
            }

            return EntryHandler.ResponseSuccess(player, request, new CheckCreateNodeAccountResponse());
        }

        private static async Task<INetData> PerformWithNodeAccountRole(CheckCreateNodeAccountRequest request, DataPlayer player)
        {
            var master = new DataPlayer(request.masterId);
            var success = await PFDriver.LoadUserData(master, PFPlayerDataFlag.ACCOUNT);
            if (!success || !master.ClusterAccount.isMaster)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.WRONG_MASTER_ACCOUNT);
            }

            if (!master.ClusterAccount.ContainsNode(player.PlayerId))
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.WRONG_MASTER_ACCOUNT);
            }

            var existAccount = player.ClusterAccount.FindAccountByServerId(request.serverId);
            if (existAccount != null)
            {
                return EntryHandler.ResponseError(player, request, NetAPIErrorCode.ALREADY_CREATED_ACCOUNT_IN_THIS_SERVER);
            }

            return EntryHandler.ResponseSuccess(player, request, new CheckCreateNodeAccountResponse());
        }
    }
}