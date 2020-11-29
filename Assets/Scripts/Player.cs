using System.Collections.Generic;
using System.Linq;

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
    /// The maximum range (in units) within which the player can pick up items.
    /// </summary>
    public float itemPickupRange;

    /// <summary>
    /// The items that the player currently holds.
    /// </summary>
    private readonly IList<Item> heldItems = new List<Item>();

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
    /// The held item that is currently out.
    /// </summary>
    public Item HeldItem { get; private set; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        foreach (var item in GetComponentsInChildren<Item>())
        {
            // Pick up all child objects
            PickUpItem(item);

            if (item is Flashlight)
            {
                // Set flashlight active
                BringOutItem(item);
            }
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        UpdateRotation();
        UpdateMoveRelative();

        if (VirtualKey.SwapItem.JustPressed())
        {
            CycleActiveItem();
        }

        if (VirtualKey.PickUpItem.JustPressed())
        {
            TryPickUpItem();
        }

        if (VirtualKey.DropItem.JustPressed())
        {
            DropItem(HeldItem);
        }
    }

    /// <summary>
    /// Makes the player begin holding the specified item.
    /// </summary>
    /// <param name="item">The item for the player to pick up.</param>
    public void PickUpItem(Item item)
    {
        if (item is null)
        {
            return;
        }

        if (heldItems.Contains(item) == false)
        {
            // If not already holding this item, pick it up
            heldItems.Add(item);

            // Add to children
            item.transform.parent = transform;

            // Hide item
            item.SetHeldState(ItemState.HeldHidden);
        }
    }

    /// <summary>
    /// Makes the player drop the held item that is currently active.
    /// </summary>
    /// <param name="item">The item for the player to drop.</param>
    public void DropItem(Item item)
    {
        if (item is null)
        {
            return;
        }

        // If holding this item, drop it
        heldItems.Remove(item);

        // Remove from children
        item.transform.parent = null;

        // Reveal dropped item
        item.SetHeldState(ItemState.NotHeld);

        // Show next item
        HeldItem = heldItems.LastOrDefault();
        HeldItem?.SetHeldState(ItemState.HeldOut);
    }

    /// <summary>
    /// Makes the player switch which held item is currently active.
    /// Having no item active is a valid result.
    /// </summary>
    public void CycleActiveItem()
    {
        int nextIndex = heldItems.IndexOf(HeldItem) + 1;
        if (nextIndex >= heldItems.Count)
        {
            // If holding last item, make none active
            HeldItem?.SetHeldState(ItemState.HeldHidden);
            HeldItem = null;
        }
        else
        {
            // Otherwise, cycle to next item
            BringOutItem(heldItems[nextIndex]);
        }
    }

    /// <summary>
    /// Makes the player hide the held item that was previously active and
    /// activate the specified held item.
    /// </summary>
    /// <param name="item">The item to activate.</param>
    private void BringOutItem(Item item)
    {
        if (item is null)
        {
            return;
        }

        // Hide previous item
        HeldItem?.SetHeldState(ItemState.HeldHidden);

        // Show current item
        HeldItem = item;
        HeldItem.SetHeldState(ItemState.HeldOut);
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

    /// <summary>
    /// If any non-held item is within range, picks it up.
    /// </summary>
    /// <returns>The item that was picked up, if one was; <see langword="null"/> otherwise.</returns>
    private Item TryPickUpItem()
    {
        // Select all non-held items within range
        var validItems = from potentialItem in FindObjectsOfType<Item>()
                         where Vector3.Distance(potentialItem.transform.position, transform.position) <= itemPickupRange
                            && heldItems.Contains(potentialItem) == false
                         select potentialItem;

        if (validItems.FirstOrDefault() is Item item)
        {
            // If there is a potential item to pick up, pick it up
            PickUpItem(item);
            BringOutItem(item);
            return item;
        }

        return null;
    }
}
