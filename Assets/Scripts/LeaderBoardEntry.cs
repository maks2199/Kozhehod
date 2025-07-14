using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string name, int score)
    {
        this.playerName = name;
        this.score = score;
    }
    public override string ToString()
    {
        return $"{playerName}: {score}";
    }
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    public string EntriesToString()
    {
        // Sort in-place by descending score
        entries.Sort((a, b) => b.score.CompareTo(a.score));
        return string.Join("\n", entries);
    }
}
