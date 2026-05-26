using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main game orchestrator. Manages the full gameplay loop:
///   Spawn player → drive → reach goal → ghost car → spawn next player → ...
///
/// Unity Editor setup:
///   - Create a "GameManager" GameObject in the scene
///   - Set timeBonusPerGoal (time added on each successful arrival)
/// </summary>
public class GameManagerTuto : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Time bonus awarded on each successful goal")]
    public float timeBonusPerGoal = 10f;

    private int currentCarIndex = 0;
    private GameObject activePlayerCar;
    private readonly List<GameObject> ghostCars = new List<GameObject>();
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        //Instance = this;
    }

    void Start()
    {
        TimerManager.Instance.StartTimer();
        SpawnNextPlayerCar();
    }

    // ─── Main flow ─────────────────────────────────────────────────────────────

    private void SpawnNextPlayerCar()
    {
        activePlayerCar = SpawnManagerTuto.Instance.SpawnPlayerCar(currentCarIndex);

        if (activePlayerCar == null)
        {
            // No more cars to control → all routes completed (win state)
            Debug.Log("[GameManager] All routes completed!");
            return;
        }

        activePlayerCar.GetComponent<CarController>()?.EnablePlayerControl();
        //activePlayerCar.GetComponent<ReplayRecorder>()?.StartRecording();
    }

    /// <summary>
    /// Called by GoalTrigger when the player car reaches its destination.
    /// </summary>
    public void OnPlayerReachedGoal(CarController car, ReplayRecorder recorder, int goalIndex)
    {
        if (isGameOver) return;

        // Safety check: make sure this is the expected destination
        if (goalIndex != currentCarIndex)
        {
            Debug.LogWarning($"[GameManager] GoalTrigger index {goalIndex} does not match current index {currentCarIndex}.");
            return;
        }

        // 1. Stop player control
        car.DisablePlayerControl();
        //recorder.StopRecording();

        // 2. Award time bonus
        TimerManager.Instance.AddTime(timeBonusPerGoal);

        // 3. Spawn a ghost car at the original spawn point with the recorded inputs
        List<InputFrame> capturedInputs = new List<InputFrame>(recorder.RecordedInputs);
        GameObject ghost = SpawnManagerTuto.Instance.SpawnGhostCar(currentCarIndex, capturedInputs);
        if (ghost != null) ghostCars.Add(ghost);

        // 4. Destroy the player car (the ghost takes over from the spawn point)
        Destroy(activePlayerCar);
        activePlayerCar = null;

        // 5. Move on to the next car
        currentCarIndex++;
        SpawnNextPlayerCar();
    }

    // ─── Game Over ─────────────────────────────────────────────────────────────

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop the player car if it is still active
        if (activePlayerCar != null)
        {
            activePlayerCar.GetComponent<CarController>()?.DisablePlayerControl();
            //activePlayerCar.GetComponent<ReplayRecorder>()?.StopRecording();
        }

        Debug.Log("[GameManager] GAME OVER!");
        // TODO: display the Game Over screen (UI Canvas)
    }
}