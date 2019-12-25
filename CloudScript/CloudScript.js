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

    args = JSON.parse(args);
    //------------------------------------------------------------------------------------------------------------
    //	VIRTUAL CURRENCIES
    //------------------------------------------------------------------------------------------------------------
    let VCResults = [];
    let currencyReward = args.CurrencyReward;
    if(currencyReward){
        Object.entries(currencyReward).forEach(([vc_code, vc_value])=>{
            log.info(`${vc_code}:${vc_value}`)

            let result = server.AddUserVirtualCurrency({
                PlayFabId: currentPlayerId,
                VirtualCurrency: vc_code,
                Amount: vc_value
            });
            VCResults.push({
                VCName: result.VirtualCurrency,
                balance: result.Balance,
                balanceChange: result.BalanceChange
            });
        })
    }

    let currencyDecrease = args.CurrencyDecrease;
    if(currencyDecrease){
        Object.entries(currencyDecrease).forEach(([vc_code, vc_value])=>{
            console.log(`${vc_code}:${vc_value}`)

            let result = server.SubtractUserVirtualCurrency({
                PlayFabId: currentPlayerId,
                VirtualCurrency: vc_code,
                Amount: vc_value
            });
            VCResults.push({
                VCName: result.VirtualCurrency,
                balance: result.Balance,
                balanceChange: result.BalanceChange
            });
        })
    }


    //------------------------------------------------------------------------------------------------------------
    //	STATISTICS
    //------------------------------------------------------------------------------------------------------------
    let Statistic = args.Statistic;
    if (Statistic)
        server.UpdatePlayerStatistics({PlayFabId: currentPlayerId, Statistics: Statistic});


    //------------------------------------------------------------------------------------------------------------
    //	USER TITLE DATA
    //------------------------------------------------------------------------------------------------------------
    let ReadOnlyData = args.ReadOnlyData;
    if (ReadOnlyData)
        server.UpdateUserReadOnlyData({PlayFabId: currentPlayerId, Data: ReadOnlyData, Permission: 1});

    let InternalData = args.InternalData;
    if (InternalData)
        server.UpdateUserInternalData({PlayFabId: currentPlayerId, Data: InternalData, Permission: 1});


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

    let ItemsGrant = args.ItemsGrant;
    let ItemsCustomData = args.ItemsCustomData;
    let ItemRevoke = args.ItemRevoke;
    let ItemsModifyUses = args.ItemsModifyUses;

    let itemsGrantRes = null;

    //Items Grant
    if (ItemsGrant) {
        let catalogVersion = args.catalogVersion;
        let result = null;
        try {
            if (catalogVersion)
                result = server.GrantItemsToUsers({
                    PlayFabId: currentPlayerId,
                    ItemGrants: ItemsGrant,
                    CatalogVersion: catalogVersion
                });
            else
                result = server.GrantItemsToUsers({PlayFabId: currentPlayerId, ItemGrants: ItemsGrant});

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
    if (ItemRevoke)
        server.RevokeInventoryItems({Items: ItemRevoke});

    // Modify Items Use
    if (ItemsModifyUses) {
        for (let i = 0; i < ItemsModifyUses.length; i++) {
            server.ModifyItemUses({
                PlayFabId: ItemsModifyUses[i].PlayFabId,
                ItemInstanceId: ItemsModifyUses[i].ItemInstanceId,
                UsesToAdd: ItemsModifyUses[i].UsesToAdd
            });
        }
    }

    // Update item CustomData
    if (ItemsCustomData) {
        for (let i = 0; i < ItemsCustomData.length; i++) {
            server.UpdateUserInventoryItemCustomData({
                PlayFabId: currentPlayerId,
                ItemInstanceId: ItemsCustomData[i].itemInstance.ItemInstanceId,
                Data: ItemsCustomData[i].itemInstance.CustomData,
                KeysToRemove: ItemsCustomData[i].keysRemove
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
