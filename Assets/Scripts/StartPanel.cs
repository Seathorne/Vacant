using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/// <summary>
/// A panel that opens at the beginning of the game and then closes.
/// </summary>
public class StartPanel : MenuPanel
{
    /// <summary>
    /// The duration (in seconds) before the title screen fades out.
    /// </summary>
    public float holdTime;

    /// <summary>
    /// The duration (in seconds) for the remainder of the UI to fade in.
    /// </summary>
    public float fadeInTime;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();

        GameManager.Pause();
    }

    /// <summary>
    /// Fades out the title screen and fades in other UI elements.
    /// </summary>
    /// <returns>Yield returns until the title screen is faded out.</returns>
    public IEnumerator FadeOut()
    {
        yield return new WaitForSecondsRealtime(holdTime);

        // Fade this title screen out
        yield return Fade(CanvasGroup, 1f, 0f, openCloseTime);
        gameObject.SetActive(false);

        // Start the game
        GameManager.Pause(setPaused: false);
        FindObjectOfType<Timer>().RestartTimer();

        // All UI elements other than start/end panel
        var ui = from Transform child in GetComponentInParent<Canvas>().transform
                 where child.GetComponent<StartPanel>() is null
                    && child.GetComponent<EndPanel>() is null
                 select child.gameObject;

        // Ensure parent UI elements are active
        ui.ForEach(x => x.SetActive(true));

        // Fade in all panels from opacity 0 to 1
        var coroutines = from child in ui
                         let canvasGroup = child.GetComponent<CanvasGroup>()
                         where canvasGroup is CanvasGroup
                         select new Func<IEnumerator>(() => Fade(canvasGroup, 0f, 1f, fadeInTime));

        // Fade panels in, in parallel
        yield return Utility.Parallel(FindObjectOfType<GameManager>(), coroutines);
    }
}
