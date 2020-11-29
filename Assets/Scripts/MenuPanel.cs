using System.Collections;
using UnityEngine;

/// <summary>
/// A menu that can open or close.
/// </summary>
public abstract class MenuPanel : MonoBehaviour
{
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
        SetIsOpen(!IsOpen);
    }

    /// <summary>
    /// Begins opening or closing the menu.
    /// </summary>
    /// <param name="setOpen">Whether to open or close the menu.</param>
    public void SetIsOpen(bool setOpen)
    {
        IsOpen = setOpen;

        // Pause immediately if opening menu
        if (setOpen)
        {
            FindObjectOfType<GameManager>()?.Pause();
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
    /// <returns>Yield returns <see langword="null"/> to fulfill the requirements of a coroutine.</returns>
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
        FindObjectOfType<GameManager>()?.Pause(setPaused: IsOpen);
    }
}
