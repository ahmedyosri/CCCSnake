using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as a cloud info storage
/// </summary>
public class GameProperties : MonoBehaviour
{
    /// <summary>
    /// Tells if the game is paused or not
    /// </summary>
    public static bool isGamePaused = false;

    /// <summary>
    /// The current score of the player in the current game
    /// </summary>
    public static int playerScore = 0;

    /// <summary>
    /// The path of the level file that should be loaded in the Main scene. Set from the Start scene by <see cref="StartManager.SetLevelTo(int)"/>
    /// </summary>
    public static string levelFilePath = "Level";

    /// <summary>
    /// The initial snake length when a new game is starting
    /// </summary>
    public const int initialSnakeLength = 3;

    /// <summary>
    /// The hardness of the current level. Set from the Start scene by <see cref="StartManager.SetLevelTo(int)"/>
    /// </summary>
    public static int gameLevel = 1;

    /// <summary>
    /// The shared level files path in the resource, hence levels paths+names are Assts/Resources/"Level/Level_1, Level_2, Level_3 ... etc"
    /// </summary>
    public const string levelPathInResources = "Levels/Level_";
}
