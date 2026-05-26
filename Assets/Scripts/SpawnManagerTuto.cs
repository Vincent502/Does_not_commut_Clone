using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiates player cars and ghost cars at the correct spawn points.
/// Singleton accessible via SpawnManager.Instance.
///
/// Unity Editor setup:
///   - Create a "SpawnManager" GameObject in the scene
///   - Assign the PlayerCarPrefab and GhostCarPrefab
///   - Fill SpawnPoints in order (index 0 → car 1, index 1 → car 2, etc.)
///
/// PlayerCarPrefab must have: Rigidbody, BoxCollider, CarController, ReplayRecorder
/// GhostCarPrefab must have:  Rigidbody, BoxCollider, CarController, ReplayPlayer
/// </summary>
public class SpawnManagerTuto : MonoBehaviour
{
    public static SpawnManagerTuto Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject playerCarPrefab;
    public GameObject ghostCarPrefab;

    [Header("Spawn Points (same index order as Goal Points)")]
    public Transform[] spawnPoints;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>Instantiates the player car at the given spawn point index.</summary>
    public GameObject SpawnPlayerCar(int index)
    {
        if (!IsValidIndex(index))
        {
            Debug.LogWarning($"[SpawnManager] Index {index} out of range ({spawnPoints.Length} spawn points).");
            return null;
        }

        Transform spawn = spawnPoints[index];
        return Instantiate(playerCarPrefab, spawn.position, spawn.rotation);
    }

    /// <summary>Instantiates a ghost car at the given spawn point index and starts its replay.</summary>
    public GameObject SpawnGhostCar(int index, List<InputFrame> inputs)
    {
        if (!IsValidIndex(index)) return null;

        Transform spawn = spawnPoints[index];
        GameObject ghost = Instantiate(ghostCarPrefab, spawn.position, spawn.rotation);

        ReplayPlayerTuto replay = ghost.GetComponent<ReplayPlayerTuto>();
        if (replay != null)
            replay.StartReplay(inputs);
        else
            Debug.LogError("[SpawnManager] GhostCarPrefab is missing a ReplayPlayer component!");

        return ghost;
    }

    private bool IsValidIndex(int index) => index >= 0 && index < spawnPoints.Length;
}
