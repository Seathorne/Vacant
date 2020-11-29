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
        OpenPosition = Vector2.zero;
        ClosedPosition = new Vector2(RectTransform.rect.width, 0f);

        SetIsOpen(false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {

    }
}
