using UnityEngine;

/// <summary>
/// Represents global game logic and game state.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Whether the game is currently paused.
    /// </summary>
    public static bool IsPaused { get; private set; } = false;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected void Start()
    {
        Pause(setPaused: false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    protected void Update()
    {
        if (IsPaused == false && VirtualKey.Pause.JustPressed())
        {
            Pause();
        }
        else if (IsPaused && VirtualKey.Unpause.JustPressed())
        {
            Pause(setPaused: false);
        }
    }

    /// <summary>
    /// Pauses or unpauses the game.
    /// </summary>
    /// <param name="setPaused"><see langword="true"/> to pause the game; <see langword="false"/> to unpause.</param>
    public void Pause(bool setPaused = true)
    {
        Time.timeScale = setPaused ? 0f : 1f;
        IsPaused = setPaused;
    }
}
