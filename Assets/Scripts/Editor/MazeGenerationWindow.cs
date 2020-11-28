using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// Provides a way to generate mazes before runtime. This can be used to
/// play the game multiple times on the same maze.
/// </summary>
public class MazeGenerationWindow : EditorWindow
{
    /// <summary>
    /// The game object to which the maze generator script is attached.
    /// </summary>
    private Generator generator;

    /// <summary>
    /// Shows the custom window in the editor.
    /// </summary>
    [MenuItem("Custom/Maze Generation")]
    public static void ShowWindow()
    {
        GetWindow<MazeGenerationWindow>();
    }

    /// <summary>
    /// This method is invoked on startup.
    /// </summary>
    protected void OnEnable()
    {
        // If a generator exists in the scene, assign it as default
        generator = FindObjectOfType<Generator>();
    }

    /// <summary>
    /// This method is invoked to set up the custom window whenever its GUI is reloaded.
    /// </summary>
    protected void OnGUI()
    {
        GUI.enabled = true;
        GUILayout.Label("Generate Maze", EditorStyles.boldLabel);

        // Create user-input inspector values
        generator = EditorGUILayout.ObjectField("Maze Generator", generator, typeof(Generator), allowSceneObjects: true) as Generator;
        var size = EditorGUILayout.IntField("Maze Size", generator?.defaultSize ?? Generator.MinMazeSize);

        // Enable generate maze button if generator selected
        GUI.enabled = generator is Generator;
        if (GUILayout.Button("Generate New Maze!"))
        {
            try
            {
                generator.NewMaze(size);
            }
            catch (System.ArgumentException e)
            {
                // If size is invalid, display message for how to fix it
                ShowNotification(new GUIContent(e.Message));
            }
        }

        // Enable destroy maze button if generator selected
        GUI.enabled = generator?.IsGenerated == true;
        if (GUILayout.Button("Destroy Maze!"))
        {
            generator.DestroyMaze();
        }

        // Enable destroy all walls button if any walls exist
        GUI.enabled = WallsExist();
        if (GUILayout.Button("Destroy All Walls!"))
        {
            DestroyAllWalls();
        }
    }

    /// <summary>
    /// Returns whether any walls have been generated.
    /// </summary>
    /// <returns><see langword="true"/> if any walls already exist; <see langword="false"/> otherwise.</returns>
    public static bool WallsExist()
    {
        return GameObject.FindGameObjectsWithTag(Generator.WallTag).Count() > 0;
    }

    /// <summary>
    /// Destroys all objects with the wall tag.
    /// </summary>
    public static void DestroyAllWalls()
    {
        foreach (var wall in GameObject.FindGameObjectsWithTag(Generator.WallTag))
        {
            DestroyImmediate(wall);
        }
    }
}