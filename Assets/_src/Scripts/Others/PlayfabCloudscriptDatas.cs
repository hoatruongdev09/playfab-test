using System;
using System.Security;
using System.Threading;

[Serializable]
public class ResponseData<T>
{
    public bool success;
    public string message;
    public T data;
}

[Serializable]
public class PlayerScore
{
    public int score;
}

[Serializable]
public class LeaderboardEntry
{
    public string DisplayName;
    public int Position;
    public int StatValue;
}

[Serializable]
public class LeaderboardData
{
    public LeaderboardEntry[] Leaderboard;
    public bool hasNext = false;
    public LeaderboardEntry currentUser;
}


[Serializable]
public class GoldBalanceData
{
    public int gold;
}

[Serializable]
public class InventoryItem
{
    public string instanceId;
    public string itemId;
    public string displayName;
}


[Serializable]
public class PurchaseItemData
{
    public int gold;
    public InventoryItem newItem;
}


