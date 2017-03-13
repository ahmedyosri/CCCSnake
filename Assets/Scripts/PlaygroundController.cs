using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaygroundController : BasicStateMachine
{    
    [SerializeField]
    private GameObject ObstaclePrefab;

    [SerializeField]
    private Transform obstaclesParent;
    
    [SerializeField]
    private Transform playgroundObjectsHolder;

    private List<List<TileType>> gameGrid;
    

    public Vector3 pickupPosition { get; private set; }
    
    public List<Tile> snakeGridTiles { get; private set; }

    public List<Tile> obstaclesGridTiles { get; private set; }


    public Action OnItemPickedup;

    public Action OnSnakeCrashed;
    

    public void ConstructLevel(List<List<int>> loadedLevel, int levelLength, int levelWidth)
    {
        gameGrid = new List<List<TileType>>(levelLength);
        for(int i=0; i< loadedLevel.Count; i++)
        {
            gameGrid.Add(new List<TileType>(levelWidth));
            for(int j=0; j< levelWidth; j++)
            {
                gameGrid[i].Add(TileType.Empty);
            }
        }

        obstaclesGridTiles = new List<Tile>();
        for (int i = 0; i < levelLength; i++)
        {
            for (int j = 0; j < levelWidth; j++)
            {
                switch (loadedLevel[i][j])
                {
                    case 0:
                        break;

                    case 1:
                        obstaclesGridTiles.Add(new Tile(j, i, Direction.Right));
                        gameGrid[i][j] = TileType.Obstacle;
                        break;
                }
            }
        }
        
        GeneratePickup();
    }
    
    public void AddSnake(Direction snakeDirection, int initialSnakeLength)
    {
        Tile snakePosition = new Tile(gameGrid[0].Count / 2, gameGrid.Count/2, Direction.Right); //Dummy direction

        snakeGridTiles = new List<Tile>();
        
        var snakeGridTileGenerationDirection = Utils.GetDirectionVector(snakeDirection);
        snakeGridTileGenerationDirection.x *= -1;

        for(int i=0; i<initialSnakeLength; i++)
        {
            var snakeGridTilePosition = snakePosition.tilePosition + snakeGridTileGenerationDirection * i;
            gameGrid[(int)snakeGridTilePosition.z][(int)snakeGridTilePosition.x] = TileType.Snake;
            snakeGridTiles.Add(new Tile(snakeGridTilePosition, snakeDirection));
        }
        
    }

    public void UpdateBoard(Direction snakeDirection)
    {
        Tile tailTile = null;
        if(snakeGridTiles.Count > 0)
        {
            tailTile = new Tile(snakeGridTiles[snakeGridTiles.Count - 1].tilePosition, snakeGridTiles[snakeGridTiles.Count - 1].tileDirection);
            gameGrid[(int)tailTile.tilePosition.z][(int)tailTile.tilePosition.x] = TileType.Empty;
        }

        for(int i=snakeGridTiles.Count-1; i>0; i--)
        {
            snakeGridTiles[i].tilePosition = snakeGridTiles[i - 1].tilePosition;
            snakeGridTiles[i].tileDirection = snakeGridTiles[i - 1].tileDirection;
        }

        snakeGridTiles[0].tileDirection = snakeDirection;

        var newHeadPosition = Utils.GetDirectionVector(snakeGridTiles[0].tileDirection);
        newHeadPosition.z *= -1;
        newHeadPosition += snakeGridTiles[0].tilePosition;

        snakeGridTiles[0].tilePosition = newHeadPosition;



        if (newHeadPosition.z >= gameGrid.Count || newHeadPosition.x >= gameGrid[0].Count ||
            newHeadPosition.z < 0 || newHeadPosition.x < 0 ||
            gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] == TileType.Obstacle ||
            gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] == TileType.Snake)
        {
            OnSnakeCrashed.Invoke();
            return;
        }

        bool pickingUp = gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] == TileType.Pickup;

        gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] = TileType.Snake;

        if (pickingUp && tailTile != null)
        {
            gameGrid[(int)tailTile.tilePosition.z][(int)tailTile.tilePosition.x] = TileType.Snake;
            snakeGridTiles.Add(tailTile);

            GeneratePickup();

            OnItemPickedup.Invoke();
        }

    }

    private void GeneratePickup()
    {
        pickupPosition = GetRandomEmptyTile().tilePosition;
        gameGrid[(int)pickupPosition.z][(int)pickupPosition.x] = TileType.Pickup;
    }

    private Tile GetRandomEmptyTile()
    {
        List<Tile> emptyList = new List<Tile>();
        for(int i=0; i<gameGrid.Count; i++)
        {
            for(int j=0; j<gameGrid[i].Count; j++)
            {
                if(gameGrid[i][j] == TileType.Empty)
                {
                    emptyList.Add(new Tile(j, i, Direction.Right));
                }
            }
        }

        var newPickupTile = emptyList[UnityEngine.Random.Range(0, emptyList.Count)];

        return newPickupTile;

    }
    
}

public class Tile
{
    public Vector3 tilePosition;
    public Direction tileDirection;

    public Tile(int XPos, int YPos, Direction TileDirection)
    {
        tilePosition = new Vector3(XPos, 0, YPos);
        tileDirection = TileDirection;
    }

    public Tile(Vector3 TilePosition, Direction TileDirection)
    {
        tilePosition = TilePosition;
        tileDirection = TileDirection;
    }
}

public class Utils
{
    public static Vector3 GetDirectionVector(Direction forDirection)
    {
        switch (forDirection)
        {
            case Direction.Right:
                return Vector3.right;
            case Direction.Down:
                return Vector3.back;
            case Direction.Left:
                return Vector3.left;
            case Direction.Up:
                return Vector3.forward;
            default:
                return Vector3.zero;
        }
    }

    public static Quaternion GetDirectionRotation(Direction forDirection)
    {
        Quaternion resQuaternion = Quaternion.identity;
        switch (forDirection)
        {
            case Direction.Right:
                resQuaternion.eulerAngles = Vector3.up * 90;
                break;
            case Direction.Down:
                resQuaternion.eulerAngles = Vector3.up * 180;
                break;
            case Direction.Left:
                resQuaternion.eulerAngles = Vector3.up * 270;
                break;
            case Direction.Up:
                resQuaternion.eulerAngles = Vector3.zero;
                break;
        }
        return resQuaternion;
    }

    public static Direction GenerateRandomDirection()
    {
        return (Direction) UnityEngine.Random.Range(0, 4);
    }

    public static string GetScoreString()
    {
        return GameProperties.playerScore.ToString("0000");
    }
}

public enum TileType
{
    Empty,
    Obstacle,
    Snake,
    Pickup
}

public enum Direction
{
    Right,
    Down,
    Left,
    Up
}