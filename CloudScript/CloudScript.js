// DEFINES
const VC_GOLD = "GO";
const VC_GEM = "GE";
const VC_QUEST_POINT = "QP";


handlers.AddPlayerTag = function (args) {
    server.AddPlayerTag({PlayFabId: currentPlayerId, TagName: args.tag});
};

handlers.RemovePlayerTag = function (args) {
    server.RemovePlayerTag({PlayFabId: currentPlayerId, TagName: args.tag});
};


//----------------------------------------------------------------------------------------
//	COMBINE UPDATE USER DATA
//----------------------------------------------------------------------------------------
handlers.UpdateUserData = function (args) {

    let errorCode = 0;

    //------------------------------------------------------------------------------------------------------------
    //	VIRTUAL CURRENCIES
    //------------------------------------------------------------------------------------------------------------
    let gemReward = args.gemReward;
    let gemDecrease = args.gemDecrease;

    let goldReward = args.goldReward;
    let goldDecrease = args.goldDecrease;

    let questPointReward = args.questPointReward;
    let questPointDecrease = args.questPointDecrease;

    let VCResults = [];

    if (gemReward > 0) {
        let result = server.AddUserVirtualCurrency({
            PlayFabId: currentPlayerId,
            VirtualCurrency: VC_GEM,
            Amount: gemReward
        });
        VCResults.push({
            VCName: result.VirtualCurrency,
            balance: result.Balance,
            balanceChange: result.BalanceChange
        });
    }

    if (goldReward > 0) {
        let result = server.AddUserVirtualCurrency({
            PlayFabId: currentPlayerId,
            VirtualCurrency: VC_GOLD,
            Amount: goldReward
        });
        VCResults.push({
            VCName: result.VirtualCurrency,
            balance: result.Balance,
            balanceChange: result.BalanceChange
        });
    }

    if (questPointReward > 0) {
        let result = server.AddUserVirtualCurrency({
            PlayFabId: currentPlayerId,
            VirtualCurrency: VC_QUEST_POINT,
            Amount: questPointReward
        });
        VCResults.push({
            VCName: result.VirtualCurrency,
            balance: result.Balance,
            balanceChange: result.BalanceChange
        });
    }

    if (gemDecrease > 0) {
        let result = server.SubtractUserVirtualCurrency({
            PlayFabId: currentPlayerId,
            VirtualCurrency: VC_GEM,
            Amount: gemDecrease
        });
        VCResults.push({
            VCName: result.VirtualCurrency,
            balance: result.Balance,
            balanceChange: result.BalanceChange
        });
    }

    if (goldDecrease > 0) {
        let result = server.SubtractUserVirtualCurrency({
            PlayFabId: currentPlayerId,
            VirtualCurrency: VC_GOLD,
            Amount: goldDecrease
        });
        VCResults.push({
            VCName: result.VirtualCurrency,
            balance: result.Balance,
            balanceChange: result.BalanceChange
        });
    }

    if (questPointDecrease > 0) {
        let result = server.SubtractUserVirtualCurrency({
            PlayFabId: currentPlayerId,
            VirtualCurrency: VC_QUEST_POINT,
            Amount: questPointDecrease
        });
        VCResults.push({
            VCName: result.VirtualCurrency,
            balance: result.Balance,
            balanceChange: result.BalanceChange
        });
    }

    //------------------------------------------------------------------------------------------------------------
    //	STATISTICS
    //------------------------------------------------------------------------------------------------------------
    let statsUpdate = args.statsUpdate;

    if (statsUpdate)
        server.UpdatePlayerStatistics({PlayFabId: currentPlayerId, Statistics: statsUpdate});


    //------------------------------------------------------------------------------------------------------------
    //	USER TITLE DATA
    //------------------------------------------------------------------------------------------------------------
    let userReadonlyData = args.userReadonlyData;

    // Update UserReadOnlyData
    if (userReadonlyData)
        server.UpdateUserReadOnlyData({PlayFabId: currentPlayerId, Data: userReadonlyData, Permission: 1});


    //------------------------------------------------------------------------------------------------------------
    //	TAGS
    //------------------------------------------------------------------------------------------------------------
    let playerTagsAdd = args.playerTagsAdd;
    let playerTagsRemove = args.playerTagsRemove;

    // Remove Player Tags
    if (playerTagsRemove) {
        for (let i = 0; i < playerTagsRemove.length; i++) {
            let tag = playerTagsRemove[i];
            server.RemovePlayerTag({PlayFabId: currentPlayerId, TagName: tag});
        }
    }

    // Add Player Tags
    if (playerTagsAdd) {
        for (let i = 0; i < playerTagsAdd.length; i++) {
            let tag = playerTagsAdd[i];
            server.AddPlayerTag({PlayFabId: currentPlayerId, TagName: tag});
        }
    }


    //------------------------------------------------------------------------------------------------------------
    //	INVENTORIES
    //------------------------------------------------------------------------------------------------------------

    let itemsGrant = args.itemsGrant;
    let itemsCustomData = args.itemsCustomData;
    let itemRevoke = args.itemRevoke;
    let itemsModifyUses = args.itemsModifyUses;

    let itemsGrantRes = null;

    //Items Grant
    if (itemsGrant) {
        let catalogVersion = args.catalogVersion;
        let result = null;
        try {
            if (catalogVersion)
                result = server.GrantItemsToUsers({
                    PlayFabId: currentPlayerId,
                    ItemGrants: itemsGrant,
                    CatalogVersion: catalogVersion
                });
            else
                result = server.GrantItemsToUsers({PlayFabId: currentPlayerId, ItemGrants: itemsGrant});

            itemsGrantRes = [];
            for (let i = 0; i < result.ItemGrantResults.length; i++) {
                let item = {
                    ItemId: result.ItemGrantResults[i].ItemId,
                    ItemInstanceId: result.ItemGrantResults[i].ItemInstanceitemsCustomDataId,
                    ItemClass: result.ItemGrantResults[i].ItemClass,
                    RemainingUses: result.ItemGrantResults[i].RemainingUses,
                    CustomData: result.ItemGrantResults[i].CustomData,
                };
                itemsGrantRes.push(item);
            }
        } catch (ex) {
            server.WriteTitleEvent({EventName: 'cs_error', Body: ex});
            log.debug("cs_error_GrantItemsToUsers: " + ex);
        }
    }

    // Revoke Items
    if (itemRevoke)
        server.RevokeInventoryItems({Items: itemRevoke});

    // Modify Items Use
    if (itemsModifyUses) {
        for (let i = 0; i < itemsModifyUses.length; i++) {
            server.ModifyItemUses({
                PlayFabId: itemsModifyUses[i].PlayFabId,
                ItemInstanceId: itemsModifyUses[i].ItemInstanceId,
                UsesToAdd: itemsModifyUses[i].UsesToAdd
            });
        }
    }

    // Update item CustomData
    if (itemsCustomData) {
        for (let i = 0; i < itemsCustomData.length; i++) {
            server.UpdateUserInventoryItemCustomData({
                PlayFabId: currentPlayerId,
                ItemInstanceId: itemsCustomData[i].itemInstance.ItemInstanceId,
                Data: itemsCustomData[i].itemInstance.CustomData,
                KeysToRemove: itemsCustomData[i].keysRemove
            });
        }
    }

    return {
        errorCode: errorCode,
        VCResults: VCResults,
        itemsGrantResult: itemsGrantRes
    };
};

//------------------------------------------------------------------------------------------------------
//	HELPER METHODS
//------------------------------------------------------------------------------------------------------

Array.prototype.swap = function (x, y) {
    let b = this[x];
    this[x] = this[y];
    this[y] = b;
    return this;
};

function GetCurTimeString() {
    let d = new Date();
    let h = d.getHours();
    let strTime = (d.getMonth() + 1) + "/" + d.getDate() + "/" + d.getFullYear() + " " + h + ":" + d.getMinutes() + ":" + d.getSeconds();
    return strTime;
}

function GetTimeString(d) {
    let h = d.getHours();
    let strTime = (d.getMonth() + 1) + "/" + d.getDate() + "/" + d.getFullYear() + " " + h + ":" + d.getMinutes() + ":" + d.getSeconds();
    return strTime;
}

Date.prototype.addHours = function (h) {
    this.setTime(this.getTime() + (h * 60 * 60 * 1000));
    return this;
};

Date.prototype.addMinutes = function (h) {
    this.setTime(this.getTime() + (h * 60 * 1000));
    return this;
};
