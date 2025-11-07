// GameEnder.cs
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEnder : MonoBehaviour
{
    public float gameDurationSeconds = 60f;
    public TimerDisplay timer;
    public PauseManager pauseManager;

    [Header("End Screen UI")]
    public GameObject endScreen;               // Panel
    public TMP_Text finalScoreText;
    public TMP_InputField nameInput;

    private bool ended;

    void Start()
    {
        if (endScreen) endScreen.SetActive(false);
        ended = false;
    }

    void Update()
    {
        if (ended || timer == null) return;
        if (timer.GetElapsedSeconds() >= gameDurationSeconds)
            ShowEndScreen();
    }

    void ShowEndScreen()
    {
        ended = true;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        int score = pauseManager != null ? pauseManager.CurrentScore : 0;
        if (finalScoreText) finalScoreText.text = $"Score: {score}";
        if (nameInput) nameInput.text = "";

        if (endScreen) endScreen.SetActive(true);
    }

    public static void Unfreeze()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
