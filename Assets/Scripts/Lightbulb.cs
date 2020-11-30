using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightbulb : MonoBehaviour
{
    private GameObject playerObject;
    private Light lightComponent;
    
    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player");
        lightComponent = transform.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(playerObject.transform.position, transform.position);
        float cullingDistance = 10.0f * Generator.wallScale;
 
        if (distanceToPlayer < cullingDistance){
            lightComponent.enabled = true;    
        }
        if (distanceToPlayer > cullingDistance){
            lightComponent.enabled = false;
        }
    }
}
