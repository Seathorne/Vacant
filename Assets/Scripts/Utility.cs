using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Provides static utility, helper, or extension methods.
/// </summary>
public static class Utility
{
    /// <summary>
    /// A mapping of physical keys from their associated virtual key.
    /// </summary>
    public static readonly Dictionary<VirtualKey, (KeyCode key, KeyCode? alt)> KeyMap
        = new Dictionary<VirtualKey, (KeyCode key, KeyCode? alt)>
        {
            { VirtualKey.Right, (key: KeyCode.D, alt: KeyCode.RightArrow) },
            { VirtualKey.Left, (key: KeyCode.A, alt: KeyCode.LeftArrow) },
            { VirtualKey.Up, (key: KeyCode.W, alt: KeyCode.UpArrow) },
            { VirtualKey.Down, (key: KeyCode.S, alt: KeyCode.DownArrow) },
            { VirtualKey.Pause, (key: KeyCode.Escape, alt: KeyCode.P) },
            { VirtualKey.Unpause, (key: KeyCode.Escape, alt: KeyCode.P) },
            { VirtualKey.SwitchItem, (key: KeyCode.Q, alt: null) },
        };

    /// <summary>
    /// Returns whether a physical key mapped to the specified virtual key is currently held.
    /// </summary>
    /// <param name="vKey">The virtual key whose physical keys to check.</param>
    /// <returns><see langword="true"/>, if the virtual key is held; <see langword="false"/> otherwise.</returns>
    public static bool Check(this VirtualKey vKey, Func<KeyCode, bool> condition)
    {
        var keyset = KeyMap[vKey];
        return condition(keyset.key)
            || (keyset.alt is KeyCode alt && condition(alt));
    }

    /// <summary>
    /// Returns whether a physical key mapped to the specified virtual key is currently held.
    /// </summary>
    /// <param name="vKey">The virtual key whose physical keys to check.</param>
    /// <returns><see langword="true"/>, if the virtual key is held; <see langword="false"/> otherwise.</returns>
    public static bool IsHeld(this VirtualKey vKey) => vKey.Check(x => Input.GetKey(x));

    /// <summary>
    /// Returns whether a physical key mapped to the specified virtual key was just pressed.
    /// </summary>
    /// <param name="vKey">The virtual key whose physical keys to check.</param>
    /// <returns><see langword="true"/>, if the virtual key was just pressed; <see langword="false"/> otherwise.</returns>
    public static bool JustPressed(this VirtualKey vKey) => vKey.Check(x => Input.GetKeyDown(x));

    /// <summary>
    /// Returns whether a physical key mapped to the specified virtual key was just released.
    /// </summary>
    /// <param name="vKey">The virtual key whose physical keys to check.</param>
    /// <returns><see langword="true"/>, if the virtual key was just released; <see langword="false"/> otherwise.</returns>
    public static bool JustReleased(this VirtualKey vKey) => vKey.Check(x => Input.GetKeyUp(x));

    /// <summary>
    /// Prints the specified objects to the console window.
    /// </summary>
    /// <param name="objects">The objects to print.</param>
    public static void Print(params dynamic[] objects)
    {
        Debug.Log(string.Join(" ", objects));
    }
}
