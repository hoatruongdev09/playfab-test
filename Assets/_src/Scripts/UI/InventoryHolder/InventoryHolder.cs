using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHolder : MonoBehaviour
{
    public static List<InventoryItem> items = new();
    [SerializeField] private TMP_Text textGold;
    [SerializeField] private Button buttonGrantGold;

    [SerializeField] private Button buttonBack;
    [SerializeField] private Button buttonPurchaseSword;
    [SerializeField] private Button buttonPurchaseShield;

    [SerializeField] private RectTransform itemHolder;
    [SerializeField] private InventoryListItem listItemPrefab;
    private readonly List<InventoryListItem> listItems = new();

    private void Awake()
    {
        buttonBack.onClick.AddListener(OnBackClicked);
        buttonPurchaseSword.onClick.AddListener(OnPurchaseSword);
        buttonPurchaseShield.onClick.AddListener(OnPurchaseShield);
        buttonGrantGold.onClick.AddListener(OnGrantGoldClicked);
    }

    private void OnDestroy()
    {
        buttonBack.onClick.RemoveListener(OnBackClicked);
        buttonPurchaseSword.onClick.RemoveListener(OnPurchaseSword);
        buttonPurchaseShield.onClick.RemoveListener(OnPurchaseShield);
        buttonGrantGold.onClick.RemoveListener(OnGrantGoldClicked);
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

    private void OnEnable()
    {
        RenderView();
    }

    private void RenderView()
    {
        SetGoldText(PlayFabHandler.playerGoldBalanceData.gold);
        RenderListView();
    }

    private void RenderListView()
    {
        ClearListView();
        CreateListItems();
    }

    private void CreateListItems()
    {
        foreach (var item in items)
        {
            AddItemUI(item);
        }
    }

    private void AddItemUI(InventoryItem item)
    {
        InventoryListItem li = Instantiate(listItemPrefab, itemHolder);
        li.SetData(item);
        listItems.Add(li);
    }

    private void ClearListView()
    {
        foreach (var item in listItems)
        {
            if (item == null) { continue; }
            Destroy(item.gameObject);
        }
        listItems.Clear();
    }

    private void SetGoldText(int gold)
    {
        textGold.text = $"Gold {gold}";
    }

    private void OnBackClicked()
    {
        gameObject.SetActive(false);
    }

    private async void OnPurchaseSword()
    {
        try
        {
            buttonPurchaseSword.interactable = false;
            var result = await PlayFabHandler.PurchaseItem("wpn_sword");
            buttonPurchaseSword.interactable = true;
            if (!result.success)
            {
                Debug.LogError(result.message);
                return;
            }
            SetGoldText(result.data.gold);
            AddItemUI(result.data.newItem);
        }
        catch (Exception e)
        {
            buttonPurchaseSword.interactable = true;
            Debug.LogException(e);
        }
    }



    private async void OnPurchaseShield()
    {
        try
        {
            buttonPurchaseShield.interactable = false;
            var result = await PlayFabHandler.PurchaseItem("wpn_shield");
            buttonPurchaseShield.interactable = true;
            if (!result.success)
            {
                Debug.LogError(result.message);
                return;
            }
            SetGoldText(result.data.gold);
            AddItemUI(result.data.newItem);
        }
        catch (Exception e)
        {
            buttonPurchaseShield.interactable = true;
            Debug.LogException(e);
        }
    }
}
