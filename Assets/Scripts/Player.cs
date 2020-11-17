using UnityEngine;

/// <summary>
/// A player that can move and pick up or use items using user input.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// The maximum velocity magnitude (in units/second) at which
    /// the player moves when any movement keys are held.
    /// </summary>
    public float maxMoveVelocity;

    /// <summary>
    /// The acceleration magnitude (in units/second/second) applied
    /// to the player when any movement keys are held.
    /// </summary>
    public float moveAcceleration;

    /// <summary>
    /// The deceleration magnitude (in units/second/second) applied
    /// to slow down the player when no movement keys are held.
    /// </summary>
    public float moveDeceleration;

    /// <summary>
    /// The maximum angular velocity magnitude (in degrees/second) at which
    /// the player rotates when any rotation keys are held.
    /// </summary>
    public float maxRotationVelocity;

    /// <summary>
    /// The angular acceleration magnitude (in degrees/second/second) applied
    /// to the player when any rotation keys are held.
    /// </summary>
    public float rotationAcceleration;

    /// <summary>
    /// The angular deceleration magnitude (in degrees/second/second) applied
    /// to slow down the player when no rotation keys are held.
    /// </summary>
    public float rotationDeceleration;

    /// <summary>
    /// Gets the current velocity (in units/second) of the player.
    /// </summary>
    public Vector3 AbsoluteVelocity { get; private set; }

    /// <summary>
    /// Gets the current velocity (in units/second) of the player
    /// toward the current facing direction.
    /// </summary>
    /// <see cref="FacingDirection"/>
    public float RelativeVelocity { get; private set; }

    /// <summary>
    /// Gets the current angular velocity (in degrees/second) of the player
    /// around the Y-axis.
    /// </summary>
    public float AngularVelocity { get; private set; }

    /// <summary>
    /// Gets the current facing direction of the player.
    /// </summary>
    public Vector3 FacingDirection { get; private set; } = Vector3.forward;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected void Start()
    {
        
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    protected void Update()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        UpdateRotation();
        UpdateMoveRelative();
    }

    /// <summary>
    /// Rotates the player by applying an angular acceleration when left or right
    /// is held and a deceleration when neither is held.
    /// </summary>
    private void UpdateRotation()
    {
        var rb = GetComponent<Rigidbody>();

        // Shorthand for keys that are held
        bool right = VirtualKey.Right.IsHeld();
        bool left = VirtualKey.Left.IsHeld();

        // Update angular velocity
        if (right ^ left)
        {
            // If right/left held, accelerate rotation
            float direction = right ? 1f : -1f;

            // Calculate new velocity based on previous velocity
            AngularVelocity = Mathf.MoveTowards(
                AngularVelocity,
                maxRotationVelocity * Time.deltaTime * direction,
                rotationAcceleration * Time.deltaTime);
        }
        else
        {
            // If right/left not held, decelerate rotation
            AngularVelocity = Mathf.MoveTowards(
                AngularVelocity,
                0f,
                rotationDeceleration * Time.deltaTime);
        }

        // Rotate and face direction of velocity
        FacingDirection = Quaternion.AngleAxis(AngularVelocity, Vector3.up) * FacingDirection;
        rb?.MoveRotation(Quaternion.LookRotation(FacingDirection, Vector3.up));
    }

    /// <summary>
    /// Moves the player by applying an acceleration towards or away from the
    /// current facing direction when up or down is held and a deceleration
    /// due to friction when neither is held.
    /// </summary>
    private void UpdateMoveRelative()
    {
        var rb = GetComponent<Rigidbody>();

        // Shorthand for keys that are held
        bool up = VirtualKey.Up.IsHeld();
        bool down = VirtualKey.Down.IsHeld();

        // Update relative velocity
        if (up ^ down)
        {
            // If up/down held, accelerate
            float direction = up ? 1f : -1f;

            // Calculate new velocity based on previous velocity
            RelativeVelocity = Mathf.MoveTowards(
                RelativeVelocity,
                maxMoveVelocity * Time.deltaTime * direction,
                moveAcceleration * Time.deltaTime);
        }
        else
        {
            // If up/down not held, decelerate
            RelativeVelocity = Mathf.MoveTowards(
                RelativeVelocity,
                0f,
                moveDeceleration * Time.deltaTime);
        }

        // Update absolute velocity and move toward or away from facing direction
        AbsoluteVelocity = RelativeVelocity * FacingDirection;
        rb?.MovePosition(transform.position + AbsoluteVelocity);
    }

    /// <summary>
    /// Moves the player by applying an absolute acceleration if any movement keys
    /// are held and a deceleration due to friction when no movement keys are held.
    /// </summary>
    private void UpdateMoveAbsolute()
    {
        var rb = GetComponent<Rigidbody>();
        var absoluteVelocity = Vector3.zero;

        // Shorthand for keys that are held
        bool right = VirtualKey.Right.IsHeld();
        bool left = VirtualKey.Left.IsHeld();
        bool up = VirtualKey.Up.IsHeld();
        bool down = VirtualKey.Down.IsHeld();

        // Update vertical velocity component
        if (right ^ left)
        {
            // If right/left held, accelerate horizontally
            float direction = right ? 1f : -1f;

            // Calculate new velocity based on previous velocity
            absoluteVelocity.x = Mathf.MoveTowards(
                AbsoluteVelocity.x,
                maxMoveVelocity * Time.deltaTime * direction,
                moveAcceleration * Time.deltaTime);
        }
        else
        {
            // If right/left not held, decelerate horizontally
            absoluteVelocity.x = Mathf.MoveTowards(
                AbsoluteVelocity.x,
                0f,
                moveDeceleration * Time.deltaTime);
        }

        // Update horizontal velocity component
        if (up ^ down)
        {
            // If up/down held, accelerate vertically
            float direction = up ? 1f : -1f;

            // Calculate new velocity based on previous velocity
            absoluteVelocity.z = Mathf.MoveTowards(
                AbsoluteVelocity.z,
                maxMoveVelocity * Time.deltaTime * direction,
                moveAcceleration * Time.deltaTime);
        }
        else
        {
            // If up/down not held, decelerate vertically
            absoluteVelocity.z = Mathf.MoveTowards(
                AbsoluteVelocity.z,
                0f,
                moveDeceleration * Time.deltaTime);
        }

        // Update absolute velocity and move
        AbsoluteVelocity = absoluteVelocity;
        rb?.MovePosition(transform.position + AbsoluteVelocity);
    }
}
