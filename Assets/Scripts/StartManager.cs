using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles core functions in the Start scene
/// </summary>
public class StartManager : MonoBehaviour
{
    
    /// <summary>
    /// Called by the Levels buttons, it sets the hardness of the levels, the complete path for the level file that should be loaded and loads the Main scene
    /// </summary>
    /// <param name="levelId"></param>
    public void SetLevelTo(int levelId)
    {
        GameProperties.levelFilePath = GameProperties.levelPathInResources + levelId.ToString();
        GameProperties.gameLevel = levelId;
        SceneManager.LoadScene(1);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
