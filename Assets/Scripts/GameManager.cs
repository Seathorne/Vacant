using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents global game logic and game state.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The panel that shows the title screen.
    /// </summary>
    public StartPanel startPanel;

    /// <summary>
    /// The panel that shows the ending screen.
    /// </summary>
    public EndPanel endPanel;

    /// <summary>
    /// The panel that shows the help/information screen.
    /// </summary>
    public HelpPanel helpPanel;

    /// <summary>
    /// The button that opens the pause/help menu.
    /// </summary>
    public Button helpButton;

    /// <summary>
    /// The generator to use for creating the maze.
    /// </summary>
    public Generator generator;

    /// <summary>
    /// The text field that shows the best maze escape time so far.
    /// </summary>
    public Text bestTime;

    /// <summary>
    /// Whether the game is currently paused.
    /// </summary>
    public static bool IsPaused { get; private set; } = false;

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    private void Start()
    {
        startPanel.gameObject.SetActive(true);
        helpPanel.gameObject.SetActive(true);
        helpButton.gameObject.SetActive(true);
        endPanel.gameObject.SetActive(false);
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

    /// <summary>
    /// Fades in the end screen.
    /// </summary>
    public void Win()
    {
        // Stop timer
        var timer = FindObjectOfType<Timer>();
        timer.StopTimer();

        // Show best time
        Timer.BestTime = Mathf.Min(timer.ElapsedTime, Timer.BestTime);
        bestTime.text = $"Best time: {Timer.ToString(Timer.BestTime)}";

        // Show end screen
        StartCoroutine(endPanel.FadeIn());
    }

    /// <summary>
    /// Restarts the game scene.
    /// </summary>
    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// Starts the game with the specified maze size.
    /// </summary>
    /// <param name="mazeSize">The size of the maze to generate.</param>
    private void Start(int mazeSize)
    {
        // Disable buttons
        startPanel.GetComponentsInChildren<Button>().ForEach(x => x.interactable = false);

        // Generate maze
        generator.NewMaze(mazeSize);

        // Begin game
        StartCoroutine(startPanel.FadeOut());
    }

    /// <summary>
    /// Starts the game with an easy-difficulty maze.
    /// </summary>
    public void StartEasy()
    {
        Start(15);
    }

    /// <summary>
    /// Starts the game with a medium-difficulty maze.
    /// </summary>
    public void StartMedium()
    {
        Start(21);
    }

    /// <summary>
    /// Starts the game with a hard-difficulty maze.
    /// </summary>
    public void StartHard()
    {
        Start(31);
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
