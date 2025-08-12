using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;

    void Update()
    {
        if (LeaderboardManager.Instance == null) return;

        var leaderboard = LeaderboardManager.Instance.GetLeaderboard();
        leaderboardText.text = "";

        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboardText.text += $"{i + 1}. {leaderboard[i].playerName} - {leaderboard[i].Score}\n";
        }
    }
}