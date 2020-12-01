﻿using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Generates mazes of a specified size.
/// </summary>
public class Generator : MonoBehaviour
{
    /// <summary>
    /// The minimum allowable width and height (in number of walls) for the maze.
    /// </summary>
    public const int MinMazeSize = 15;

    /// <summary>
    /// The maximum allowable width and height (in number of walls) for the maze.
    /// </summary>
    public const int MaxMazeSize = 101;

    /// <summary>
    /// The default width and height (in number of elements) of the maze.
    /// </summary>
    public int defaultMazeSize = 21;
    
    /// <summary>
    /// The empty game object that contains all wall objects.
    /// </summary>
    public GameObject wallsEmpty;
    
    /// <summary>
    /// The empty game object that contains all light objects.
    /// </summary>
    public GameObject lightsEmpty;
    
    /// <summary>
    /// The empty game object that contains all ghost objects.
    /// </summary>
    public GameObject ghostsEmpty;
    
    /// <summary>
    /// The prefab of the wall used throughout the maze.
    /// </summary>
    public GameObject wallPrefab;
    
    /// <summary>
    /// The prefab of the overhead light used in the maze.
    /// </summary>
    public GameObject lightPrefab;
    
    /// <summary>
    /// The prefab of the wall used throughout the maze.
    /// </summary>
    public GameObject ghostPrefab;
    
    /// <summary>
    /// The prefab of the compass.
    /// </summary>
    public GameObject compassPrefab;
    
    /// <summary>
    /// The prefab of the maze exit doorway.
    /// </summary>
    public GameObject exitPrefab;
    
    /// <summary>
    /// The default scale used for each maze object.
    /// </summary>
    public static float wallScale = 8.0f;
    
    /// <summary>
    /// The game object to use as the maze ceiling.
    /// </summary>
    public GameObject ceilingObject;

    /// <summary>
    /// The game object to use as the maze floor.
    /// </summary>
    public GameObject floorObject;

    /// <summary>
    /// Whether the generator should automatically generate a maze on startup.
    /// </summary>
    public bool autoGenerate;

    /// <summary>
    /// Gets whether a maze has been generated:
    /// <see langword="true"/> if a maze is currently generated; <see langword="false"/> otherwise.
    /// </summary>
    public bool IsGenerated { get; private set; } = false;
    
    /// <summary>
    /// Enumerations for the element types generated by GenerateMaze()
    /// </summary>
    public enum MazeElement : int
    {
        OutOfBounds,
        Wall,
        Path,
        Room,
    };

    /// <summary>
    /// The width and height (in number of walls) of the maze that is
    /// generated, set by the parameter to <see cref="NewMaze(int)"/>.
    /// </summary>
    private int mazeSize;

    /// <summary>
    /// The scaling factor of the floor on which the maze rests.
    /// </summary>
    private float floorScale;
    
    /// <summary>
    /// The array of abstract representations of walls
    /// </summary>
    private MazeElement[,] mazeData;

    /// <summary>
    /// A collection of walls that have already been generated and make up
    /// the current maze.
    /// </summary>
    private readonly List<GameObject> mazeObjects = new List<GameObject>();

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    protected void Start()
    {
        // Clamp maze size to minimum and maximum
        mazeSize = Mathf.Clamp(defaultMazeSize, MinMazeSize, MaxMazeSize);
        IsGenerated = !autoGenerate;

        // If a maze is not already created
        if (!IsGenerated)
        {
            // Create new maze
            NewMaze(defaultMazeSize);
        }
    }

