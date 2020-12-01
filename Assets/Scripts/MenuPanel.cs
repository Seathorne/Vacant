using System.Collections;

using UnityEngine;

/// <summary>
/// A menu that can open or close.
/// Authors: Scott Clarke and Daniel Darnell.
/// </summary>
public abstract class MenuPanel : MonoBehaviour
{
    /// <summary>
    /// The layer on which all UI elements reside.
    /// </summary>
    public const int UILayer = 5;

    /// <summary>
    /// The time (in seconds) it takes to open or close the menu.
    /// </summary>
    public float openCloseTime;

    /// <summary>
    /// The opening or closing coroutine that is currently running.
    /// </summary>
    private Coroutine currentCoroutine;

    /// <summary>
    /// Whether the menu is currently open or closed.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// This game object's <see cref="UnityEngine.CanvasGroup"/> component.
    /// </summary>
    public CanvasGroup CanvasGroup { get; protected set; }

    /// <summary>
    /// This game object's <see cref="UnityEngine.RectTransform"/> component.
    /// </summary>
    public RectTransform RectTransform { get; protected set; }

    /// <summary>
    /// The target position when the menu is open.
    /// </summary>
    public Vector2 OpenPosition { get; protected set; }

    /// <summary>
    /// The target position when the menu is closed.
    /// </summary>
    public Vector2 ClosedPosition { get; protected set; }

    /// <summary>
    /// If the menu is open, begins closing it. If it is closed, begins opening it.
    /// </summary>
    public void ToggleIsOpen()
    {
        Open(!IsOpen);
    }

    /// <summary>
    /// Opens or closes the menu instantly.
    /// </summary>
    /// <param name="setOpen">Whether to open or close the menu.</param>
    public void OpenInstantly(bool setOpen = true)
    {
        IsOpen = setOpen;
        RectTransform.anchoredPosition = setOpen ? OpenPosition : ClosedPosition;
    }

    /// <summary>
    /// Begins opening or closing the menu.
    /// </summary>
    /// <param name="setOpen">Whether to open or close the menu.</param>
    public void Open(bool setOpen = true)
    {
        IsOpen = setOpen;

        // Pause immediately if opening menu
        if (setOpen)
        {
            GameManager.Pause();
        }

        if (currentCoroutine is Coroutine)
        {
            // If already opening/closing, stop that routine
            StopCoroutine(currentCoroutine);
        }

        // Start opening/closing
        float speed = Vector2.Distance(OpenPosition, ClosedPosition) / openCloseTime;
        currentCoroutine = StartCoroutine(MoveTowards(setOpen ? OpenPosition : ClosedPosition, speed));
    }

    /// <summary>
    /// Moves the menu towards the target position at a set speed.
    /// </summary>
    /// <param name="target">The target position towards which the menu will move.</param>
    /// <param name="speed">The speed (in units/second) at which the menu will move.</param>
    /// <returns>Yield returns <see langword="null"/> until the movement is complete.</returns>
    public IEnumerator MoveTowards(Vector2 target, float speed)
    {
        while (Vector2.Distance(RectTransform.anchoredPosition, target) >= 1f)
        {
            // Move towards target
            RectTransform.anchoredPosition = Vector2.MoveTowards(RectTransform.anchoredPosition, target, speed * Time.unscaledDeltaTime);
            yield return null;
        }

        // Ensure target position is reached exactly
        RectTransform.anchoredPosition = target;

        // Pause/unpause if opening/closing menu
        GameManager.Pause(setPaused: IsOpen);
    }

    /// <summary>
    /// Fades the provided <see cref="CanvasGroup"/> object from the start to the end opacity
    /// over the specified duration.
    /// </summary>
    /// <param name="canvasGroup">The <see cref="CanvasGroup"/> object to fade.</param>
    /// <param name="start">The starting opacity.</param>
    /// <param name="end">The ending opacity.</param>
    /// <param name="duration">The duration (in seconds) over which the fade should occur.</param>
    /// <returns>Yield returns <see langword="null"/> until the fade is complete.</returns>
    public static IEnumerator Fade(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        canvasGroup.alpha = start;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = end;
    }
}
