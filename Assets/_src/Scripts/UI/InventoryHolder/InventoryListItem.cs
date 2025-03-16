using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text textItem;
    [SerializeField] private TMP_Text textId;

    public void SetData(InventoryItem item)
    {
        textItem.text = item.displayName;
        textId.text = item.instanceId;
    }
}
