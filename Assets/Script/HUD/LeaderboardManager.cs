using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    private List<SnakeHead3D> players = new List<SnakeHead3D>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(SnakeHead3D player)
    {
        if (!players.Contains(player))
            players.Add(player);
    }

    public void UnregisterPlayer(SnakeHead3D player)
    {
        if (players.Contains(player))
            players.Remove(player);
    }

    public List<(string playerName, int Score)> GetLeaderboard()
    {
        return players
            .Select(p => ($"Player {p.Object.InputAuthority.PlayerId}", p.Score))
            .OrderByDescending(p => p.Score)
            .ToList();
    }
}
