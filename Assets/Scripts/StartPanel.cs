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

        OpenPosition = Vector2.zero;
        ClosedPosition = new Vector2(0f, RectTransform.rect.height);
        
        StartCoroutine(FadeOutTitle());
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// Fades out the title screen.
    /// </summary>
    /// <returns>Yield returns until the title screen is faded out.</returns>
    private IEnumerator FadeOutTitle()
    {
        // Pause game while this title screen fades out
        GameManager.Pause();

        // Deactivate all parent UI elements other than this one
        var ui = from Transform child in GetComponentInParent<Canvas>().transform
                 where child != transform
                 select child.gameObject;
        ui.ForEach(x => x.SetActive(false));

        yield return new WaitForSecondsRealtime(holdTime);

        // Fade this title screen out
        yield return Fade(CanvasGroup, 1f, 0f, openCloseTime);
        GameManager.Pause(setPaused: false);

        yield return new WaitForSecondsRealtime(fadeInTime);

        // Reactivate all parent UI elements
        ui.ForEach(x => x.SetActive(true));

        // Fade in all panels from opacity 0 to 1
        var coroutines = from child in ui
                         let canvasGroup = child.GetComponent<CanvasGroup>()
                         where canvasGroup is CanvasGroup
                         select new Func<IEnumerator>(() => Fade(canvasGroup, 0f, 1f, 2f));
        yield return Utility.Parallel(this, coroutines);
    }
}
