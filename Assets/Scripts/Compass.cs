﻿using UnityEngine;

/// <summary>
/// A compass that always points toward a set location.
/// </summary>
public class Compass : Item
{
    /// <summary>
    /// The location toward which the compass always points.
    /// </summary>
    public Vector3 pointLocation;

    /// <summary>
    /// The angular velocity magnitude (in degrees/second) at which
    /// the compass needle rotates toward the target location.
    /// </summary>
    public float rotationVelocity;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (HeldState != ItemState.HeldHidden)
        {
            UpdateRotation();
        }
    }

    /// <summary>
    /// Rotates the compass needle to aim at a set location.
    /// </summary>
    private void UpdateRotation()
    {
        var direction = pointLocation - transform.position;

        // Only modify y-rotation
        var rotation = Quaternion.Euler(transform.rotation.x, Quaternion.LookRotation(direction).eulerAngles.y, transform.rotation.z);

        if (rotation != transform.rotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationVelocity * Time.deltaTime);
        }
    }
}
