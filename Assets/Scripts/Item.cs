using UnityEngine;

/// <summary>
/// Represents an item that the player can hold.
/// </summary>
public abstract class Item : MonoBehaviour
{
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

    }

    /// <summary>
    /// Activates or deactivates the item.
    /// </summary>
    /// <param name="value">Whether to activate or deactivate the item.</param>
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
