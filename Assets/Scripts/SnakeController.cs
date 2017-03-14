using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: the key to having a smooth walking snake is the Straight Animation loop, 
// which moves the (Bomb object) from last tile position to the new tile position in the same amount of time needed between two UpdateBoard calls
// Also Birth Animation is played by newly added tiles

/// <summary>
/// Controls the snake 3D Object running in the world
/// </summary>
public class SnakeController : MonoBehaviour
{
    /// <summary>
    /// Animation that moves the (Bomb Object) a unit during the time unit
    /// </summary>
    const string StraightAnimation = "BombStraight";

    /// <summary>
    /// Animation that give a birth from ground look to the (Bomb Object) during the time unit
    /// </summary>
    const string BirthAnimation = "BombBirth";

    /// <summary>
    /// List of gameobjects where each one represents a SnakeTile Gameobject
    /// </summary>
    private List<GameObject> snakeTiles;

    /// <summary>
    /// The current Direction of the Snake
    /// </summary>
    private Direction snakeDirection;
    
    /// <summary>
    /// The stored directions from player, in case he quickly pressed two very quick sequel directions
    /// </summary>
    private Queue<Direction> registeredDirections;
    
    /// <summary>
    /// Reference to prefab constructing a snake tile
    /// </summary>
    [SerializeField]
    private GameObject snakeTilePrefab;

    /// <summary>
    /// Reference to the Snake First tile Prefab
    /// </summary>
    [SerializeField]
    private GameObject snakeTileHeadPrefab;
    
    /// <summary>
    /// Reference to the InputManager
    /// </summary>
    [SerializeField]
    GameplayInputManager inputManager;

    /// <summary>
    /// Reference to the ExplosionPrefab
    /// </summary>
    [SerializeField]
    GameObject explosionPrefab;

    /// <summary>
    /// The current snake speed
    /// </summary>
    private float snakeSpeed = 1.0f;

    /// <summary>
    /// The current snake speed
    /// </summary>
    public float SnakeSpeed { get { return snakeSpeed; } }

    /// <summary>
    /// While playing the death animation, how many tile should be destroyed within a second
    /// </summary>
    public int snakeDestructionSpeed = 4;

    /// <summary>
    /// Action fires when Snake destroying animation is completed
    /// </summary>
    public Action OnSnakeDestroyed;


    // Use this for initialization
    void Start ()
    {
        snakeTiles = new List<GameObject>();

        inputManager.OnMoveRight += MoveRight;

        inputManager.OnMoveDown += MoveDown;

        inputManager.OnMoveLeft += MoveLeft;

        inputManager.OnMoveUp += MoveUp;
    }
    
    /// <summary>
    /// Initializes the snake, currently it initializes the snake direction only
    /// </summary>
    /// <param name="initialSnakeDirection"></param>
    public void InitSnake(Direction initialSnakeDirection)
    {
        snakeDirection = initialSnakeDirection;
    }

    void MoveRight()
    {
        ChangeDirectionTo(Direction.Right);
    }

    void MoveLeft()
    {
        ChangeDirectionTo(Direction.Left);
    }

    void MoveUp()
    {
        ChangeDirectionTo(Direction.Up);
    }

    void MoveDown()
    {
        ChangeDirectionTo(Direction.Down);
    }

    /// <summary>
    /// Registers newDirection to the queue of registeredDirections, also it filters wrong Directions (i.e you cannot go in the opposite direction)
    /// </summary>
    void ChangeDirectionTo(Direction newDirection)
    {
        if (registeredDirections == null)
        {
            registeredDirections = new Queue<Direction>();
        }

        var lastRegisteredDirection = registeredDirections.Count > 0 ? registeredDirections.Peek() : snakeDirection;

        switch (newDirection)
        {
            case Direction.Right:
                if (snakeDirection == Direction.Left || lastRegisteredDirection == Direction.Left)
                {
                    return;
                }
                break;
            case Direction.Down:
                if (snakeDirection == Direction.Up || lastRegisteredDirection == Direction.Up)
                {
                    return;
                }
                break;
            case Direction.Left:
                if (snakeDirection == Direction.Right || lastRegisteredDirection == Direction.Right)
                {
                    return;
                }
                break;
            case Direction.Up:
                if (snakeDirection == Direction.Down || lastRegisteredDirection == Direction.Down)
                {
                    return;
                }
                break;
        }

        registeredDirections.Enqueue(newDirection);
    }

    /// <summary>
    /// Fetches the next registered direction
    /// </summary>
    /// <returns></returns>
    public Direction GrabNextDirection()
    {
        if (registeredDirections != null && registeredDirections.Count > 0)
        {
            snakeDirection = registeredDirections.Peek();
            return registeredDirections.Dequeue();
        }
        else
        {
            return snakeDirection;
        }
    }
    
    /// <summary>
    /// Speeds up the snake by speedBonus
    /// </summary>
    public void SpeedupSnake(float speedBonus)
    {
        snakeSpeed += Mathf.Abs(speedBonus);
    }

    /// <summary>
    /// Receives a List of tiles from GameplayManager to draw the 3D Snake based on it,
    /// It adds new SnakeTiles upon need (whether snake has eaten a fruit or at the beginning of a new game)
    /// Also, it Handles the animation of each tile based on the context
    /// </summary>
    /// <param name="snakeGridTiles"></param>
    public void DrawSnakeObjects(List<Tile> snakeGridTiles)
    {
        bool firstTime = (snakeGridTiles.Count == 0);
        bool snakeExtended = (snakeGridTiles.Count != snakeTiles.Count);

        for (int i = 0; i < snakeGridTiles.Count; i++)
        {
            if (snakeTiles.Count < snakeGridTiles.Count)
            {
                // Add snakehead prefab if it is the first tile ever
                if (snakeTiles.Count == 0)
                {
                    snakeTiles.Add(Instantiate(snakeTileHeadPrefab, transform));
                }
                else // Add regular snake tile
                {
                    snakeTiles.Add(Instantiate(snakeTilePrefab, transform));
                }
                snakeTiles[i].GetComponent<Animator>().speed = snakeSpeed;
            }

            var snakeTile3DPosition = Vector3.right * snakeGridTiles[i].tilePosition.x + Vector3.back * snakeGridTiles[i].tilePosition.z;
            snakeTiles[i].transform.localPosition = snakeTile3DPosition;
            snakeTiles[i].transform.rotation = Utils.GetDirectionRotation(snakeGridTiles[i].tileDirection);


            var animatorComp = snakeTiles[i].GetComponent<Animator>();
            animatorComp.speed = snakeSpeed;

            // Play Birth animation for all tiles if the first time, otherwise play Straight Animation
            animatorComp.Play(firstTime ? BirthAnimation : StraightAnimation, 0, 0);
        }

        // Except for a newly added snake tail tile, Run Birth Animation
        if (snakeExtended)
        {
            snakeTiles[snakeGridTiles.Count - 1].GetComponent<Animator>().Play(BirthAnimation, 0, 0);
        }
        
    }

    /// <summary>
    /// Starts the death animation for the snake
    /// </summary>
    public void DestroySnake()
    {
        StartCoroutine(DestroySnakeCoroutine());
    }

    private IEnumerator DestroySnakeCoroutine()
    {
        for(int i=0; i<snakeTiles.Count; i++)
        {
            Instantiate(explosionPrefab, snakeTiles[i].transform.position, Quaternion.identity);
            Destroy(snakeTiles[i].gameObject);
            yield return new WaitForSeconds(1.0f/snakeDestructionSpeed);
        }
        OnSnakeDestroyed.Invoke();
    }
    
}
