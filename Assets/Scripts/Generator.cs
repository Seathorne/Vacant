using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

public class Generator : EditorWindow
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
    /// The tag given to all walls generated as part of the maze.
    /// </summary>
    public const string WallTag = "Wall";

    private int mazeSize = 21;

    private GameObject floor;
    private GameObject wallPrefab;
    
    private Vector3 floorScale;
    private int[,] mazeData;

    /// <summary>
    /// A collection of walls that have already been generated.
    /// </summary>
    private readonly List<GameObject> walls = new List<GameObject>();

    /// <summary>
    /// Gets whether a maze has been generated:
    /// <see langword="true"/> if a maze is currently generated; <see langword="false"/> otherwise.
    /// </summary>
    public bool IsGenerated { get; private set; } = false;

    /// <summary>
    /// Shows the custom window in the editor.
    /// </summary>
    [MenuItem("Custom/Maze Generation")]
    public static void ShowWindow()
    {
        GetWindow<Generator>();
    }

    /// <summary>
    /// This method is invoked to set up the custom window whenever its GUI is reloaded.
    /// </summary>
    private void OnGUI()
    {
        GUI.enabled = true;

        GUILayout.Label("Generate Maze", EditorStyles.boldLabel);

        // Create user-input inspector values
        mazeSize = EditorGUILayout.IntField("Maze Size", mazeSize);
        floor = EditorGUILayout.ObjectField("Floor", floor, typeof(GameObject), allowSceneObjects: true) as GameObject;
        wallPrefab = EditorGUILayout.ObjectField("Walls", wallPrefab, typeof(GameObject), allowSceneObjects: true) as GameObject;

        // Disable generate button if maze already generated
        GUI.enabled = !IsGenerated;
        if (GUILayout.Button("Generate Maze!"))
        {
            if (mazeSize % 2 == 0)
            {
                ShowNotification(new GUIContent("Maze Size must be an odd number"));

                GUI.enabled = false;
            }
            else if (mazeSize < MinMazeSize || mazeSize > MaxMazeSize)
            {
                ShowNotification(new GUIContent($"Maze Size must be within interval [{MinMazeSize}, {MaxMazeSize}]"));

                GUI.enabled = false;
            }
            else if (floor is null)
            {
                ShowNotification(new GUIContent("No floor object selected"));

                GUI.enabled = false;
            }
            else if (wallPrefab is null)
            {
                ShowNotification(new GUIContent("No wall objects selected"));

                GUI.enabled = false;
            }
            else
            {
                // If all inspector values valid, generate maze
                GenerateMaze();
            }
        }

        // Disable destroy button if maze not yet generated
        GUI.enabled = true;
        if (GUILayout.Button("Destroy Maze!"))
        {
            DestroyMaze();
            IsGenerated = false;
        }

        GUI.enabled = true;
    }
    
    /// <summary>
    /// Calculates and instantiates a new maze in the scene.
    /// </summary>
    void GenerateMaze()
    {
        if (IsGenerated)
        {
            throw new System.InvalidOperationException("Maze should not be generated twice in a row!");
        }

        floorScale = floor.transform.localScale;
        mazeData = new int[mazeSize, mazeSize];

        CalculateMaze();
        InstantiateMaze();

        IsGenerated = true;
    }

    /// <summary>
    /// Destroys all walls composing a maze that has already been generated.
    /// </summary>
    void DestroyMaze()
    {
        // Destroy all generated walls
        if (walls.Count > 0)
        {
            foreach (var wall in walls)
            {
                DestroyImmediate(wall);
            }

            walls.Clear();
        }
        else
        {
            var children = from wall in GameObject.FindGameObjectsWithTag(WallTag)
                           where wall.tag == WallTag
                           select wall;

            foreach (var child in children)
            {
                DestroyImmediate(child);
            }
        }

        IsGenerated = false;
    }

    int GetMazeElement(int x, int y)
    {
        if(x < 0 || x > mazeSize - 1 || y < 0 || y > mazeSize - 1)
        {
            return -1;
        }
        
        return mazeData[x, y];
    }
    
    void CalculateMaze()
    {
        int startingRoomBounds = (4 * mazeSize) / 10;
        Vector2Int currentCell = new Vector2Int(2 * Random.Range(0, mazeSize / 2) + 1, 2 * Random.Range(0, mazeSize / 2) + 1);
        List<Vector2Int> pathCells = new List<Vector2Int>();
        
        for(int i = startingRoomBounds; i < mazeSize - startingRoomBounds; i++)
        {
            for(int j = startingRoomBounds; j < mazeSize - startingRoomBounds; j++)
            {
                mazeData[i, j] = 2;
            }
        }
        
        pathCells.Add(currentCell);
        while(true)
        {
            List<Vector2Int> validDirections = new List<Vector2Int>();
            Vector2Int direction;
            Vector2Int nextCell = new Vector2Int();
            Vector2Int dividingWall = new Vector2Int();
            
            validDirections.Add(new Vector2Int(-1,  0));
            validDirections.Add(new Vector2Int( 1,  0));
            validDirections.Add(new Vector2Int( 0, -1));
            validDirections.Add(new Vector2Int( 0,  1));
            
            for(int i = 0; i < validDirections.Count;)
            {
                nextCell.Set(currentCell.x + 2 * validDirections[i].x, currentCell.y + 2 * validDirections[i].y);
                dividingWall.Set(currentCell.x + validDirections[i].x, currentCell.y + validDirections[i].y);
                
                if(GetMazeElement(nextCell.x, nextCell.y) != 0 || GetMazeElement(dividingWall.x, dividingWall.y) != 0)
                {
                    validDirections.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            
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
            
            direction = validDirections[Random.Range(0, validDirections.Count)];
            nextCell.Set(currentCell.x + 2 * direction.x, currentCell.y + 2 * direction.y);
            dividingWall.Set(currentCell.x + direction.x, currentCell.y + direction.y);
            
            mazeData[nextCell.x, nextCell.y] = 1;
            mazeData[dividingWall.x, dividingWall.y] = 1;
            
            pathCells.Add(nextCell);
            currentCell = nextCell;
        }
        
        for(int i = 0; i < mazeSize; i += mazeSize - 1)
        {
            for(int j = 0; j < mazeSize; j++)
            {
                mazeData[i, j] = 0;
            }
        }
        
        for(int i = 0; i < mazeSize; i += mazeSize - 1)
        {
            for(int j = 0; j < mazeSize; j++)
            {
                mazeData[j, i] = 0;
            }
        }

        for(int i = startingRoomBounds; i < mazeSize - startingRoomBounds; i++)
        {
            for(int j = startingRoomBounds; j < mazeSize - startingRoomBounds; j++)
            {
                if(i == startingRoomBounds || i == mazeSize - startingRoomBounds - 1 ||
                   j == startingRoomBounds || j == mazeSize - startingRoomBounds - 1)
                {
                    mazeData[i, j] = 0;
                }
                else
                {
                    mazeData[i, j] = 1;
                }
            }
        }
        
        GenerateDoor(new Vector2Int(0, 0), new Vector2Int(mazeSize - 1, mazeSize - 1));
        GenerateDoor(new Vector2Int(startingRoomBounds, startingRoomBounds),
                     new Vector2Int(mazeSize - startingRoomBounds - 1, mazeSize - startingRoomBounds - 1));

        // 0 = wall
        // 1 = path
        // 2 = off-limits
    }
    
    void GenerateDoor(Vector2Int topLeft, Vector2Int bottomRight)
    {
        string axis = Random.Range(0, 2) == 0 ? "x" : "y";
        int wall = Random.Range(0, 2);
        
        if(axis == "x")
        {
            while(true)
            {
                int doorOffset = Random.Range(topLeft.x + 1, bottomRight.x);
                Vector2Int door = new Vector2Int();
                
                door.Set(doorOffset, wall == 0 ? topLeft.y : bottomRight.y);
                
                if(GetMazeElement(door.x, door.y - 1) != 0 && GetMazeElement(door.x, door.y + 1) != 0)
                {
                    mazeData[door.x, door.y] = 1;
                    
                    break;
                }
            }
        }
        
        if(axis == "y")
        {
            while(true)
            {
                int doorOffset = Random.Range(topLeft.y + 1, bottomRight.y);
                Vector2Int door = new Vector2Int();
                
                door.Set(wall == 0 ? topLeft.x : bottomRight.x, doorOffset);
                
                if(Mathf.Abs(GetMazeElement(door.x - 1, door.y)) == 1 &&
                   Mathf.Abs(GetMazeElement(door.x + 1, door.y)) == 1)
                {
                    mazeData[door.x, door.y] = 1;
                    
                    break;
                }
            }
        }
    }
    
    void InstantiateMaze()
    {
        float wallScale = (floorScale.x * 10.0f) / (float) mazeSize;
        float floorOffset = floorScale.x * 5.0f;
        
        for(int i = 0; i < mazeData.GetLength(0); i++)
        {
            for(int j = 0; j < mazeData.GetLength(1); j++)
            {
                if(mazeData[i,j] == 0)
                {
                    Vector3 wallPosition = new Vector3();
                    GameObject wall;
                    
                    wallPosition.x = wallScale / 2.0f + j * wallScale - floorOffset;
                    wallPosition.y = (wallPrefab.transform.localScale.y * wallScale) / 2.0f;
                    wallPosition.z = wallScale / 2.0f + i * wallScale - floorOffset;
                        
                    wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                    wall.transform.localScale = new Vector3(wallScale, wallScale, wallScale);
                    wall.transform.parent = floor.transform;
                    wall.tag = WallTag;

                    walls.Add(wall);
                }
            }
        }
    }
}
