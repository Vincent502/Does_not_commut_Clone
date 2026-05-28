using System.Collections.Generic;
using UnityEngine;
using InputSystem;

public class GameManager : MonoBehaviour
{
    #region Public
    
    public static GameManager Instance { get; private set; }

    public float _currentTime { get; private set; }

    #endregion

    #region Unity API

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        m_interacte = new InputSystem_Actions();
    }

    private void Start()
    {
        if (_spawnManager != null)
            _playerCar = _spawnManager.PlayerCar;

        if (_playerCar != null)
            _playerCar._isRacing = false;

        Time.timeScale = 0f;
        _currentTime = m_initialTimer;
        _raceStarted = false;
    }

    private void Update()
    {
        if (m_interacte.Player.Interact.WasPerformedThisFrame())
        {
            _timer.StartRewindTime();
            foreach (var ghost in _activeGhosts)
                ghost?.StartRewind();
            var playerRecorder = GetRecorder();
            if (playerRecorder != null)
            {
                _playerCar._isRacing = false; 
                playerRecorder.StopRecording(); 
                playerRecorder.StartRewind();   
            }
            _isRewindingAll = true;
            _raceStarted = true;      
            _playerCar._isRacing = false;
                        
        }
        if (_isRewindingAll)
        {
            bool playerDone = true;
            var recorder = GetRecorder();
            if (recorder != null) playerDone = !recorder.IsRewinding;

            bool ghostsDone = true;
            foreach (var ghost in _activeGhosts)
            {
                if (ghost != null && ghost.IsRewinding)
                {
                    ghostsDone = false;
                    break;
                }
            }

            if (playerDone && ghostsDone)
            {
                _timer.StopRewindTime();
                _isRewindingAll = false;
                _raceStarted = false;      
                if (_playerCar != null) _playerCar._isRacing = false; 
            }
        }
    }

    private void OnEnable()
    {
        CarMovement.OnPlayerSelectedStart += StartRace;
        m_interacte.Enable();
    }


    private void OnDisable()
    {
        CarMovement.OnPlayerSelectedStart -= StartRace;
        m_interacte.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion

    #region Main API

    private void StartRace()
    {
        if (_gameOver) return;
        if (_raceStarted) return;
        if (_playerCar == null) return;

        _raceStarted = true;
        _playerCar._isRacing = true;

        _timer.StopRewindTime(); 

        GetRecorder()?.StartRecording();

        ClearGhosts();
        RebuildGhostsFromRoutes();

        foreach (var ghost in _activeGhosts)
            ghost.StartReplay();
        if(_currentCarIndex == 0) _timer.StartTimer();
        else _timer.ResumeTime();
        Time.timeScale = 1f;
    }

    private void ClearGhosts()
    {
        foreach (var g in _activeGhosts)
            if (g != null) Destroy(g.gameObject);
        _activeGhosts.Clear();
    }

    private void RebuildGhostsFromRoutes()
    {
        for (int i = 0; i < _routes.Count; i++)
        {
            var spawn = _spawnManager.GetSpawnPoint(i);
            var ghost = _spawnManager.SpawnGhostCar(spawn.position, spawn.rotation, _routes[i]);
            if (ghost != null) _activeGhosts.Add(ghost);
        }
    }

    private ReplayRecorder GetRecorder() =>
    _playerCar != null ? _playerCar.GetComponent<ReplayRecorder>() : null;

    public void OnTimerExpired()
    {
        if (_gameOver) return;
        _gameOver = true;

        if (_playerCar != null)
            _playerCar._isRacing = false;

        Time.timeScale = 0f;
        Debug.Log("[GameManager] GAME OVER — time's off !");
    }

    public void OnReachPoint(int goalIndex)
    {
        
        if (_gameOver) return;
        if (_playerCar == null) return;
        if (goalIndex != _currentCarIndex) return; 
                                                   
        _playerCar._isRacing = false;
        var recorder = GetRecorder();
        recorder?.StopRecording();
        _timer.PauseTime();
        if (recorder == null) return;

        
        var captured = new List<InputFrame>(recorder.RecordedInputs);

        _routes.Add(captured);

        var ghostSpawn = _spawnManager.GetSpawnPoint(_currentCarIndex);

        var ghost = _spawnManager.SpawnGhostCar(ghostSpawn.position, ghostSpawn.rotation, captured);
        if (ghost != null) _activeGhosts.Add(ghost);

        Destroy(_playerCar.gameObject);
        _timer.AddTime(10);
        _playerCar = null;

        
        _currentCarIndex++;

        if (_currentCarIndex >= _spawnManager.SpawnPointCount)
        {
            Debug.Log("[GameManager] All routes completed! WIN");
            _gameOver = true;          
            _raceStarted = false;
            _timer.PauseTime();        
            Time.timeScale = 0f;       
            return;
        }

        _playerCar = _spawnManager.SpawnPlayerCar(_currentCarIndex);
        if (_playerCar == null)
        {
            Debug.LogError("[GameManager] Failed to spawn next player car.");
            return;
        }
        
        _playerCar._isRacing = false;   
        _raceStarted = false;           
        _spawnManager.FocusCameraOn(_playerCar.transform); 
                                                           
    }

    public int CurrentCarIndex => _currentCarIndex;

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
    private bool _isRewindingAll = false;

    private readonly List<List<InputFrame>> _routes = new();
    private readonly List<GosthReplay> _activeGhosts = new();

    private InputSystem_Actions m_interacte;



    #endregion
}