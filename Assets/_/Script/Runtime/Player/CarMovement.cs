using InputSystem;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class CarMovement : MonoBehaviour
{
    #region Public

    [Space(2), Header("Driving Settings")]

    public float _acceleration = 18f;
    public float _maxSpeed = 12f;
    public float _rotationSpeed = 130f;

    public Vector2 _moveInput { get; private set; }

    public static event Action OnPlayerSelectedStart;

    [HideInInspector] public bool _isRacing = false;

    #endregion

    #region Unity API

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
        m_rb.linearDamping = 0f;
        m_rb.angularDamping = 10f;

        m_actions = new InputSystem_Actions();
        m_actions.Enable();
    }

    private void Update()
    {
        if (_isRacing) return;

        if (m_actions.Player.Jump.WasPressedThisFrame())
            OnPlayerSelectedStart?.Invoke();
    }

    private void FixedUpdate()
    {
        if (!_isRacing) return;

        _moveInput = m_actions.Player.Move.ReadValue<Vector2>();

        ApplyTurn(_moveInput.x);
        ApplyMovement(_moveInput.y);
    }

    private void OnDestroy()
    {
        m_actions.Disable();
    }

    #endregion

    #region Main API

    private void ApplyMovement(float move)
    {
        if (Mathf.Approximately(move, 0f)) return;

        m_rb.AddForce(transform.forward * move * _acceleration, ForceMode.Acceleration);

        if (m_rb.linearVelocity.magnitude > _maxSpeed)
            m_rb.linearVelocity = m_rb.linearVelocity.normalized * _maxSpeed;
    }

    private void ApplyTurn(float turn)
    {
        float currentSpeed = m_rb.linearVelocity.magnitude;
        if (currentSpeed > 0.3f)
        {
            float direction = Vector3.Dot(m_rb.linearVelocity, transform.forward) >= 0 ? 1f : -1f;

            float turnAmount = turn * _rotationSpeed * direction * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);

            m_rb.MoveRotation(m_rb.rotation * turnRotation);
        }
    }

    #endregion

    #region Private & Protected

    private Rigidbody m_rb;
    private InputSystem_Actions m_actions;

    #endregion
}
