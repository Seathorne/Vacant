using UnityEngine;

/// <summary>
/// Contains constant values that allow game settings to be configured in one location.
/// </summary>
public static class Settings
{
    /// <summary>
    /// The keys that will perform any behavior associated with holding up (e.g. moving forward).
    /// </summary>
    public static readonly KeyCode[] UpKeys =
    {
        KeyCode.UpArrow,
        KeyCode.W
    };

    /// <summary>
    /// The keys that will perform any behavior associated with holding down (e.g. moving backward).
    /// </summary>
    public static readonly KeyCode[] DownKeys =
    {
        KeyCode.DownArrow,
        KeyCode.S
    };

    /// <summary>
    /// The keys that will performr any behavior associated with holding right (e.g. moving right).
    /// </summary>
    public static readonly KeyCode[] RightKeys =
    {
        KeyCode.RightArrow,
        KeyCode.D
    };

    /// <summary>
    /// The keys that will perform any behavior associated with holding left (e.g. moving left).
    /// </summary>
    public static readonly KeyCode[] LeftKeys =
    {
        KeyCode.LeftArrow,
        KeyCode.A
    };

    /// <summary>
    /// The key that will pause or unpause the game.
    /// </summary>
    public const KeyCode PauseKey = KeyCode.Escape;

    /// <summary>
    /// The key that will allow the player to switch which item is currently held.
    /// </summary>
    public const KeyCode SwitchItemKey = KeyCode.Q;
}
