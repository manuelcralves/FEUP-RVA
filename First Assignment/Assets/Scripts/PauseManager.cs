using UnityEngine;
using UnityEngine.SceneManagement;  
using Vuforia;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Full-screen panel that shows while paused")]
    public GameObject pauseMenu;
    [Tooltip("The top-right Pause button object")]
    public GameObject pauseButton;        
    public TimerDisplay timer;
    public bool IsPaused { get; private set; }
    [Header("Game Stats")]
    public TMP_Text ballsAliveText;
    public TMP_Text scoreText;

private int score = 0;
public int CurrentScore => score;




    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (pauseMenu)  pauseMenu.SetActive(false);
        if (pauseButton) pauseButton.SetActive(true);
        IsPaused = false;
    }

    void OnEnable()
    {
        BallPop.OnAnyBallPopped += HandleBallPopped;
    }

    void OnDisable()
    {
        BallPop.OnAnyBallPopped -= HandleBallPopped;
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    // --- PAUSE ---
    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;

        // Freeze gameplay & audio
        Time.timeScale = 0f;
        AudioListener.pause = true;

        // UI
        if (pauseMenu)  pauseMenu.SetActive(true);
        if (pauseButton) pauseButton.SetActive(false);
        if (timer) timer.PauseTimer();

        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.enabled = false;

        }
    }

    // --- RESUME ---
    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseMenu) pauseMenu.SetActive(false);
        if (pauseButton) pauseButton.SetActive(true);
        if (timer) timer.ResumeTimer();

        if (VuforiaBehaviour.Instance != null)
        {
            VuforiaBehaviour.Instance.enabled = true;

        }    
        }
    
    void HandleBallPopped(int points)
    {
        score += points;
    }
    void UpdateStatsUI()
    {
        int ballsAlive = GameObject.FindGameObjectsWithTag("Ball").Length;

        if (ballsAliveText) ballsAliveText.text = $"Balls Alive: {ballsAlive}";
        if (scoreText) scoreText.text = $"Score: {score}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
        UpdateStatsUI();

    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !IsPaused)
            Pause();
    }

    public void QuitToMenu()
{
    Time.timeScale = 1f;
    AudioListener.pause = false;
    if (timer) timer.ResetTimer();

    if (VuforiaBehaviour.Instance != null)
        VuforiaBehaviour.Instance.enabled = true;

    SceneManager.LoadScene("MainMenu");
}
}
