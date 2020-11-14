/// <summary>
/// Represents the logical outcome of pressing a key, allowing
/// multiple physical keys to map to the same logical behavior.
/// </summary>
public enum VirtualKey
{
    /// <summary>
    /// A key that will perform any behavior associated with holding right (e.g. moving rightward).
    /// </summary>
    Right,

    /// <summary>
    /// A key that will perform any behavior associated with holding left (e.g. moving leftward).
    /// </summary>
    Left,

    /// <summary>
    /// A key that will perform any behavior associated with holding up (e.g. moving forward).
    /// </summary>
    Up,

    /// <summary>
    /// A key that will perform any behavior associated with holding down (e.g. moving backward).
    /// </summary>
    Down,

    /// <summary>
    /// A key that will pause the game.
    /// </summary>
    Pause,

    /// <summary>
    /// A key that will unpause the game.
    /// </summary>
    Unpause,

    /// <summary>
    /// A key that will allow the player to switch which item is currently held.
    /// </summary>
    SwitchItem
}
