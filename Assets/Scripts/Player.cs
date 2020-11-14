using UnityEngine;

/// <summary>
/// A player that can move and pick up or use items using user input.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// The maximum velocity (in units/frame) magnitude at which
    /// the player moves when any movement keys are held.
    /// </summary>
    public float maxMoveVelocity;

    /// <summary>
    /// The acceleration (in units/frame/frame) magnitude applied
    /// to the player when any movement keys are held.
    /// </summary>
    public float moveAcceleration;

    /// <summary>
    /// The deceleration (in units/frame/frame) applied
    /// to the player when no movement keys are held.
    /// </summary>
    public float moveDeceleration;

    /// <summary>
    /// Gets the current velocity of the player.
    /// </summary>
    public Vector3 Velocity { get; private set; }

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

        UpdateMove();
    }

    /// <summary>
    /// Moves the player by applying an acceleration if any movement keys
    /// are held and a deceleration when no movement keys are held.
    /// </summary>
    private void UpdateMove()
    {
        // Shorthand for keys that are held
        bool right = VirtualKey.Right.IsHeld();
        bool left = VirtualKey.Left.IsHeld();
        bool up = VirtualKey.Up.IsHeld();
        bool down = VirtualKey.Down.IsHeld();

        // Initialize acceleration and deceleration due to friction
        var acceleration = Vector3.zero;
        (bool x, bool z) decelerate = (right == left, up == down);

        if (decelerate.x == false)
        {
            // If right/left held, accelerate right/left
            acceleration.x = right ? 1f : -1f;
        }

        if (decelerate.z == false)
        {
            // If up/down held, accelerate up/down
            acceleration.z = up ? 1f : -1f;
        }

        // Scale acceleration to constant acceleration magnitude
        acceleration.Normalize();
        acceleration *= moveAcceleration;

        // Calculate new velocity based on previous velocity
        var velocity = Velocity + acceleration;

        if (velocity.magnitude > maxMoveVelocity)
        {
            // Scale new velocity to max velocity magnitude
            velocity = velocity.normalized * maxMoveVelocity;
        }

        if (decelerate.x || decelerate.z)
        {
            // If right/left or up/down not held, decelerate horizontally or vertically
            var target = new Vector3(
                    decelerate.x ? 0f : velocity.x,
                    velocity.y,
                    decelerate.z ? 0f : velocity.z);
            velocity = Vector3.MoveTowards(velocity, target, moveDeceleration);
        }

        // Update velocity and move
        Velocity = velocity;
        GetComponent<Rigidbody>()?.MovePosition(transform.position + Velocity * Time.deltaTime);
    }
}
