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

    private void Update()
    {
        if (!_raceStarted) return;
        if (_currentTime <= 0) return;

        _currentTime -= Time.deltaTime;
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

        Debug.Log("La course commence !");
        _playerCar._isRacing = true;
        Time.timeScale = 1f;
    }

    public void OnReachPoint()
    {
        Debug.Log("[GameManager] Le joueur a atteint le goal !");

        if (_playerCar != null)
            _playerCar._isRacing = false;
    }

    #endregion

    #region Private & Protected

    [Space(5), Header("Références")]
    [SerializeField] private SpawnManager _spawnManager;

    private CarMovement _playerCar;
    private float m_initialTimer = 60f;
    private bool _raceStarted;

    #endregion
}
