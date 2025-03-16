

handlers.IncrementPlayerScore = function (args) {
    if (!currentPlayerId) {
        return { success: false, message: "Invalid player ID!" }
    }
    const { statName = "Score", incrementBy = 1 } = args

    const playerStats = server.GetPlayerStatistics({ PlayFabId: currentPlayerId })
    currentStatValue = 0

    for (let i = 0; i < playerStats.Statistics.length; i++) {
        if (playerStats.Statistics[i].StatisticName === statName) {
            currentStatValue = playerStats.Statistics[i].Value
            break
        }
    }
    const newStatValue = currentStatValue + incrementBy
    const updateRequest = {
        PlayFabId: currentPlayerId,
        Statistics: [{ StatisticName: statName, Value: newStatValue }]
    };

    server.UpdatePlayerStatistics(updateRequest)
    return {
        success: true,
        message: "",
        data: { score: newStatValue }
    }
}

handlers.GetPlayerScore = function (args) {
    const { statName = "Score" } = args

    const playerStats = server.GetPlayerStatistics({ PlayFabId: currentPlayerId })
    currentStatValue = 0
    for (let i = 0; i < playerStats.Statistics.length; i++) {
        if (playerStats.Statistics[i].StatisticName === statName) {
            currentStatValue = playerStats.Statistics[i].Value
            break
        }
    }
    return {
        success: true,
        message: "",
        data: { score: currentStatValue }
    }
}

handlers.GetTopScores = function (args) {
    if (!currentPlayerId) {
        return { success: false, message: "Invalid player ID!" }
    }
    const { statName = "Score", pageIndex = 1, pageSize = 10 } = args
    const startPosition = pageSize * (pageIndex - 1)

    const request = {
        StatisticName: statName,
        StartPosition: startPosition,
        MaxResultsCount: pageSize
    }
    const leaderboardData = server.GetLeaderboard(request)

    const getUserRankRequest = {
        PlayFabId: currentPlayerId,
        StatisticName: statName,
        MaxResultsCount: 1
    }

    const currentUserRankData = server.GetLeaderboardAroundUser(getUserRankRequest)

    const currentUser = currentUserRankData.Leaderboard.length > 0 ?
        currentUserRankData.Leaderboard[0] : null

    const getNextLeaderboardRequest = {
        StatisticName: statName,
        StartPosition: pageSize * pageIndex,
        MaxResultsCount: 1
    }
    const nextLeaderboard = server.GetLeaderboard(getNextLeaderboardRequest)
    const hasNext = nextLeaderboard.Leaderboard.length > 0
    return {
        success: true,
        message: "",
        data: { ...leaderboardData, hasNext, currentUser }
    }
}

handlers.GrantGold = function (args) {
    if (!currentPlayerId) {
        return { success: false, message: "Invalid player ID!" }
    }
    const { amount = 100 } = args
    const request = {
        PlayFabId: currentPlayerId,
        VirtualCurrency: "GO",
        Amount: amount
    }


    server.AddUserVirtualCurrency(request)


    const balanceRequest = {
        PlayFabId: currentPlayerId
    }

    const balanceResult = server.GetUserInventory(balanceRequest)

    return {
        success: true,
        message: "",
        data: {
            gold: balanceResult.VirtualCurrency["GO"] || 0
        }
    }
}

handlers.GetGoldBalance = function (args) {
    if (!currentPlayerId) {
        return { success: false, message: "Invalid player ID!" }
    }
    const balanceRequest = {
        PlayFabId: currentPlayerId
    }
    const balanceResult = server.GetUserInventory(balanceRequest)
    return {
        success: true,
        message: "",
        data: {
            gold: balanceResult.VirtualCurrency["GO"] || 0
        }
    }
}

handlers.PurchaseItem = function (args) {
    const { itemId = "", catalogVersion = null } = args

    if (!currentPlayerId) {
        return { success: false, message: "Invalid player ID!" }
    }


    const catalogItems = server.GetCatalogItems({ CatalogVersion: catalogVersion }).Catalog
    const item = catalogItems.find(i => i.ItemId === itemId)

    if (!item) {
        return { success: false, message: "Item not found in catalog!" }
    }


    const currencyEntries = Object.entries(item.VirtualCurrencyPrices)
    if (currencyEntries.length === 0) {
        return { success: false, message: "Item has no price in any currency!" }
    }

    const [currencyCode, price] = currencyEntries[0]


    const userInventory = server.GetUserInventory({ PlayFabId: currentPlayerId })
    const playerBalance = userInventory.VirtualCurrency[currencyCode] || 0

    if (playerBalance < price) {
        return { success: false, message: "Not enough currency!" }
    }


    server.SubtractUserVirtualCurrency({
        PlayFabId: currentPlayerId,
        VirtualCurrency: currencyCode,
        Amount: price
    })
    const grantResult = server.GrantItemsToUser({
        PlayFabId: currentPlayerId,
        CatalogVersion: catalogVersion,
        ItemIds: [itemId]
    })


    const updatedInventory = server.GetUserInventory({ PlayFabId: currentPlayerId })
    const newBalance = updatedInventory.VirtualCurrency[currencyCode]
    const newItem = grantResult.ItemGrantResults ? grantResult.ItemGrantResults[0] : null

    return {
        success: true,
        message: "Item purchased successfully!",
        data: {
            gold: newBalance,
            newItem: newItem ? {
                instanceId: newItem.ItemInstanceId,
                itemId: newItem.ItemId,
                displayName: newItem.DisplayName
            } : null
        }
    }
}

handlers.GetPlayerInventory = function (args) {

    if (!currentPlayerId) {
        return { success: false, message: "Invalid player ID!" }
    }


    const inventoryData = server.GetUserInventory({ PlayFabId: currentPlayerId })

    return {
        success: true,
        message: "Inventory retrieved successfully!",
        data: inventoryData.Inventory.map(item => ({
            instanceId: item.ItemInstanceId,
            itemId: item.ItemId,
            displayName: item.DisplayName,
        }))
    }
}
