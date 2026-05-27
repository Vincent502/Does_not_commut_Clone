using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Public
    
    public static GameManager Instance { get; private set; }

    public float _currentTime { get; private set; }

    #endregion

    #region Unity API

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (_spawnManager != null)
            _playerCar = _spawnManager.PlayerCar;

        if (_playerCar != null)
            _playerCar._isRacing = false;

        Time.timeScale = 0f;
        _currentTime = m_initialTimer;
        _raceStarted = false;
    }

    private void OnEnable()
    {
        CarMovement.OnPlayerSelectedStart += StartRace;
    }


    private void OnDisable()
    {
        CarMovement.OnPlayerSelectedStart -= StartRace;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion

    #region Main API

    private void StartRace()
    {
        if (_raceStarted) return;
        _raceStarted = true;

        if (_playerCar == null)
        {
            Debug.LogWarning("[GameManager] No player car — check SpawnManager prefab.");
            return;
        }

        Debug.Log("Start run !");
        _playerCar._isRacing = true;
        GetRecorder()?.StartRecording();
        Time.timeScale = 1f;
        _timer.StartTimer();
    }

    public void OnTimerExpired()
    {
        if (_gameOver) return;
        _gameOver = true;

        if (_playerCar != null)
            _playerCar._isRacing = false;

        //  Time.timeScale = 0f;  // optionnel : freeze total
        Debug.Log("[GameManager] GAME OVER — time's off !");
    }

    public void OnReachPoint(int goalIndex)
    {
        Debug.Log("[GameManager] player reach the goal !");

        if (_playerCar != null)
            _playerCar._isRacing = false;

        if (_gameOver) return;
        // multi car next here
        var recorder = GetRecorder();
        recorder?.StopRecording();

        _routes.Add(new List<InputFrame>(recorder.RecordedInputs));

        var spawn = _spawnManager.GetSpawnPoint(goalIndex); // ou _currentCarIndex selon ton flow
        _spawnManager.SpawnGhostCar(
            spawn.position,
            spawn.rotation,
            recorder.RecordedInputs
        );

        if (recorder != null)
        {
            Debug.Log($"[Replay] Voiture {_currentCarIndex} → {recorder.RecordedInputs.Count} frames");
        }

        _timer.PauseTime();
    }

    private ReplayRecorder GetRecorder() =>
    _playerCar != null ? _playerCar.GetComponent<ReplayRecorder>() : null;

    #endregion

    #region Private & Protected

    [Space(5), Header("Références")]
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private LevelTimer _timer;

    private CarMovement _playerCar;
    private float m_initialTimer = 60f;
    private bool _raceStarted;
    private bool _gameOver;
    private int _currentCarIndex = 0;
    private readonly List<List<InputFrame>> _routes = new();

    #endregion
}