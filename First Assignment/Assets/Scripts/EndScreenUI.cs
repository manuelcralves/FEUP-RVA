using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    public PauseManager pauseManager;
    public TMP_InputField nameInput;

    public void Submit()
    {
        string playerName = string.IsNullOrWhiteSpace(nameInput?.text) ? "Player" : nameInput.text.Trim();
        int score = pauseManager != null ? pauseManager.CurrentScore : 0;

        Leaderboard.AddScore(playerName, score);

        GameEnder.Unfreeze();

        MainMenu.OpenLeaderboardOnStart = true;

        SceneManager.LoadScene("MainMenu");
    }
}
