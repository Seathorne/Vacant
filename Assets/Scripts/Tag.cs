/// <summary>
/// Provides cleaner access to Unity tags to avoid hard-coding strings.
/// Authors: Scott Clarke and Daniel Darnell.
/// </summary>
public static class Tag
{
    /// <summary>
    /// The tag applied to walls of the maze.
    /// </summary>
    public const string Wall = "Wall";

    /// <summary>
    /// The tag applied to <see cref="Ghost"/> objects in the maze.
    /// </summary>
    public const string Ghost = "Ghost";

    /// <summary>
    /// The tag applied to ceiling lights in the maze.
    /// </summary>
    public const string Light = "Light";

    /// <summary>
    /// The tag applied to the exit of the maze.
    /// </summary>
    public const string Exit = "Exit";

    /// <summary>
    /// The tag applied to the <see cref="Compass"/> item
    /// that can be held by the player.
    /// </summary>
    public const string Compass = "Compass";
}
