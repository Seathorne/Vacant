using System;
using System.Collections;
using System.Linq;

using UnityEngine;

/// <summary>
/// A panel that opens at the beginning of the game and then closes.
/// </summary>
public class EndPanel : MenuPanel
{
    /// <summary>
    /// The duration (in seconds) before the title screen fades out.
    /// </summary>
    public float holdTime;

    /// <summary>
    /// The duration (in seconds) for the remainder of the UI to fade out.
    /// </summary>
    public float fadeOutTime;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// Fades in the end screen.
    /// </summary>
    /// <returns>Yield returns until the end screen is faded in.</returns>
    public IEnumerator FadeIn()
    {
        GameManager.Pause();

        // All UI elements other than this one
        var ui = from Transform child in GetComponentInParent<Canvas>().transform
                 where child.GetComponent<EndPanel>() is null
                 select child.gameObject;

        // Fade out all panels from opacity 1 to 0
        var coroutines = from child in ui
                         let canvasGroup = child.GetComponent<CanvasGroup>()
                         where canvasGroup is CanvasGroup
                         select new Func<IEnumerator>(() => Fade(canvasGroup, 1f, 0f, fadeOutTime));
        
        // Fade panels out, in parallel
        yield return Utility.Parallel(FindObjectOfType<GameManager>(), coroutines);

        // Deactivate any remaining UI elements other than this one
        ui.ForEach(x => x.SetActive(false));

        // Fade this end screen in
        gameObject.SetActive(true);
        Utility.Print(CanvasGroup);
        yield return Fade(CanvasGroup, 0f, 1f, openCloseTime);
    }
}
