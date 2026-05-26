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

        GameObject instance = Instantiate(
            _playerCarPrefab,
            transform.position,
            transform.rotation
        );

        PlayerCar = instance.GetComponent<CarMovement>();
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

    #region Private & Protected

    [SerializeField] private GameObject _playerCarPrefab;
    [SerializeField] private CameraFlollow _cameraFollow;
    #endregion
}
