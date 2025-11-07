using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static bool OpenLeaderboardOnStart = false;

    public GameObject buttonContainer; 
    public GameObject helpPanel;       
    public GameObject leaderboardPanel;

    void Start()
    {
        helpPanel?.SetActive(false);

        if (OpenLeaderboardOnStart)
        {
            OpenLeaderboardOnStart = false;
            OpenLeaderboard();
        }
        else
        {
            CloseLeaderboard();
        }
    }

    public void PlayGame() => SceneManager.LoadScene("SampleScene");
    public void QuitGame() { Application.Quit(); Debug.Log("Game Quit"); }

    public void OpenHelp() { buttonContainer.SetActive(false); helpPanel.SetActive(true); }
    public void CloseHelp() { helpPanel.SetActive(false); buttonContainer.SetActive(true); }

    public void OpenLeaderboard()
    {
        if (leaderboardPanel) leaderboardPanel.SetActive(true);
        if (buttonContainer) buttonContainer.SetActive(false);
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel) leaderboardPanel.SetActive(false);
        if (buttonContainer) buttonContainer.SetActive(true);
    }
}
