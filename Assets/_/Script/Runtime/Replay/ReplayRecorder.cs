using UnityEngine;
using System.Collections.Generic;
using InputSystem;

[RequireComponent(typeof(CarMovement))]
public class ReplayRecorder : MonoBehaviour
{
    #region Public

    public List<InputFrame> RecordedInputs { get; private set; } = new();
    public List<RewindFrame> RecordedStates { get; private set; } = new();
    #endregion


    #region Unity API

    private void Awake() 
    {
        _actions = new InputSystem_Actions();
        _car = GetComponent<CarMovement>();
    }

    private void OnEnable() => _actions.Enable();

    private void OnDisable() => _actions.Disable();

    private void FixedUpdate()
    {
        switch (_state)
        {
            case State.Recording:
                {
                    float steer = _actions.Player.Move.ReadValue<Vector2>().x;
                    RecordedInputs.Add(new InputFrame { horizontal = steer });
                    RecordedStates.Add(new RewindFrame
                    {
                        position = transform.position,
                        rotation = transform.rotation,
                        linearVelocity = _car.Rb.linearVelocity,
                        angularVelocity = _car.Rb.angularVelocity
                    });
                    break;
                }
            case State.Rewinding:
                {
                    if (_rewindIndex < 0 || _rewindIndex >= RecordedStates.Count)
                    {
                        StopRewind();
                        return;
                    }
                    var state = RecordedStates[_rewindIndex];
                    _car.Rb.position = state.position;
                    _car.Rb.rotation = state.rotation;
                    _car.Rb.linearVelocity = state.linearVelocity;
                    _car.Rb.angularVelocity = state.angularVelocity;
                    _rewindIndex--;
                    if (_rewindIndex < 0)
                        StopRewind();
                    break;
                }
        }
    }

    #endregion


    #region Main API

    public void StartRecording()
    {
        RecordedInputs.Clear();
        RecordedStates.Clear();
        _state = State.Recording;
    }
    public void StopRecording()
    {
        if (_state == State.Recording)
            _state = State.Idle;
    }

    public void StartRewind()
    {
        if (RecordedStates.Count == 0) return;


        _car._isRacing = false;
        _car.SetReplayDriven(true);
        _rewindIndex = RecordedStates.Count - 1;
        _state = State.Rewinding;
    }
    public void StopRewind()
    {
        _car._isRacing = true;
        _state = State.Idle;
        _car.SetReplayDriven(false);
    }

    public bool IsRewinding => _state == State.Rewinding;

    #endregion


    #region Private & Protected

    private bool _isRecording;
    private InputSystem_Actions _actions;
    private State _state = State.Idle;
    private int _rewindIndex = -1;
    private CarMovement _car;
    private enum State
    {
        Idle,
        Recording,
        Rewinding
    }

    [System.Serializable]
    public struct RewindFrame
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
    }

    #endregion
}
