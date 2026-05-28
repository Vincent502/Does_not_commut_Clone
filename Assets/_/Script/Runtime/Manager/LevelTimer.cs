using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    #region Public

    public float CurrentTime { get; private set; }

    #endregion
    

    #region Unity API

    private void Start()
    {
        RefreshUI();
    }

    private void Update()
    {
        if (!_isRunning) return;

        if (_isRewinding)
        {
            CurrentTime += Time.deltaTime;
            RefreshUI();
        }
        else
        {
            CurrentTime -= Time.deltaTime; 
            if (CurrentTime <= 0f)
            {
                CurrentTime = 0f;
                _isRunning = false;
                RefreshUI();
                _gameManager?.OnTimerExpired();
                return;
            }
        }

        RefreshUI();
    }

    #endregion


    #region Main API

    public void StartTimer()
    {
        CurrentTime = _startTime;
        _isRunning = true;
        RefreshUI();
    }

    public void ResumeTime()
    {
        if (CurrentTime > 0f)
            _isRunning = true;
    }

    private void RefreshUI()
    {
        if (_timerText != null)
            _timerText.text = Mathf.CeilToInt(CurrentTime).ToString();
    }

    public void AddTime(float seconds) => CurrentTime += seconds;

    public void PauseTime() => _isRunning = false;

    public void StartRewindTime()
    {
        _isRewinding = true;
        _isRunning = true;
    }

    public void StopRewindTime()
    {
        _isRewinding = false;
        _isRunning = false;
        CurrentTime = CurrentTime - 1f;
        RefreshUI();
    }

    #endregion


    #region Private & Protected

    [SerializeField] private float _startTime = 60f;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameManager _gameManager;
    private bool _isRunning;
    private bool _isRewinding;

    #endregion
}