using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents the 2D state of the game, as well as updating the 2D-game's state (advances it by one step) by each call to UpdateBoard
/// </summary>
public class PlaygroundController : BasicStateMachine
{    
    /// <summary>
    /// The 2D array that represents the state of the game, where each item in that 2D Array is a <see cref="TileType"/> 
    /// </summary>
    private List<List<TileType>> gameGrid;
    
    /// <summary>
    /// The 2D position of the Fruit (pickup) in the current state
    /// </summary>
    public Tile pickupPosition { get; private set; }
    
    /// <summary>
    /// The 2D Positions of the snake in the current state
    /// </summary>
    public List<Tile> snakeGridTiles { get; private set; }

    /// <summary>
    /// The 2D Positions of the Obstacles in the the current state
    /// </summary>
    public List<Tile> obstaclesGridTiles { get; private set; }

    // Action fired as the snake head overlaps a Fruit (pickup) tile
    public Action OnItemPickedup;

    // Action fired as the snake crashes with an obstacle/boarder/itself
    public Action OnSnakeCrashed;
    
    /// <summary>
    /// Constructs the 2D array that will represent the level
    /// </summary>
    /// <param name="loadedLevel">Loaded level 2D Array produced by <see cref="LevelLoader"/></param>
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
    
    /// <summary>
    /// Called during the beginning of the game, adds snake tiles to gameGrid (the 2D representation of the current state) placing the snake's head in the middle of the grid
    /// </summary>
    /// <param name="snakeDirection">The initial snake direction</param>
    /// <param name="initialSnakeLength">The initial snake length</param>
    public void AddSnake(Direction snakeDirection, int initialSnakeLength)
    {
        // Calculate the position of the head
        Tile snakePosition = new Tile(gameGrid[0].Count / 2, gameGrid.Count/2, Direction.Right); //Dummy direction

        snakeGridTiles = new List<Tile>();
        
        // Calculate the direction of which snake should be extended, like if the snake heading right
        // tiles should be [mid,mid] [mid,mid-1] [mid, mid-2] ... etc
        var snakeGridTileGenerationDirection = Utils.GetDirectionVector(snakeDirection);
        snakeGridTileGenerationDirection.x *= -1;

        for(int i=0; i<initialSnakeLength; i++)
        {
            var snakeGridTilePosition = snakePosition.tilePosition + snakeGridTileGenerationDirection * i;
            gameGrid[(int)snakeGridTilePosition.z][(int)snakeGridTilePosition.x] = TileType.Snake;
            snakeGridTiles.Add(new Tile(snakeGridTilePosition, snakeDirection));
        }
        
    }

    /// <summary>
    /// Advances the state of the game by one step
    /// </summary>
    /// <param name="snakeDirection">The new direction of the snake</param>
    public void UpdateBoard(Direction snakeDirection)
    {
        // 1- Empty the tail tile and saves it's position
        Tile tailTile = null;
        if(snakeGridTiles.Count > 0)
        {
            tailTile = new Tile(snakeGridTiles[snakeGridTiles.Count - 1].tilePosition, snakeGridTiles[snakeGridTiles.Count - 1].tileDirection);
            gameGrid[(int)tailTile.tilePosition.z][(int)tailTile.tilePosition.x] = TileType.Empty;
        }

        // 2- Advance each tile to take the position of the previous tile
        for(int i=snakeGridTiles.Count-1; i>0; i--)
        {
            snakeGridTiles[i].tilePosition = snakeGridTiles[i - 1].tilePosition;
            snakeGridTiles[i].tileDirection = snakeGridTiles[i - 1].tileDirection;
        }

        // 3- Change the snake head direction
        snakeGridTiles[0].tileDirection = snakeDirection;

        // 4- Calculate the new snake head position
        var newHeadPosition = Utils.GetDirectionVector(snakeGridTiles[0].tileDirection);
        newHeadPosition.z *= -1;
        newHeadPosition += snakeGridTiles[0].tilePosition;

        snakeGridTiles[0].tilePosition = newHeadPosition;


        // 5- Check if the snake carshed with itself/obstacle or a boarder to fire OnSnakeCrashed Action
        if (newHeadPosition.z >= gameGrid.Count || newHeadPosition.x >= gameGrid[0].Count ||
            newHeadPosition.z < 0 || newHeadPosition.x < 0 ||
            gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] == TileType.Obstacle ||
            gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] == TileType.Snake)
        {
            OnSnakeCrashed.Invoke();
            return;
        }

        // 6- Check if the new snake head position overlaps the Fruit (pickup)
        bool pickingUp = gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] == TileType.Pickup;

        // 7- Update that tile type to Snake
        gameGrid[(int)newHeadPosition.z][(int)newHeadPosition.x] = TileType.Snake;

        // 8- Add new snake tile at the saved last tail position
        if (pickingUp && tailTile != null)
        {
            gameGrid[(int)tailTile.tilePosition.z][(int)tailTile.tilePosition.x] = TileType.Snake;
            snakeGridTiles.Add(tailTile);

            // 9- Regenerate a new position for the pickup
            GeneratePickup();

            // 10- Fire the OnItemPickedup Action
            OnItemPickedup.Invoke();
        }

    }

    /// <summary>
    /// Changes the position of the Fruit(pickup) and updates the gamegrid with the new position
    /// </summary>
    private void GeneratePickup()
    {
        pickupPosition = GetRandomEmptyTile();
        gameGrid[(int)pickupPosition.tilePosition.z][(int)pickupPosition.tilePosition.x] = TileType.Pickup;
    }

    /// <summary>
    /// Scans the grid for empty tiles and returns a random one
    /// </summary>
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

/// <summary>
/// Represents a tile unit in the game (A simple transform that consists of position and direction)
/// </summary>
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

/// <summary>
/// Some utililty functions used among the game
/// </summary>
public class Utils
{
    /// <summary>
    /// Gets the 3D in-game world vector that represents a give direction
    /// </summary>
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

    /// <summary>
    /// Gets the quaternion representing a given direction
    /// </summary>
    /// <param name="forDirection"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Converts the playerScore from int to 0000 formatted string
    /// </summary>
    /// <returns></returns>
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