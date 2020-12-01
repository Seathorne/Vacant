using UnityEngine;

/// <summary>
/// Whether an item is held and out, held and hidden, or not held.
/// Authors: Scott Clarke and Daniel Darnell.
/// </summary>
public enum ItemState
{
    /// <summary>
    /// The item is not held.
    /// </summary>
    NotHeld,

    /// <summary>
    /// The item is held and hidden.
    /// </summary>
    HeldHidden,

    /// <summary>
    /// The item is held and out.
    /// </summary>
    HeldOut
}

/// <summary>
/// Represents an item that the player can hold.
/// Authors: Scott Clarke and Daniel Darnell.
/// </summary>
public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// Whether this item is currently held and out, held and hidden, or not held.
    /// </summary>
    public ItemState HeldState { get; private set; }

    /// <summary>
    /// Sets this item's to be held and out, held and hidden, or not held.
    /// </summary>
    /// <param name="value">The new held state.</param>
    public void SetHeldState(ItemState value)
    {
        HeldState = value;

        // If hiding item, make invisible
        gameObject.SetActive(value != ItemState.HeldHidden);
    }
}
