using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour {
    
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
