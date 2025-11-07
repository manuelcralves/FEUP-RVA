using UnityEngine;
using TMPro;
using System.Collections;

public class LeaderboardUI : MonoBehaviour
{
    public TMP_Text[] rows; 

    IEnumerator Start()
    {
        yield return null;
        Refresh();
    }

    public void Refresh()
    {
        var data = Leaderboard.Load();

        for (int i = 0; i < rows.Length; i++)
        {
            if (!rows[i]) continue;

            if (i < data.entries.Count)
            {
                var e = data.entries[i];
                rows[i].text = $"{i + 1}. {e.name} — {e.score}";
            }
            else
            {
                rows[i].text = $"{i + 1}. ---";
            }
        }
    }

    public void ClearAll()
    {
        PlayerPrefs.DeleteKey("LEADERBOARD_V1");
        PlayerPrefs.Save();
        Refresh();
    }
}
