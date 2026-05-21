using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the global level timer.
/// Singleton accessible via TimerManager.Instance.
///
/// Unity Editor setup:
///   - Create a "TimerManager" GameObject in the scene
///   - Assign a UI Text element to the timerText field
/// </summary>
public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Timer")]
    public float startTime = 60f;
    public float CurrentTime { get; private set; }

    [Header("UI")]
    [Tooltip("HUD Text element displaying the remaining time (optional)")]
    public TMP_Text timerText;

    private bool isRunning = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartTimer()
    {
        CurrentTime = startTime;
        isRunning   = true;
    }

    /// <summary>
    /// Adds time to the timer (called when a car reaches its destination).
    /// </summary>
    public void AddTime(float seconds)
    {
        CurrentTime += seconds;
    }

    // Update handles the timer and UI (no physics here)
    void Update()
    {
        if (!isRunning) return;

        CurrentTime -= Time.deltaTime;
        UpdateUI();

        if (CurrentTime <= 0f)
        {
            CurrentTime = 0f;
            isRunning = false;
            GameManager.Instance.GameOver();
        }
    }

    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(CurrentTime).ToString();
    }
}
