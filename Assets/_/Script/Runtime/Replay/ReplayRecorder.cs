using UnityEngine;
using System.Collections.Generic;
using InputSystem;

[RequireComponent(typeof(CarMovement))]
public class ReplayRecorder : MonoBehaviour
{
    #region Public
    public List<InputFrame> RecordedInputs { get; private set; } = new();

    #endregion


    #region Unity API

    void Awake() => _actions = new InputSystem_Actions();
    void OnEnable() => _actions.Enable();
    void OnDisable() => _actions.Disable();

    void FixedUpdate()
    {
        if (!_isRecording) return;
        float steer = _actions.Player.Move.ReadValue<Vector2>().x;
        RecordedInputs.Add(new InputFrame { horizontal = steer });
    }

    #endregion


    #region Main API

    public void StartRecording()
    {
        RecordedInputs.Clear();
        _isRecording = true;
    }
    public void StopRecording() => _isRecording = false;

    #endregion


    #region Private & Protected

    private bool _isRecording;
    private InputSystem_Actions _actions;

    #endregion
}
