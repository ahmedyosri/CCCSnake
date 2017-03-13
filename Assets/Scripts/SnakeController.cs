using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    const string StraightAnimation = "BombStraight";

    const string BirthAnimation = "BombBirth";

    private List<GameObject> snakeTiles;

    private Direction snakeDirection;
    
    private Queue<Direction> registeredDirections;
    
    [SerializeField]
    private GameObject snakeTilePrefab;

    [SerializeField]
    private GameObject snakeTileHeadPrefab;
    
    [SerializeField]
    GameplayInputManager inputManager;

    [SerializeField]
    GameObject explosionPrefab;

    public float snakeSpeed = 1.0f;

    public int snakeDestructionSpeed = 4;

    public float SnakeSpeed { get { return snakeSpeed; } }

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
    
    public void SpeedupSnake(float speedBonus)
    {
        snakeSpeed += Mathf.Abs(speedBonus);
    }

    public void DrawSnakeObjects(List<Tile> snakeGridTiles)
    {
        bool firstTime = (snakeGridTiles.Count == 0);
        bool newTile = (snakeGridTiles.Count != snakeTiles.Count);

        for (int i = 0; i < snakeGridTiles.Count; i++)
        {
            if (snakeTiles.Count < snakeGridTiles.Count)
            {
                if (snakeTiles.Count == 0)
                {
                    snakeTiles.Add(Instantiate(snakeTileHeadPrefab, transform));
                }
                else
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

            // The defaults is to play the Straight Animation
            animatorComp.Play(firstTime ? BirthAnimation : StraightAnimation, 0, 0);
        }

        // Except for a newly added snake tile, override it with the Birth Animation
        if (newTile)
        {
            snakeTiles[snakeGridTiles.Count - 1].GetComponent<Animator>().Play(BirthAnimation, 0, 0);
        }
        
    }

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
