using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LeaderboardEntry {
    public string name;
    public int score;
    public LeaderboardEntry(string name, int score) { this.name = name; this.score = score; }
}

[Serializable]
public class LeaderboardData {
    public List<LeaderboardEntry> entries = new();
}

public static class Leaderboard
{
    private const string Key = "LEADERBOARD_V1";

    public static LeaderboardData Load()
    {
        if (!PlayerPrefs.HasKey(Key)) return new LeaderboardData();
        var json = PlayerPrefs.GetString(Key);
        return JsonUtility.FromJson<LeaderboardData>(json) ?? new LeaderboardData();
    }

    public static void Save(LeaderboardData data)
    {
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(Key, json);
        PlayerPrefs.Save();
    }

    public static void AddScore(string name, int score, int keepTop = 5)
    {
        var data = Load();
        data.entries.Add(new LeaderboardEntry(name, score));
        data.entries = data.entries
            .OrderByDescending(e => e.score)
            .ThenBy(e => e.name)
            .Take(keepTop)
            .ToList();
        Save(data);
    }
}
