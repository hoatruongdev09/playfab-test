using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject loginHolder;
    [SerializeField] private Button buttonLogin;

    [Space]
    [SerializeField] private GameObject loggedHolder;
    [SerializeField] private TMP_Text textID;
    [SerializeField] private TMP_Text textName;
    [Space]
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private Button buttonChangeName;
    [Space]
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private Button buttonIncreaseKill;
    [SerializeField] private TMP_Text textGold;
    [SerializeField] private Button buttonGrantGold;

    [Space]
    [SerializeField] private Button buttonOpenLeaderboard;
    [SerializeField] private LeaderboardHolder leaderboardHolder;
    [Space]
    [SerializeField] private Button buttonOpenInventory;
    [SerializeField] private InventoryHolder inventoryHolder;

    private void Awake()
    {
        buttonLogin.onClick.AddListener(OnLoginClicked);
        buttonChangeName.onClick.AddListener(OnChangeNameClicked);
        buttonIncreaseKill.onClick.AddListener(OnInCreaseScoreClicked);
        buttonOpenLeaderboard.onClick.AddListener(OnOpenLeaderboardClicked);
        buttonGrantGold.onClick.AddListener(OnGrantGoldClicked);
        buttonOpenInventory.onClick.AddListener(OnOpenInventory);
    }


    private void OnDestroy()
    {
        buttonLogin.onClick.RemoveListener(OnLoginClicked);
        buttonChangeName.onClick.RemoveListener(OnChangeNameClicked);
        buttonIncreaseKill.onClick.RemoveListener(OnInCreaseScoreClicked);
        buttonOpenLeaderboard.onClick.RemoveListener(OnOpenLeaderboardClicked);
        buttonGrantGold.onClick.RemoveListener(OnGrantGoldClicked);
        buttonOpenInventory.onClick.RemoveListener(OnOpenInventory);
    }

    private async void OnOpenInventory()
    {
        try
        {
            var data = await PlayFabHandler.GetPlayerInventory();
            InventoryHolder.items = data.data;
            inventoryHolder.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void OnGrantGoldClicked()
    {
        try
        {
            buttonGrantGold.interactable = false;
            var response = await PlayFabHandler.GrantGold(100);
            PlayFabHandler.playerGoldBalanceData = response.data;
            SetGoldText(PlayFabHandler.playerGoldBalanceData.gold);
            buttonGrantGold.interactable = true;
        }
        catch (Exception e)
        {
            buttonGrantGold.interactable = true;
            Debug.LogException(e);
        }
    }

    private async void OnLoginClicked()
    {
        try
        {
            buttonLogin.interactable = false;
            await PlayFabHandler.DoGuestLogin();
            await PlayFabHandler.FetchPlayerGoldData();
            await PlayFabHandler.FetchAccountInfo();
            await PlayFabHandler.FetchStatisticData();

            textID.text = $"PlayFabID: {PlayFabHandler.playFabId}";
            var displayName = PlayFabHandler.accountInfo.AccountInfo.TitleInfo.DisplayName;
            int scoreValue = PlayFabHandler.playerStatistic.Statistics.FirstOrDefault(stat => stat.StatisticName == "Score")?.Value ?? 0;

            SetScoreText(scoreValue);
            SetGoldText(PlayFabHandler.playerGoldBalanceData.gold);
            textName.text = string.IsNullOrEmpty(displayName) ? "DisplayName: Not Set" : $"DisplayName: {displayName}";
            loginHolder.SetActive(false);
            loggedHolder.SetActive(true);
            buttonLogin.interactable = true;
        }
        catch (Exception e)
        {
            buttonLogin.interactable = true;
            Debug.LogException(e);
        }
    }

    private void SetGoldText(int gold)
    {
        textGold.text = $"Gold: {gold}";
    }

    private void SetScoreText(int scoreValue)
    {
        textScore.text = $"Scores: {scoreValue}";
    }

    private async void OnChangeNameClicked()
    {
        string displayName = inputName.text;
        if (string.IsNullOrEmpty(displayName)) { return; }
        try
        {
            buttonChangeName.interactable = false;
            await PlayFabHandler.UpdateDisplayName(displayName);
            textName.text = $"DisplayName: {displayName}";
            buttonChangeName.interactable = true;
        }
        catch (Exception e)
        {
            buttonChangeName.interactable = true;
            Debug.LogException(e);
        }
    }



    private async void OnInCreaseScoreClicked()
    {
        try
        {
            buttonIncreaseKill.interactable = false;
            var response = await PlayFabHandler.IncrementScore(1);
            SetScoreText(response.data.score);
            buttonIncreaseKill.interactable = true;
        }
        catch (Exception e)
        {
            buttonIncreaseKill.interactable = true;
            Debug.LogException(e);
        }
    }

    private async void OnOpenLeaderboardClicked()
    {
        try
        {
            buttonOpenLeaderboard.interactable = false;
            var response = await PlayFabHandler.GetTopScores(LeaderboardHolder.currentPage, 10);
            LeaderboardHolder.currentLeaderboardData = response.data;
            leaderboardHolder.gameObject.SetActive(true);
            buttonOpenLeaderboard.interactable = true;
        }
        catch (Exception e)
        {
            buttonOpenLeaderboard.interactable = true;
            Debug.LogException(e);
        }
    }


}
