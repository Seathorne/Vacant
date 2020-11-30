using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents global game logic and game state.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The button that opens the pause/help menu.
    /// </summary>
    public Button helpButton;

    /// <summary>
    /// Whether the game is currently paused.
    /// </summary>
    public static bool IsPaused { get; private set; } = false;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        // If pausing is currently allowed
        if (helpButton.isActiveAndEnabled)
        {
            if (VirtualKey.TogglePause.JustPressed())
            {
                // Relay keyboard -> button press
                helpButton.GetComponent<Graphic>().CrossFadeColor(helpButton.colors.highlightedColor, helpButton.colors.fadeDuration, ignoreTimeScale: true, useAlpha: true);
                helpButton.onClick.Invoke();
            }
            else if (VirtualKey.TogglePause.JustReleased())
            {
                helpButton.GetComponent<Graphic>().CrossFadeColor(helpButton.colors.normalColor, helpButton.colors.fadeDuration, ignoreTimeScale: true, useAlpha: true);
            }
        }
    }

    /// <summary>
    /// Pauses or unpauses the game.
    /// </summary>
    /// <param name="setPaused"><see langword="true"/> to pause the game; <see langword="false"/> to unpause.</param>
    public static void Pause(bool setPaused = true)
    {
        Time.timeScale = setPaused ? 0f : 1f;
        IsPaused = setPaused;
    }
}
