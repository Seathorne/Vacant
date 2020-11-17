using UnityEngine;

/// <summary>
/// A light that aim at the current mouse location.
/// </summary>
public class Flashlight : MonoBehaviour
{
    /// <summary>
    /// The angular velocity magnitude (in degrees/second) at which
    /// the flashlight rotates toward the target location.
    /// </summary>
    public float rotationVelocity;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected void Start()
    {

    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    protected void Update()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        UpdateRotation();
    }

    /// <summary>
    /// Rotates the flashlight to aim at an object at which the mouse is pointed.
    /// </summary>
    private void UpdateRotation()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            // If mouse raycast hits an object, rotate to face collision point

            var direction = hit.point - transform.position;
            var rotation = Quaternion.LookRotation(direction);

            if (rotation != transform.rotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationVelocity * Time.deltaTime);
            }
        }
    }
}
