using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI of the Main scene
/// </summary>
public class UIManager : MonoBehaviour {

    /// <summary>
    /// Reference to the score Text component in the gameplay window
    /// </summary>
    [SerializeField]
    private Text scoreValue;

    /// <summary>
    /// Reference to the Gameover popup
    /// </summary>
    [SerializeField]
    private GameObject gameoverPopup;

    /// <summary>
    /// Referebce to the Score text in the Gameover Popup
    /// </summary>
    [SerializeField]
    private Text finalScoreText;

    /// <summary>
    /// Shows or hide the Gameover popup
    /// </summary>
    public bool GameoverPopupVisible
    {
        get
        {
            return gameoverPopup.activeSelf;
        }
        set
        {
            finalScoreText.text = Utils.GetScoreString();
            gameoverPopup.SetActive(value);
        }
    }

    /// <summary>
    /// Sets the score text or retrieve the string of the Score Text
    /// </summary>
    public string Score
    {
        get
        {
            return scoreValue.ToString();
        }
        set
        {
            scoreValue.text = value;
        }
    }

    public Action OnPausePressed;
    public Action OnResumePressed;
    public Action OnQuitPressed;
    public Action OnRestartPressed;
    public Action OnHomePressed;
    
    /// <summary>
    /// Called by Canvas > Pause button
    /// </summary>
    public void PressPause()
    {
        OnPausePressed.Invoke();
    }

    /// <summary>
    /// Called by Canvas > PausePopup > Resume button
    /// </summary>
    public void PressResume()
    {
        OnResumePressed.Invoke();
    }

    /// <summary>
    /// Called by Canvas > PausePopup/GameoverPopup > Quit button
    /// </summary>
    public void PressQuit()
    {
        OnQuitPressed.Invoke();
    }

    /// <summary>
    /// Called by Canvas > PausePopup/GameoverPopup > Restart button
    /// </summary>
    public void PressRestart()
    {
        OnRestartPressed.Invoke();
    }

    /// <summary>
    /// Called by Canvas > PausePopup/GameoverPopup > Home button
    /// </summary>
    public void PressHome()
    {
        OnHomePressed.Invoke();
    }
    
}
