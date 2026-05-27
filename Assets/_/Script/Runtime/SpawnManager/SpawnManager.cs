using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Public

    public CarMovement PlayerCar { get; private set; }

    #endregion

    #region Unity API

    void Awake()
    {
        if (_playerCarPrefab == null)
        {
            Debug.LogError("[SpawnManager] Player car prefab is not assigned.");
            return;
        }

        PlayerCar = SpawnPlayerCar(_currentSpawnIndex);

        //GameObject instance = Instantiate(
        //    _playerCarPrefab,
        //    transform.position,
        //    transform.rotation
        //);

        //PlayerCar = instance.GetComponent<CarMovement>();
        if (PlayerCar == null)
            Debug.LogError("[SpawnManager] Player car prefab is missing CarMovement.");

        if (PlayerCar != null && PlayerCar.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (PlayerCar != null && _cameraFollow != null)
            _cameraFollow.SetTarget(PlayerCar.transform);
    }

    #endregion


    #region Main API

    public Transform GetSpawnPoint(int index) => _spawnPoints[index];

    public GameObject SpawnGhostCar(Vector3 pos,Quaternion rot, List<InputFrame> inputs)
    {
        
        var go = Instantiate(_ghostCarPrefab, pos, rot);
        var replay = go.GetComponent<GosthReplay>();
        replay?.StartReplay(new List<InputFrame>(inputs));
        return go;
    }

    public CarMovement SpawnPlayerCar(int index)
    {
        var spawn = _spawnPoints[index];
        var go = Instantiate(_playerCarPrefab, spawn.position, spawn.rotation);
        // caméra, rigidbody reset...
        return go.GetComponent<CarMovement>();
    }

    #endregion

    #region Private & Protected

    [SerializeField] private GameObject _playerCarPrefab;
    [SerializeField] private CameraFlollow _cameraFollow;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _ghostCarPrefab;
    private int _currentSpawnIndex; 

    #endregion
}
