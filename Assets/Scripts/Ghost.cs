using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Generator MazeGenerator;
    
    /// <summary>
    /// The velocity of the ghost in maze elements per second.
    /// </summary>
    private float velocity = 1.0f;
    
    /// <summary>
    /// The dashing velocity of the ghost in maze elements per second.
    /// </summary>
    private float dashVelocity = 5.0f;
    
    private bool dashing = false;
    
    private bool playerSeen = false;
    
    private bool draggingPlayer = false;
    
    private float adjustedVelocity;
    
    private List<Vector2Int> directions = new List<Vector2Int>();
    
    private Vector2Int currentDirection;
    private Vector3 nextPivot;
    private Vector2Int nextPivotElement;
    
    private GameObject playerObject;
    
    
    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator = (Generator) transform.parent.transform.parent.Find("Generator").gameObject.GetComponent(typeof(Generator));
        
        UpdateAdjustedVelocity();
        
        directions.Add(new Vector2Int(-1,  0));
        directions.Add(new Vector2Int( 1,  0));
        directions.Add(new Vector2Int( 0, -1));
        directions.Add(new Vector2Int( 0,  1));
        
        playerObject = GameObject.Find("Player");
        
        CalculateNextPivot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        Vector3 delta;
        float playerDraggingOffset = Generator.wallScale * 0.3f;
        Vector3 playerDraggingPosition = new Vector3();
        
        delta.x = Mathf.Abs(transform.position.x - nextPivot.x);
        delta.z = Mathf.Abs(transform.position.z - nextPivot.z);
        
        playerSeen = IsPlayerSeen();
        
        if(playerSeen == true || draggingPlayer == true)
        {
            dashing = true;
            UpdateAdjustedVelocity();
            
            if(draggingPlayer == true)
            {
                playerDraggingPosition.x = transform.position.x - currentDirection.x * playerDraggingOffset;
                playerDraggingPosition.y = playerObject.transform.position.y;
                playerDraggingPosition.z = transform.position.z - currentDirection.y * playerDraggingOffset;
                
                playerObject.transform.position = playerDraggingPosition;
            }
        }
        
        if(delta.x < adjustedVelocity && delta.z < adjustedVelocity)
        {
            transform.position = nextPivot;
            
            if(playerSeen == false)
            {
                dashing = false;
                UpdateAdjustedVelocity();
            }
            
            CalculateNextPivot();
        }
        else
        {
            transform.position = new Vector3(transform.position.x + currentDirection.x * adjustedVelocity,
                                             transform.position.y,
                                             transform.position.z + currentDirection.y * adjustedVelocity);
        }
    }
    
    void UpdateAdjustedVelocity()
    {
        if(dashing == true)
        {
            adjustedVelocity = dashVelocity * Generator.wallScale * Time.fixedDeltaTime;
        }
        else
        {
            adjustedVelocity = velocity * Generator.wallScale * Time.fixedDeltaTime;
        }
    }
    
    List<Vector2Int> CalculateValidDirections(Vector2Int element)
    {
        List<Vector2Int> validDirections = new List<Vector2Int>();
        Vector2Int nextElement = new Vector2Int();
        
        foreach(var direction in directions)
        {
            nextElement = element + direction;
            
            if(MazeGenerator[nextElement.x, nextElement.y] != Generator.MazeElement.Wall &&
               MazeGenerator[nextElement.x, nextElement.y] != Generator.MazeElement.Room &&
               MazeGenerator[nextElement.x, nextElement.y] != Generator.MazeElement.OutOfBounds)
            {
                validDirections.Add(new Vector2Int(direction.x, direction.y));
            }
        }
        
        return validDirections;
    }
    
    void CalculateNextPivot()
    {
        Vector2Int mazeIndices = MazeGenerator.PositionToMazeIndices(this.transform.position);
        List<Vector2Int> validDirections;
        Vector2Int direction;
        Vector2Int nextElement;
        float stride = 0.0f;
        
        validDirections = CalculateValidDirections(mazeIndices);
        
        if(validDirections.Count > 0)
        {
            if(playerSeen == true && validDirections.Contains(currentDirection))
            {
                direction = currentDirection;
            }
            else
            {
                direction = validDirections[Random.Range(0, validDirections.Count)];
            }
            
            stride += Generator.wallScale;
            nextElement = mazeIndices + direction + direction;
            while(MazeGenerator[nextElement.x, nextElement.y] != Generator.MazeElement.Wall &&
                  MazeGenerator[nextElement.x, nextElement.y] != Generator.MazeElement.Room &&
                  MazeGenerator[nextElement.x, nextElement.y] != Generator.MazeElement.OutOfBounds)
            {
                stride += Generator.wallScale;
                
                if(CalculateValidDirections(nextElement).Count > 2 && playerSeen == false)
                {
                    break;
                }
                
                nextElement = nextElement + direction;
            }
            
            currentDirection = direction;
            
            nextPivot.x = transform.position.x + (float) direction.x * stride;
            nextPivot.y = transform.position.y;
            nextPivot.z = transform.position.z + (float) direction.y * stride;
            
            nextPivotElement = MazeGenerator.PositionToMazeIndices(nextPivot);
        }
    }
    
    bool IsPlayerSeen()
    {
        Vector2Int scanElement = MazeGenerator.PositionToMazeIndices(this.transform.position) + currentDirection;
        Vector2Int playerElement = MazeGenerator.PositionToMazeIndices(playerObject.transform.position);
        
        while(MazeGenerator[scanElement.x, scanElement.y] != Generator.MazeElement.Wall &&
              MazeGenerator[scanElement.x, scanElement.y] != Generator.MazeElement.Room &&
              MazeGenerator[scanElement.x, scanElement.y] != Generator.MazeElement.OutOfBounds)
        {
            if(scanElement.Equals(playerElement))
            {
                return true;
            }
            
            scanElement += currentDirection;
        }
        
        return false;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            draggingPlayer = true;
            Invoke("Respawn", 4.0f);
        }
    }
    
    void Respawn()
    {
        MazeGenerator.InstantiateGhost();
        
        Destroy(gameObject);
    }
}
