using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem;

/// <summary>
/// Records every input frame while the player is driving.
/// Must be placed on the player car prefab.
/// Reads input from the New Input System (Player/Move action).
/// </summary>
[RequireComponent(typeof(CarController))]
public class ReplayRecorderTuto : MonoBehaviour
{
    public List<InputFrameTuto> RecordedInputs { get; private set; } = new List<InputFrameTuto>();

    private bool isRecording = false;
    private InputSystem_Actions _actions;

    void Awake()
    {
        _actions = new InputSystem_Actions();
    }

    void OnEnable()  => _actions.Enable();
    void OnDisable() => _actions.Disable();

    public void StartRecording()
    {
        RecordedInputs.Clear();
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    // FixedUpdate is in sync with physics → guarantees an accurate replay
    void FixedUpdate()
    {
        if (!isRecording) return;

        Vector2 move = _actions.Player.Move.ReadValue<Vector2>();

        //RecordedInputs.Add(new InputFrame
        //{
        //    vertical   = move.y,
        //    horizontal = move.x
        //});
    }
}
