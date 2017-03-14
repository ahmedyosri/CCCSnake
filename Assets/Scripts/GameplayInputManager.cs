using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles fetching input from keyboard and touch-enabled mobiles
/// </summary>
public class GameplayInputManager : MonoBehaviour
{
    /// <summary>
    /// Fired when the player press RightArrow, D-key or when right swipe is detected
    /// </summary>
    public Action OnMoveRight;

    /// <summary>
    /// Fired when the player press LeftArrow, A-key or when left swipe is detected
    /// </summary>
    public Action OnMoveLeft;

    /// <summary>
    /// Fired when the player press UpArrow, W-key or when up swipe is detected
    /// </summary>
    public Action OnMoveUp;

    /// <summary>
    /// Fired when the player press DownArrow, S-key or when down swipe is detected
    /// </summary>
    public Action OnMoveDown;

    /// <summary>
    /// When a swipe is detected, it becomes either Vector2.right, Vector2.left, Vector2.up or Vector2.down based on the swipe
    /// </summary>
    private Vector2 detectedSwipe;

    /// <summary>
    /// Records the main touch (GetTouch(0)) starting position
    /// </summary>
    private Vector2 touchStartPosition;
    
	// Update is called once per frame
	void Update ()
    {
        if (GameProperties.isGamePaused)
        {
            return;
        }

        FetchSwipe();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || detectedSwipe == Vector2.right)
        {
            OnMoveRight.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || detectedSwipe == Vector2.left)
        {
            OnMoveLeft.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || detectedSwipe == Vector2.up)
        {
            OnMoveUp.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || detectedSwipe == Vector2.down)
        {
            OnMoveDown.Invoke();
        }

        detectedSwipe = Vector2.zero;

    }

    /// <summary>
    /// Detects a swipe and modify detectedSwipe vector to represent the detected swipe
    /// </summary>
    private void FetchSwipe()
    {
        // No touches detected, return
        if(Input.touchCount < 1)
        {
            detectedSwipe = Vector2.zero;
            touchStartPosition = Vector2.zero;
            return;
        }

        var mainTouch = Input.GetTouch(0);
        
        // Touch is just starting, record the start position then return
        if(touchStartPosition == Vector2.zero)
        {
            touchStartPosition = mainTouch.position;
            return;
        }

        // Touch is still on the screen, return
        if(mainTouch.phase != TouchPhase.Ended)
        {
            return;
        }

        // Touch has finally ended, calculate the swipeVector (the vector going from the start position to the final position)
        var swipeVector = mainTouch.position - touchStartPosition;

        // Based on the swipeVector value, assign a corresponding vector to detectedSwipe
        if(Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            detectedSwipe = swipeVector.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            detectedSwipe = swipeVector.y > 0 ? Vector2.up : Vector2.down;
        }

    }
}
