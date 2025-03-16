using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text textRank;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textValue;

    public void SetData(LeaderboardEntry leaderboardEntry)
    {
        textRank.text = (leaderboardEntry.Position + 1).ToString();
        textName.text = leaderboardEntry.DisplayName;
        textValue.text = leaderboardEntry.StatValue.ToString();
    }
}
