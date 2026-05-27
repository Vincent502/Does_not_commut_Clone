using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(CarMovement))]
public class GosthReplay : MonoBehaviour
{
    #region Public

    #endregion
    //
    //
    #region Unity API

    private void Awake()
    {
        _car = GetComponent<CarMovement>();
    }
    void FixedUpdate()
    {
        if (!_isReplaying || _data == null) return;
        if (_frameIndex >= _data.Count)
        {
            _isReplaying = false;
            _car.SetReplayDriven(false);
            return;
        }
        var frame = _data[_frameIndex];
        _car.ApplyReplayFrame(frame.horizontal);
        _frameIndex++;
    }

    #endregion
    // 
    //
    #region Main API

    public void LoadRoute(List<InputFrame> data)
    {
        _data = new List<InputFrame>(data);
        _frameIndex = 0;
        _isReplaying = false;
        _car.SetReplayDriven(false);
    }

    public void StartReplay()
    {
        if (_data == null || _data.Count == 0) return;
        _frameIndex = 0;
        _isReplaying = true;
        _car.SetReplayDriven(true);
    }

    #endregion
    //
    // 
    #region Private & Protected

    private bool _isReplaying;
    private CarMovement _car;
    private List<InputFrame> _data;
    private int _frameIndex = 0;
    #endregion
}
