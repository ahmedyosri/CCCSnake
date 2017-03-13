using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Text scoreValue;

    [SerializeField]
    private GameObject gameoverPopup;

    [SerializeField]
    private Text finalScoreText;

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
    
    public void PressPause()
    {
        OnPausePressed.Invoke();
    }
    public void PressResume()
    {
        OnResumePressed.Invoke();
    }
    public void PressQuit()
    {
        OnQuitPressed.Invoke();
    }

    public void PressRestart()
    {
        OnRestartPressed.Invoke();
    }

    public void PressHome()
    {
        OnHomePressed.Invoke();
    }
    
}
