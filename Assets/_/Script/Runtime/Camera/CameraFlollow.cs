using UnityEngine;

public class CameraFlollow : MonoBehaviour
{
    #region Public

    public Transform _target;
    public Vector3 _offset = new Vector3(0f, 7f, -10f);
    public float _smooth = 8f;
    public bool _rotateWithCar = false; // false = vue stable, true = suit le yaw

    #endregion


    #region Unity API

    void LateUpdate()
    {
        if (_target == null) return;
        Vector3 desired = _target.position + _offset;
        transform.position = Vector3.Lerp(transform.position, desired, _smooth * Time.deltaTime);
        if (_rotateWithCar)
            transform.rotation = Quaternion.Euler(90f, _target.eulerAngles.y, 0f);
        else
            transform.rotation = Quaternion.Euler(90f, 0f, 0f); 
    }

    #endregion
    // 
    //
    #region Main API

    public void SetTarget(Transform target) => _target = target;

    #endregion
    //
    // 
    #region Private & Protected

    #endregion
}