using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public static class PlayFabHandler
{
    public static string playFabId = string.Empty;
    public static GetPlayerStatisticsResult playerStatistic;
    public static GetAccountInfoResult accountInfo;
    public static GoldBalanceData playerGoldBalanceData;

    public static async Task DoGuestLogin()
    {

        var loginResult = await GuestLogin();
        playFabId = loginResult.PlayFabId;
    }

    public static async Task FetchAccountInfo()
    {
        accountInfo = await GetAccountInfo();
    }

    public static async Task FetchStatisticData()
    {
        playerStatistic = await GetPlayerStatistics();
    }

    public static async Task FetchPlayerGoldData()
    {
        var response = await GetGoldBalance();
        playerGoldBalanceData = response.data;
    }
    public static async Task<LoginResult> GuestLogin()
    {
        TaskCompletionSource<LoginResult> tsc = new();
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        var request = new LoginWithCustomIDRequest()
        {
            CustomId = deviceId,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request,
            (result) =>
            {
                tsc.SetResult(result);
            },
            error =>
            {
                tsc.SetException(new Exception(error.ErrorMessage));
            }
        );

        return await tsc.Task;
    }

    public static async Task<UpdateUserTitleDisplayNameResult> UpdateDisplayName(string name)
    {
        TaskCompletionSource<UpdateUserTitleDisplayNameResult> tsc = new();
        var request = new UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
        {
            tsc.SetResult(result);
        }, error =>
        {
            tsc.SetException(new Exception(error.ErrorMessage));
        });

        return await tsc.Task;
    }

    public static async Task<GetAccountInfoResult> GetAccountInfo()
    {
        TaskCompletionSource<GetAccountInfoResult> tsc = new();
        var accountInfoRequest = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(accountInfoRequest, result =>
        {
            tsc.SetResult(result);
        }, error =>
        {
            tsc.SetException(new Exception(error.ErrorMessage));
        });
        return await tsc.Task;
    }

    public static async Task<ResponseData<PlayerScore>> IncrementScore(int amount)
    {
        TaskCompletionSource<ResponseData<PlayerScore>> tsc = new();

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "IncrementPlayerScore",
            FunctionParameter = new { statName = "Score", incrementBy = amount },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            string data = result.FunctionResult.ToString();
            ResponseData<PlayerScore> score = JsonUtility.FromJson<ResponseData<PlayerScore>>(data);
            tsc.SetResult(score);
        }, error =>
        {
            tsc.SetException(new Exception(error.ErrorMessage));
        });
        return await tsc.Task;
    }
    public static async Task<GetPlayerStatisticsResult> GetPlayerStatistics()
    {
        TaskCompletionSource<GetPlayerStatisticsResult> tcs = new();

        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            result =>
            {
                tcs.SetResult(result);
            },
            error =>
            {
                tcs.SetException(new Exception(error.ErrorMessage));
            });

        return await tcs.Task;
    }

    public static async Task<ResponseData<LeaderboardData>> GetTopScores(int pageIndex = 1, int pageSize = 100)
    {
        TaskCompletionSource<ResponseData<LeaderboardData>> tcs = new();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetTopScores",
            FunctionParameter = new { statName = "Score", pageIndex, pageSize },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            string data = result.FunctionResult.ToString();
            ResponseData<LeaderboardData> leaderboard = JsonUtility.FromJson<ResponseData<LeaderboardData>>(data.ToString());
            tcs.SetResult(leaderboard);
        }, error =>
        {
            tcs.SetException(new Exception(error.ErrorMessage));
        });
        return await tcs.Task;
    }

    public static async Task<ResponseData<GoldBalanceData>> GetGoldBalance()
    {
        TaskCompletionSource<ResponseData<GoldBalanceData>> tcs = new();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetGoldBalance",
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            string data = result.FunctionResult.ToString();
            ResponseData<GoldBalanceData> goldData = JsonUtility.FromJson<ResponseData<GoldBalanceData>>(data.ToString());
            tcs.SetResult(goldData);
        }, error =>
        {
            tcs.SetException(new Exception(error.ErrorMessage));
        });
        return await tcs.Task;
    }

    public static async Task<ResponseData<GoldBalanceData>> GrantGold(int amount)
    {
        TaskCompletionSource<ResponseData<GoldBalanceData>> tcs = new();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GrantGold",
            GeneratePlayStreamEvent = true,
            FunctionParameter = new { amount },
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            string data = result.FunctionResult.ToString();
            ResponseData<GoldBalanceData> goldData = JsonUtility.FromJson<ResponseData<GoldBalanceData>>(data.ToString());
            tcs.SetResult(goldData);
        }, error =>
        {
            tcs.SetException(new Exception(error.ErrorMessage));
        });
        return await tcs.Task;
    }

    public static async Task<ResponseData<PurchaseItemData>> PurchaseItem(string itemId)
    {
        TaskCompletionSource<ResponseData<PurchaseItemData>> tcs = new();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "PurchaseItem",
            FunctionParameter = new { itemId },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            string data = result.FunctionResult.ToString();
            ResponseData<PurchaseItemData> responseData = JsonUtility.FromJson<ResponseData<PurchaseItemData>>(data.ToString());
            tcs.SetResult(responseData);
        }, error =>
        {
            tcs.SetException(new Exception(error.ErrorMessage));
        });
        return await tcs.Task;
    }

    public static async Task<ResponseData<List<InventoryItem>>> GetPlayerInventory()
    {
        TaskCompletionSource<ResponseData<List<InventoryItem>>> tcs = new();
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetPlayerInventory",
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            string data = result.FunctionResult.ToString();
            ResponseData<List<InventoryItem>> responseData = JsonUtility.FromJson<ResponseData<List<InventoryItem>>>(data.ToString());
            tcs.SetResult(responseData);
        }, error =>
        {
            tcs.SetException(new Exception(error.ErrorMessage));
        });
        return await tcs.Task;
    }
}
