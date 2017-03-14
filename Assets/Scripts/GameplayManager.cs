using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * The reason why I am referencing the pickup object directly here is that it would be replaced later on by a PickupsManager, which should handles the spawning of different pickups acress the gameplay
 */

/// <summary>
/// Manages the whole gameplay session through Main scene
/// </summary>
public class GameplayManager : BasicStateMachine {

    /// <summary>
    /// Loads and parses the level file
    /// </summary>
    private LevelLoader levelLoader;

    /// <summary>
    /// Reference to the environment manager which constructs the 3D World
    /// </summary>
    [SerializeField]
    private EnvironmentManager environmentManager;
    
    /// <summary>
    /// Reference to the playgroundController which represnets the current state of the Game in 2D 
    /// </summary>
    [SerializeField]
    private PlaygroundController playgroundController;

    /// <summary>
    /// Reference to the SnakeController, which draws the snake 3D Objects and listens to the input
    /// </summary>
    [SerializeField]
    private SnakeController snakeController;
    
    /// <summary>
    /// Reference to the Fruit pickup-able object in the world
    /// </summary>
    [SerializeField]
    private Pickup pickupObject;

    /// <summary>
    /// Particle System should play where the snake picks up a fruit
    /// </summary>
    [SerializeField]
    private GameObject itemPickedupPSPrefab;

    /// <summary>
    /// Reference to the UI manager of the scene
    /// </summary>
    [SerializeField]
    private UIManager uiManager;

    /// <summary>
    /// Reference to the AudioManager of the scene
    /// </summary>
    [SerializeField]
    private AudioManager audioManager;
    
	// Use this for initialization
	void Start ()
    {
        levelLoader = new LevelLoader();

        levelLoader.LoadLevel(GameProperties.levelFilePath);
        

        playgroundController.ConstructLevel(levelLoader.LoadedLevel, levelLoader.LevelLength, levelLoader.LevelWidth);

        environmentManager.UpdateEnvironment(levelLoader.LevelWidth, levelLoader.LevelLength, playgroundController.obstaclesGridTiles);


        var randomSnakeDirection = Utils.GenerateRandomDirection();

        playgroundController.AddSnake(randomSnakeDirection, GameProperties.initialSnakeLength);

        snakeController.InitSnake(randomSnakeDirection);


        SetupListeners();

        GameProperties.playerScore = 0;

        OnGameResumed();
	}

    void SetupListeners()
    {
        uiManager.OnHomePressed += OnGoHome;
        uiManager.OnPausePressed += OnGamePaused;
        uiManager.OnQuitPressed += OnGameQuit;
        uiManager.OnRestartPressed += OnGameRestart;
        uiManager.OnResumePressed += OnGameResumed;
        
        playgroundController.OnItemPickedup += OnItemPickedup;
        playgroundController.OnSnakeCrashed += OnSnakeCrashed;
    }

	// Update is called once per frame
	void Update ()
    {
        switch (m_State)
        {
            case 0: // Game paused
                break;

            case 1: // Game running states
                float updateInterval = 1 / snakeController.SnakeSpeed;
                m_Timer = Time.time + updateInterval;
                m_State++;
                break;

            case 2:
                if(Time.time < m_Timer)
                {
                    break;
                }
                m_State = 1;

                playgroundController.UpdateBoard(snakeController.GrabNextDirection());
                snakeController.DrawSnakeObjects(playgroundController.snakeGridTiles);

                var pickupPosition = playgroundController.pickupPosition.tilePosition;
                pickupObject.transform.localPosition = Vector3.right * pickupPosition.x + Vector3.back * pickupPosition.z;

                break;
        }
	}
    
    void OnGamePaused()
    {
        m_State = 0;

        GameProperties.isGamePaused = true;
        audioManager.SetVolume(Track.BackgroundMusic, 0.2f);
        audioManager.Stop(Track.Roll);
    }

    void OnGameResumed()
    {
        m_State = 1;

        GameProperties.isGamePaused = false;
        audioManager.SetVolume(Track.BackgroundMusic, 1.0f);
        audioManager.Play(Track.Roll);

    }

    void OnGoHome()
    {
        SceneManager.LoadScene(0);
    }

    void OnGameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGameQuit()
    {
        Application.Quit();
    }

    void OnSnakeCrashed()
    {
        m_State = 0;
        snakeController.OnSnakeDestroyed += OnGameOver;
        snakeController.DestroySnake();
        audioManager.Stop(Track.BackgroundMusic);
        audioManager.Stop(Track.Roll);
    }

    void OnGameOver()
    {
        uiManager.GameoverPopupVisible = true;
    }

    void OnItemPickedup()
    {
        Instantiate(itemPickedupPSPrefab, pickupObject.transform.position, Quaternion.identity);

        if (pickupObject.OnItemPickedup != null)
        {
            pickupObject.OnItemPickedup.Invoke();
        }

        GameProperties.playerScore += pickupObject.itemScore * GameProperties.gameLevel;
        snakeController.SpeedupSnake(pickupObject.speedBonus * GameProperties.gameLevel);
        uiManager.Score = Utils.GetScoreString();
        audioManager.Play(Track.Pickup, true);
    }
}
