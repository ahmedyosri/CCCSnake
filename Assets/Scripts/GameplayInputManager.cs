using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayInputManager : MonoBehaviour {

    public Action OnMoveRight;

    public Action OnMoveLeft;

    public Action OnMoveUp;

    public Action OnMoveDown;

    private Vector2 detectedSwipe;
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

    private void FetchSwipe()
    {
        if(Input.touchCount < 1)
        {
            detectedSwipe = Vector2.zero;
            touchStartPosition = Vector2.zero;
            return;
        }

        var mainTouch = Input.GetTouch(0);
        
        if(touchStartPosition == Vector2.zero)
        {
            touchStartPosition = mainTouch.position;
            return;
        }

        if(mainTouch.phase != TouchPhase.Ended)
        {
            return;
        }

        var swipeVector = mainTouch.position - touchStartPosition;

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
