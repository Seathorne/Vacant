using UnityEngine;

/// <summary>
/// A light that aim at the current mouse location.
/// Authors: Scott Clarke and Daniel Darnell.
/// </summary>
public class Flashlight : Item
{
    /// <summary>
    /// The angular velocity magnitude (in degrees/second) at which
    /// the flashlight rotates toward the target location.
    /// </summary>
    public float rotationVelocity;

    /// <summary>
    /// The raycast hit from the flashlight beam last frame,
    /// or <see langword="null"/> if nothing was hit.
    /// </summary>
    public RaycastHit RecentHit { get; private set; }

    /// <summary>
    /// The ghost hit by the flashlight beam last frame,
    /// or <see langword="null"/> if no ghost was hit.
    /// </summary>
    public Ghost RecentGhostHit { get; private set; }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        if (HeldState == ItemState.HeldOut)
        {
            if (UpdateRaycast())
            {
                UpdateRotation();
            }

            UpdateHitGhost();
        }
    }

    /// <summary>
    /// Casts a ray from the camera to the mouse location and
    /// determines whether any game object was hit.
    /// </summary>
    /// <returns>Whether any game object was hit</returns>
    private bool UpdateRaycast()
    {
        bool hitAnything = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);
        RecentHit = hit;

        return hitAnything;
    }

    /// <summary>
    /// Rotates the flashlight to aim at an object at which the mouse is pointed.
    /// </summary>
    private void UpdateRotation()
    {
        // If mouse raycast hits an object, rotate to face collision point

        var direction = RecentHit.point - transform.position;
        var rotation = Quaternion.LookRotation(direction);

        if (rotation != transform.rotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationVelocity * Time.deltaTime);
        }
    }

    /// <summary>
    /// Updates whether a ghost has been hit with the flashlight beam.
    /// </summary>
    private void UpdateHitGhost()
    {
        var prevGhost = RecentGhostHit;
        RecentGhostHit = RecentHit.collider?.GetComponent<Ghost>();

        if (prevGhost is Ghost && RecentGhostHit != prevGhost)
        {
            // If stopped shining on previous ghost
            prevGhost.FlashlightHit(setHit: false);
        }

        // If newly hit object is ghost, shine on it
        RecentGhostHit?.FlashlightHit();
    }
}
