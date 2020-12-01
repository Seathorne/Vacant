using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Keeps track of the time taken for the player to complete a maze.
/// </summary>
public class Timer : MonoBehaviour
{
    /// <summary>
    /// The text displaying the current time.
    /// </summary>
    private Text timerText;

    /// <summary>
    /// The global time (in seconds) when the current maze was started.
    /// </summary>
    private float startTime;

    /// <summary>
    /// The best maze completion time (in seconds) so far.
    /// </summary>
    public static float BestTime { get; set; } = float.MaxValue;

    /// <summary>
    /// The time (in seconds) elapsed in the maze so far.
    /// </summary>
    public float ElapsedTime { get; private set; }

    /// <summary>
    /// Whether the timer is running or stopped.
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Starts or restarts the timer.
    /// </summary>
    public void RestartTimer()
    {
        startTime = Time.time;
        IsRunning = true;
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void StopTimer()
    {
        IsRunning = false;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        timerText = FindObjectOfType<Text>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (GameManager.IsPaused || !IsRunning)
        {
            return;
        }

        ElapsedTime = Time.time - startTime;
        timerText.text = ToString(ElapsedTime);
    }

    /// <summary>
    /// Converts the provided time (in secondss) to a "MM:SS" formatted string.
    /// </summary>
    /// <param name="time">The time (in seconds) to convert to a string.</param>
    /// <returns>A string representing the provided time.</returns>
    public static string ToString(float time) => $"{Mathf.FloorToInt(time / 60f):00}:{Mathf.FloorToInt(time % 60):00}";
}
