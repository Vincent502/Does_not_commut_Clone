using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem;

/// <summary>
/// Handles arcade-style car movement with drift and deceleration feel.
/// Used by both the player (via EnablePlayerControl) and ReplayPlayer (via ApplyMovement).
/// Reads input from the New Input System (Player/Move action).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Driving Settings")]
    public float acceleration    = 18f;   // Forward push force
    public float maxSpeed        = 12f;   // Top speed cap
    public float rotationSpeed   = 130f;  // Degrees per second

    [Header("Feel")]
    [Range(0f, 1f)]
    [Tooltip("How much lateral (sideways) velocity is kept. 0 = no drift, 1 = full ice.")]
    public float driftFactor     = 0.92f;

    [Tooltip("Speed multiplier applied each frame when no input is pressed (0-1).")]
    public float decelerationDrag = 0.94f;

    private Rigidbody rb;
    private bool isPlayerControlled = false;
    private InputSystem_Actions _actions;

    // Cached input value read each FixedUpdate (Vector2: x = turn, y = move)
    public Vector2 MoveInput { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY
                       | RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;
        rb.linearDamping  = 0f;  // We handle drag manually for better feel
        rb.angularDamping = 10f; // Snap rotation instantly

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
        // ── 1. Forward / backward acceleration ───────────────────────────────
        rb.AddForce(transform.forward * move * acceleration, ForceMode.Acceleration);

        // ── 2. Speed cap ──────────────────────────────────────────────────────
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        // ── 3. Drift — cancel only part of the lateral (sideways) velocity ───
        //    driftFactor close to 1 = more sliding, close to 0 = grippy
        float lateralSpeed = Vector3.Dot(rb.linearVelocity, transform.right);
        rb.linearVelocity -= transform.right * lateralSpeed * (1f - driftFactor);

        // ── 4. Natural deceleration when no throttle input ───────────────────
        if (Mathf.Approximately(move, 0f))
            rb.linearVelocity *= decelerationDrag;

        // ── 5. Rotation — only meaningful when the car is moving ─────────────
        float currentSpeed = rb.linearVelocity.magnitude;
        if (currentSpeed > 0.3f)
        {
            // Invert steering when reversing
            float direction = Vector3.Dot(rb.linearVelocity, transform.forward) >= 0 ? 1f : -1f;
            Quaternion turnRotation = Quaternion.Euler(
                0f, turn * rotationSpeed * direction * Time.fixedDeltaTime, 0f
            );
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    public void EnablePlayerControl()  => isPlayerControlled = true;
    public void DisablePlayerControl() => isPlayerControlled = false;
}