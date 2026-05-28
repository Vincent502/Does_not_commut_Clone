using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarMovement))]
public class GosthReplay : MonoBehaviour
{

    #region Unity API

    private void Awake()
    {
        _car = GetComponent<CarMovement>();
    }

    private void FixedUpdate()
    {
        if (_mode == ReplayMode.Stopped || _data == null || _data.Count == 0)
            return;

        if (_mode == ReplayMode.Forward)
        {
            if (_frameIndex < 0 || _frameIndex >= _data.Count)
            {
                StopReplay();
                return;
            }

            var frame = _data[_frameIndex];
            _car.ApplyReplayFrame(frame.horizontal, 1f);

            _recordedStates.Add(new RewindFrame
            {
                position = _car.Rb.position,
                rotation = _car.Rb.rotation,
                linearVelocity = _car.Rb.linearVelocity,
                angularVelocity = _car.Rb.angularVelocity
            });

            _frameIndex++;
            return;
        }

        // Rewind
        if (_rewindIndex < 0 || _rewindIndex >= _recordedStates.Count)
        {
            StopReplay();
            return;
        }

        var state = _recordedStates[_rewindIndex];
        _car.Rb.position = state.position;
        _car.Rb.rotation = state.rotation;
        _car.Rb.linearVelocity = state.linearVelocity;
        _car.Rb.angularVelocity = state.angularVelocity;

        _rewindIndex--;
    }

    #endregion


    #region Main API

    public void LoadRoute(List<InputFrame> data)
    {
        _data = new List<InputFrame>(data);
        _frameIndex = 0;
        _rewindIndex = -1;
        _recordedStates.Clear();

        _mode = ReplayMode.Stopped;
        _car.SetReplayDriven(false);
    }

    public void StartReplay()
    {
        if (_data == null || _data.Count == 0) return;

        _frameIndex = 0;
        _rewindIndex = -1;
        _recordedStates.Clear();

        _mode = ReplayMode.Forward;
        _car.SetReplayDriven(true);
    }

    public void StartRewind()
    {
        if (_recordedStates.Count == 0) return;

        _rewindIndex = _recordedStates.Count - 1;
        _rewindLerpT = 0f;
        _mode = ReplayMode.Rewind;
        _car.SetReplayDriven(true);
    }

    public void StopReplay()
    {
        _mode = ReplayMode.Stopped;
        _car.SetReplayDriven(false);
    }

    public bool IsRewinding => _mode == ReplayMode.Rewind;

    #endregion


    #region Private & Protected

    [SerializeField] private float _rewindSmoothSpeed = 12f;
    private float _rewindLerpT = 0f;

    private enum ReplayMode
    {
        Stopped,
        Forward,
        Rewind
    }

    [System.Serializable]
    private struct RewindFrame
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 linearVelocity;
        public Vector3 angularVelocity;
    }

    private CarMovement _car;
    private List<InputFrame> _data;
    private int _frameIndex = 0;
    private ReplayMode _mode = ReplayMode.Stopped;

    private readonly List<RewindFrame> _recordedStates = new();
    private int _rewindIndex = -1;

    #endregion
}