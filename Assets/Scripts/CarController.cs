using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem;

/// <summary>
/// Handles arcade-style car movement.
/// Used by both the player (via EnablePlayerControl) and ReplayPlayer (via ApplyMovement).
/// Reads input from the New Input System (Player/Move action).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Driving Settings")]
    public float speed = 10f;
    public float rotationSpeed = 120f;

    private Rigidbody rb;
    private bool isPlayerControlled = false;

    private InputSystem_Actions _actions;

    // Cached input value read each FixedUpdate (Vector2: x = turn, y = move)
    public Vector2 MoveInput { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Prevents the car from tipping over or floating
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;

        _actions = new InputSystem_Actions();
    }

    void OnEnable()  => _actions.Enable();
    void OnDisable() => _actions.Disable();

    void FixedUpdate()
    {
        if (!isPlayerControlled) return;

        MoveInput = _actions.Player.Move.ReadValue<Vector2>();
        ApplyMovement(MoveInput.y, MoveInput.x);
    }

    /// <summary>
    /// Applies movement — called by both the player and ReplayPlayer.
    /// </summary>
    public void ApplyMovement(float move, float turn)
    {
        rb.MovePosition(rb.position + transform.forward * move * speed * Time.fixedDeltaTime);

        Quaternion turnRotation = Quaternion.Euler(0f, turn * rotationSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    public void EnablePlayerControl()  => isPlayerControlled = true;
    public void DisablePlayerControl() => isPlayerControlled = false;
}