    /// <summary>
    /// Generates a new maze.
    /// </summary>
    /// <param name="size">The width and height (in number of walls) of the maze.</param>
    public void NewMaze(int size)
    {
        // If minimum and maximum maze sizes are invalid
        if (size < MinMazeSize || size > MaxMazeSize)
        {
            throw new System.ArgumentException($"Size must be in interval [{MinMazeSize}, {MaxMazeSize}].", nameof(size));
        }

        // If maze size is even
        if (size % 2 == 0)
        {
            throw new System.ArgumentException($"Size must be an even number.", nameof(size));
        }

        // If a maze already exists, destroy it
        DestroyMaze();
        
        // Set initial maze data to walls
        mazeSize = size;
        mazeData = new MazeElement[mazeSize, mazeSize];
        for (int i = 0; i < mazeSize - 1; i++)
        {
            for(int j = 0; j < mazeSize - 1; j++)
            {
                this[i, j] = MazeElement.Wall;
            }
        }
        
        //Scale and position maze floor and ceiling
        floorScale = (wallScale * mazeSize) / 10.0f;
        floorObject.transform.localScale = new Vector3(floorScale, floorScale, floorScale);
        ceilingObject.transform.localScale = new Vector3(floorScale, floorScale, floorScale);
        ceilingObject.transform.position = ceilingObject.transform.position + (new Vector3(0.0f, wallScale, 0.0f));
        
        // Generate and instantiate maze
        GenerateMaze();
        InstantiateMaze();
        IsGenerated = true;
    }

    /// <summary>
    /// Destroys all objects composing a maze that has already been generated.
    /// </summary>
    public void DestroyMaze()
    {
        // Destroy all generated walls
        foreach (var mazeObject in mazeObjects)
        {
            DestroyImmediate(mazeObject);
        }
        
        // Clear all maze objects
        mazeObjects.Clear();
        IsGenerated = false;
    }
    
    /// <summary>
    /// Safe operator overload to access maze data.
    /// </summary>
    /// <param name="x">X index</param>
    /// <param name="y">Y index</param>
    public MazeElement this[int x, int y]
    {
        get
        {
            if (x < 0 || x > mazeSize - 1 || y < 0 || y > mazeSize - 1)
            {
                return MazeElement.OutOfBounds;
            }

            return mazeData[x, y];
        }
        
        private set
        {
            if (x >= 0 && x <= mazeSize - 1 && y >= 0 && y <= mazeSize - 1)
            {
                mazeData[x, y] = value;
            }
            else
            {
                Debug.Log("Maze data index out of bounds!");
            }
        }
    }
    
    /// <summary>
    /// Converts world position to maze indices
    /// </summary>
    /// <param name="position">World position to convert</param>
    public Vector2Int PositionToMazeIndices(Vector3 position)
    {
        Vector2Int mazeIndices = new Vector2Int();
        
        // Convert world position into maze indices
        mazeIndices.x = (int) Mathf.Floor((position.x + floorScale * 5.0f) / wallScale);
        mazeIndices.y = (int) Mathf.Floor((position.z + floorScale * 5.0f) / wallScale);
        
        return mazeIndices;
    }
    
