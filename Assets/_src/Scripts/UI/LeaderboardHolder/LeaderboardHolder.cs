using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardHolder : MonoBehaviour
{
    public static LeaderboardData currentLeaderboardData;
    public static int currentPage = 1;
    [SerializeField] private RectTransform entryHolder;
    [SerializeField] private LeaderboardListItem itemPrefab;
    [SerializeField] private LeaderboardListItem currentUser;
    private readonly List<LeaderboardListItem> currentItems = new();
    [SerializeField] private Button buttonNext;
    [SerializeField] private Button buttonBack;

    private void Awake()
    {
        buttonNext.onClick.AddListener(OnNextClicked);
        buttonBack.onClick.AddListener(OnBackClicked);
    }
    private void OnDestroy()
    {
        buttonNext.onClick.RemoveListener(OnNextClicked);
        buttonBack.onClick.RemoveListener(OnBackClicked);
    }

    private async void OnNextClicked()
    {
        try
        {
            buttonNext.interactable = false;
            var response = await PlayFabHandler.GetTopScores(++currentPage, 10);
            currentLeaderboardData = response.data;
            CreateList(currentLeaderboardData);
        }
        catch (Exception e)
        {
            buttonNext.interactable = true;
            Debug.LogError(e);
        }
    }

    private void OnBackClicked()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        RenderView();
    }

    private void RenderView()
    {
        ClearList();
        CreateList(currentLeaderboardData);
    }

    private void CreateList(LeaderboardData data)
    {
        buttonNext.interactable = data.hasNext;
        foreach (var item in data.Leaderboard)
        {
            LeaderboardListItem li = Instantiate(itemPrefab, entryHolder);
            li.SetData(item);
            currentItems.Add(li);
        }
        currentUser.gameObject.SetActive(data.currentUser != null);
        if (data.currentUser != null)
        {
            currentUser.SetData(data.currentUser);
        }
    }

    private void ClearList()
    {
        foreach (var item in currentItems)
        {
            if (item == null) { continue; }
            Destroy(item.gameObject);
        }
        currentItems.Clear();
    }
}
