using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProperties : MonoBehaviour
{
    public static bool isGamePaused = false;

    public static int playerScore = 0;

    public static string levelFilePath = "Level";

    public const int initialSnakeLength = 3;

    public static int gameLevel = 1;

    public const string levelPathInResources = "Levels/Level_";
}
