using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText; 

    private float elapsed;        
    private bool running = true;     

    void Awake()
    {
        elapsed = 0f;
        running = true;
        UpdateLabel();
    }

    void Update()
    {
        if (!running) return;

        elapsed += Time.deltaTime;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);
        int hundredths = Mathf.FloorToInt((elapsed - Mathf.Floor(elapsed)) * 100f);
        if (timerText)
            timerText.text = $"{minutes:00}:{seconds:00}.{hundredths:00}";
    }

    public void PauseTimer()  { running = false; }
    public void ResumeTimer() { running = true; }
    public void ResetTimer()
    {
        elapsed = 0f;
        UpdateLabel();
    }

    public float GetElapsedSeconds() => elapsed;
}
