using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatically replays a recorded sequence of inputs.
/// Must be placed on the ghost car prefab.
/// </summary>
[RequireComponent(typeof(CarController))]
public class ReplayPlayerTuto : MonoBehaviour
{
    private List<InputFrame> replayData;
    private CarController car;
    private int currentFrame = 0;
    private bool isReplaying = false;

    // Awake (not Start) so the reference is ready before SpawnManager calls StartReplay
    void Awake()
    {
        car = GetComponent<CarController>();
    }

    /// <summary>
    /// Starts the replay with the provided input data.
    /// Called by SpawnManager immediately after instantiation.
    /// </summary>
    public void StartReplay(List<InputFrame> data)
    {
        replayData = new List<InputFrame>(data);
        currentFrame = 0;
        isReplaying = true;
    }

    void FixedUpdate()
    {
        if (!isReplaying || replayData == null) return;

        if (currentFrame >= replayData.Count)
        {
            // Replay finished, car comes to a stop
            isReplaying = false;
            return;
        }

        InputFrame frame = replayData[currentFrame];
        //car.ApplyMovement(frame.vertical, frame.horizontal);
        currentFrame++;
    }
}