    /// <summary>
    /// Generate the matrix for a new maze, including all items and ghosts.
    /// </summary>
    public void GenerateMaze()
    {
        int startingRoomBounds = (4 * mazeSize) / 10;
        Vector2Int currentCell = new Vector2Int(2 * Random.Range(0, mazeSize / 2) + 1, 2 * Random.Range(0, mazeSize / 2) + 1);
        List<Vector2Int> pathCells = new List<Vector2Int>();
        
        // Create starting room
        for(int i = startingRoomBounds; i < mazeSize - startingRoomBounds; i++)
        {
            for(int j = startingRoomBounds; j < mazeSize - startingRoomBounds; j++)
            {
                this[i, j] = MazeElement.Room;
            }
        }
        
        // Generate backtracking packed maze
        pathCells.Add(currentCell);
        while(true)
        {
            List<Vector2Int> validDirections = new List<Vector2Int>();
            Vector2Int direction;
            Vector2Int nextCell = new Vector2Int();
            Vector2Int dividingWall = new Vector2Int();
            
            // Create list of possible directions
            validDirections.Add(new Vector2Int(-1,  0));
            validDirections.Add(new Vector2Int( 1,  0));
            validDirections.Add(new Vector2Int( 0, -1));
            validDirections.Add(new Vector2Int( 0,  1));
            
            // Check possible directions and remove invalid ones
            for(int i = 0; i < validDirections.Count;)
            {
                nextCell.Set(currentCell.x + 2 * validDirections[i].x, currentCell.y + 2 * validDirections[i].y);
                dividingWall.Set(currentCell.x + validDirections[i].x, currentCell.y + validDirections[i].y);
                
                // If direction is invalid (next cell or dividing wall are already maze path)
                if(this[nextCell.x, nextCell.y] != MazeElement.Wall || this[dividingWall.x, dividingWall.y] != MazeElement.Wall)
                {
                    validDirections.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            
            // If no valid directions are left (dead end), backtrack
            if(validDirections.Count == 0)
            {
                if(pathCells.Count == 1)
                {
                    break;
                }
                
                pathCells.RemoveAt(pathCells.Count - 1);
                currentCell = pathCells[pathCells.Count - 1];
                
                continue;
            }
            
            // Set new direction
            direction = validDirections[Random.Range(0, validDirections.Count)];
            nextCell.Set(currentCell.x + 2 * direction.x, currentCell.y + 2 * direction.y);
            dividingWall.Set(currentCell.x + direction.x, currentCell.y + direction.y);
            
            // Carve out path in that direction
            this[nextCell.x, nextCell.y] = MazeElement.Path;
            this[dividingWall.x, dividingWall.y] = MazeElement.Path;
            
            // Move to next cell in chosen direction
            pathCells.Add(nextCell);
            currentCell = nextCell;
        }
        
        // Create walls around perimeter of maze
        for(int i = 0; i < mazeSize; i += mazeSize - 1)
        {
            for(int j = 0; j < mazeSize; j++)
            {
                this[i, j] = MazeElement.Wall;
            }
        }
        for(int i = 0; i < mazeSize; i += mazeSize - 1)
        {
            for(int j = 0; j < mazeSize; j++)
            {
                this[j, i] = MazeElement.Wall;
            }
        }
        
        // Create walls around perimeter of starting room
        for(int i = startingRoomBounds; i < mazeSize - startingRoomBounds; i++)
        {
            for(int j = startingRoomBounds; j < mazeSize - startingRoomBounds; j++)
            {
                if(i == startingRoomBounds || i == mazeSize - startingRoomBounds - 1 ||
                   j == startingRoomBounds || j == mazeSize - startingRoomBounds - 1)
                {
                    this[i, j] = MazeElement.Wall;
                }
            }
        }
        
        // Generate exit door and instantiate exit door prefab
        InstantiateExit(GenerateDoor(new Vector2Int(0, 0), new Vector2Int(mazeSize - 1, mazeSize - 1)));
        
        // Generate starting room door
        GenerateDoor(new Vector2Int(startingRoomBounds, startingRoomBounds),
                     new Vector2Int(mazeSize - startingRoomBounds - 1, mazeSize - startingRoomBounds - 1));
    }
    
    /// <summary>
    /// Generates valid and randomly positioned door along perimeter of given rectangle.
    /// </summary>
    /// <param name="topLeft">Top-left corner of rectangle.</param>
    /// <param name="bottomRight">Bottom-right corner of rectangle.</param>
    Vector2Int GenerateDoor(Vector2Int topLeft, Vector2Int bottomRight)
    {
        string axis = Random.Range(0, 2) == 0 ? "x" : "y";
        int wall = Random.Range(0, 2);
        Vector2Int door = new Vector2Int();
        
        // If door is to be on x-axis
        if(axis == "x")
        {
            while(true)
            {
                int doorOffset = Random.Range(topLeft.x + 1, bottomRight.x);
                
                // Pick one of the two horizontal walls
                door.Set(doorOffset, wall == 0 ? topLeft.y : bottomRight.y);
                
                // Place door on viable space of selected wall
                if(this[door.x, door.y - 1] != MazeElement.Wall && 
                   this[door.x, door.y + 1] != MazeElement.Wall)
                {
                    this[door.x, door.y] = MazeElement.Path;
                    
                    break;
                }
            }
        }
        
        // If door is to be on y-axis
        if(axis == "y")
        {
            while(true)
            {
                int doorOffset = Random.Range(topLeft.y + 1, bottomRight.y);
                
                // Pick one of the two vertical walls
                door.Set(wall == 0 ? topLeft.x : bottomRight.x, doorOffset);
                
                // Place door on viable space of selected wall
                if(this[door.x - 1, door.y] != MazeElement.Wall &&
                   this[door.x + 1, door.y] != MazeElement.Wall)
                {
                    this[door.x, door.y] = MazeElement.Path;
                    
                    break;
                }
            }
        }
        
        return door;
    }
    
    /// <summary>
    /// Instantiates overhead lights inside maze.
    /// </summary>
    public void InstantiateLights()
    {
        float floorOffset = floorScale * 5.0f;
        float scale = wallScale * 0.3f;
        Vector3 position = new Vector3();
        GameObject lightObject;
        
        // Set vertical location to ceiling of maze
        position.y = wallScale - (lightPrefab.transform.localScale.y / 2);
        
        // Loop through maze size in increments of 2 (for light positions)
        for(int i = 1; i < mazeSize; i += 2)
        {
            for(int j = 1; j < mazeSize; j += 2)
            {
                // Calculate position
                position.x = wallScale / 2.0f + i * wallScale - floorOffset;
                position.z = wallScale / 2.0f + j * wallScale - floorOffset;
                
                // Instantiate prefab
                lightObject = Instantiate(lightPrefab, position, Quaternion.identity);
                lightObject.transform.parent = lightsEmpty.transform;
                lightObject.tag = Tag.Light;
                
                // Set scale and add to object list
                lightObject.transform.localScale = new Vector3(scale, scale, scale);
                mazeObjects.Add(lightObject);
            }
        }
    }
    
    /// <summary>
    /// Instantiates one randomly-positioned ghost.
    /// </summary>
    public void InstantiateGhost()
    {
        int x;
        int y;
        float floorOffset = floorScale * 5.0f;
        float scale = wallScale * 0.5f;
        Vector3 position = new Vector3();
        GameObject ghostObject;
        
        // Generate random coordinates till path space is found
        while(this[(x = Random.Range(1, mazeSize - 1)), (y = Random.Range(1, mazeSize - 1))] != MazeElement.Path);
        
        // Calculate position
        position.x = wallScale / 2.0f + x * wallScale - floorOffset;
        position.y = wallScale / 2.0f;
        position.z = wallScale / 2.0f + y * wallScale - floorOffset;
        
        // Instantiate prefab
        ghostObject = Instantiate(ghostPrefab, position, Quaternion.identity);
        ghostObject.transform.parent = ghostsEmpty.transform;
        ghostObject.tag = Tag.Ghost;
        
        // Set scale and add to object list
        ghostObject.transform.localScale = ghostPrefab.transform.localScale * scale;
        mazeObjects.Add(ghostObject);
    }
    
    /// <summary>
    /// Instantiates exit door prefab.
    /// </summary>
    /// <param name="indices">Indices of exit door.</param>
    void InstantiateExit(Vector2Int indices)
    {
        float floorOffset = floorScale * 5.0f;
        float scale = 1.0f;
        Vector3 position = new Vector3();
        GameObject exitObject;
        float rotation = 0.0f;
        
        // Instantiate prefab
        exitObject = Instantiate(exitPrefab, position, Quaternion.identity);
        exitObject.transform.parent = transform.parent;
        exitObject.tag = Tag.Exit;
        
        // Set initial scale
        exitObject.transform.localScale = new Vector3(scale, scale, scale);
        mazeObjects.Add(exitObject);
        
        // Set rotation and scale
        if(this[indices.x + 1, indices.y] == MazeElement.OutOfBounds)
        {
            rotation = 90.0f;
            position.x = wallScale / 2.0f;
            
        }
        else if(this[indices.x - 1, indices.y] == MazeElement.OutOfBounds)
        {
            rotation = -90.0f;
            position.x = -wallScale / 2.0f;
        }
        else if(this[indices.x, indices.y + 1] == MazeElement.OutOfBounds)
        {
            rotation = 0.0f;
            position.z = wallScale / 2.0f;
        }
        else if(this[indices.x, indices.y - 1] == MazeElement.OutOfBounds)
        {
            rotation = 180.0f;
            position.z = -wallScale / 2.0f;
        }
        
        // Set position
        position.x += wallScale / 2.0f + indices.x * wallScale - floorOffset;
        position.y = wallScale / 2.0f;
        position.z += wallScale / 2.0f + indices.y * wallScale - floorOffset;
        exitObject.transform.position = position;
        
        // Rotate door
        exitObject.transform.eulerAngles = new Vector3(
            exitObject.transform.eulerAngles.x,
            exitObject.transform.eulerAngles.y + rotation,
            exitObject.transform.eulerAngles.z
        );
    }
    
    /// <summary>
    /// Instantiates compass at random valid location in maze.
    /// </summary>
    void InstantiateCompass()
    {
        int x;
        int y;
        float floorOffset = floorScale * 5.0f;
        float scale = wallScale * 0.2f;
        Vector3 position = new Vector3();
        GameObject compassObject;
        
        // Generate random coordinates till path space is found
        while(this[(x = Random.Range(1, mazeSize - 1)), (y = Random.Range(1, mazeSize - 1))] != MazeElement.Path);
        
        // Calculate position
        position.x = wallScale / 2.0f + x * wallScale - floorOffset;
        position.y = wallScale / 4.0f;
        position.z = wallScale / 2.0f + y * wallScale - floorOffset;
        
        // Instantiate prefab
        compassObject = Instantiate(compassPrefab, position, Quaternion.identity);
        compassObject.tag = Tag.Compass;
        
        // Adjust scale, point location, and add to object list
        compassObject.transform.localScale = new Vector3(scale, scale, scale);
        compassObject.GetComponent<Compass>().PointLocation = GameObject.Find("Sunlight").transform.position;
        mazeObjects.Add(compassObject);
    }
    
    /// <summary>
    /// Instantiates entire maze.
    /// </summary>
    void InstantiateMaze()
    {
        float floorOffset = floorScale * 5.0f;
        
        // Loop through maze data
        for(int i = 0; i < mazeData.GetLength(0); i++)
        {
            for(int j = 0; j < mazeData.GetLength(1); j++)
            {
                Vector3 position = new Vector3();
                GameObject prefab = null;
                GameObject mazeObject = null;
                GameObject parent = null;
                string tag = null;
                float mazeObjectScale = 1.0f;
                
                // Check which object is being instantiated
                switch(this[i, j])
                {
                    case MazeElement.Wall:
                        prefab = wallPrefab;
                        parent = wallsEmpty;
                        tag = Tag.Wall;
                        break;
                }
                
                // Instantiate maze walls
                if(prefab != null)
                {
                    float scale = wallScale * mazeObjectScale;
                    
                    // Calculate position
                    position.x = wallScale / 2.0f + i * wallScale - floorOffset;
                    position.y = wallScale / 2.0f;
                    position.z = wallScale / 2.0f + j * wallScale - floorOffset;
                    
                    // Instantiate prefab
                    mazeObject = Instantiate(prefab, position, Quaternion.identity);
                    mazeObject.transform.parent = parent.transform;
                    mazeObject.tag = tag;
                    
                    // Adjust scale and add to object list
                    mazeObject.transform.localScale = new Vector3(scale, scale, scale);
                    mazeObjects.Add(mazeObject);
                }
            }
        }
        
        // Instantiate lights
        InstantiateLights();
        
        // Instantiate ghosts
        for (int i = 0; i < (int) Mathf.Floor((mazeSize * mazeSize) / 100); i++)
        {
            InstantiateGhost();
        }
        
        // Instantiate compass
        InstantiateCompass();
    }
}
