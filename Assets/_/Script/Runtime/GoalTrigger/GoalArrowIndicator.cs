using UnityEngine;

public class GoalArrowIndicator : MonoBehaviour
{
    #region Unity API

    private void LateUpdate()
    {
        TryAcquirePlayer();
        if (_player == null || _gameManager == null || _goalsByIndex == null || _goalsByIndex.Length == 0)
            return;
        int idx = _gameManager.CurrentCarIndex;
        if (idx < 0 || idx >= _goalsByIndex.Length || _goalsByIndex[idx] == null)
            return;
        Vector3 playerPos = _player.position;
        Vector3 goalPos = _goalsByIndex[idx].transform.position;
        
        transform.position = playerPos + Vector3.up * _height;
        
        Vector3 dir = goalPos - playerPos;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        
        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90f, yaw, _spriteAngleOffset);
        
    }

    #endregion


    #region Main API

    private void TryAcquirePlayer()
    {
        if (_player != null) return;
        if (Time.time < _nextSearchTime) return;
        _nextSearchTime = Time.time + _reacquireInterval;
        var cars = Object.FindObjectsByType<CarMovement>(FindObjectsSortMode.None);
        foreach (var c in cars)
        {
            if (c != null && c.GetComponent<ReplayRecorder>() != null)
            {
                _player = c.transform;
                break;
            }
        }
    }

    public void InvalidatePlayer() => _player = null;

    #endregion


    #region Private & Protected

    [Space(2),Header("Refs")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GoalTrigger[] _goalsByIndex;

    [Space(2), Header("Follow")]
    [SerializeField] private float _height = 2.5f;
    [SerializeField] private float _reacquireInterval = 0.2f;

    [Space(2), Header("Sprite Rotation")]
    [SerializeField] private float _spriteAngleOffset = 0f;

    private Transform _player;
    private float _nextSearchTime;

    #endregion
}
