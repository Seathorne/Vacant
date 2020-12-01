using UnityEngine;

/// <summary>
/// Authors: Scott Clarke and Daniel Darnell.
/// </summary>
public class Lightbulb : MonoBehaviour
{
    private GameObject playerObject;
    private Light lightComponent;
    
    // Start is called before the first frame update
    void Start()
    {
        // Retrieve player object and light component
        playerObject = GameObject.Find("Player");
        lightComponent = transform.GetComponent<Light>();
    }
    
    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(playerObject.transform.position, transform.position);
        float cullingDistance = 8.0f * Generator.wallScale;
        
        // If lightbulb is within culling distance of player
        if (distanceToPlayer < cullingDistance)
        {
            lightComponent.enabled = true;    
        }
        else if (distanceToPlayer > cullingDistance)
        {
            lightComponent.enabled = false;
        }
    }
}
