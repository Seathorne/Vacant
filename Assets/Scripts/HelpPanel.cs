using UnityEngine;

/// <summary>
/// A menu that can open or close.
/// </summary>
public class HelpPanel : MenuPanel
{
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();

        OpenPosition = Vector2.zero;
        ClosedPosition = new Vector2(RectTransform.rect.width, 0f);

        OpenInstantly(setOpen: false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {

    }
}
