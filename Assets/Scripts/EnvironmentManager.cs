using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles 3D World construction for a given level file
/// </summary>
public class EnvironmentManager : MonoBehaviour
{    
    /// <summary>
    /// The ratio between a level's width to a good top-down camera height
    /// </summary>
    const float LevelWidthToCameraHeightRatio = 0.85f;

    /// <summary>
    /// The ratio between a level's length to a good top-down camera height
    /// </summary>
    const float LevelLengthToCameraHeightRatio = 1.40f;

    /// <summary>
    /// How far the far topdown clip plane should be, based on it's height
    /// </summary>
    const float cameraFarClipToHeightRatio = 1.1f;

    /// <summary>
    /// The ratio between background of the level to the level Width/Length
    /// </summary>
    const float backgroundSizeToLevelWidthRatio = 0.1f;

    /// <summary>
    /// The loaded level width
    /// </summary>
    private int levelWidth;

    /// <summary>
    /// The loaded level length
    /// </summary>
    private int levelLength;

    /// <summary>
    /// The background (floor) of the 3D world
    /// </summary>
    [SerializeField]
    private GameObject playgroundBackground;

    /// <summary>
    /// The top-down main camera of the Main scene
    /// </summary>
    [SerializeField]
    private Camera topdownCamera;

    /// <summary>
    /// The Transform component of the empty gameobject should enlist all generated boarder tiles gameobjects as its children
    /// </summary>
    [SerializeField]
    private Transform boardersParent;

    /// <summary>
    /// Prefab to represent a boarder tile
    /// </summary>
    [SerializeField]
    private GameObject boarderTilePrefab;

    /// <summary>
    /// Parent of the live objects running across the scene, used as a pivot so we can comfortably place objects using TilePositions
    /// </summary>
    [SerializeField]
    private Transform playgroundObjectsHolder;

    /// <summary>
    /// The transform component of the empty gameobject should enlist all generated obstacle tiles gameobjects as its children
    /// </summary>
    [SerializeField]
    private Transform obstaclesParent;

    /// <summary>
    /// Prefab to represent a obstable tile
    /// </summary>
    [SerializeField]
    private GameObject obstaclePrefab;

    /// <summary>
    /// Creates and update the environment, the camera, the boarders and the obstacles
    /// </summary>
    /// <param name="obstaclesGridTiles">List of tiles, where each tile is the place of an obstace in the 2D representaion of the game, originally created by <see cref="PlaygroundController"/></param>
    public void UpdateEnvironment(int newLevelWidth, int newLevelLength, List<Tile> obstaclesGridTiles)
    {
        levelWidth = newLevelWidth;
        levelLength = newLevelLength;
        
        UpdateCamera();

        UpdatePlaygroundBackground();

        CreateBoarders();

        CreateObstacles(obstaclesGridTiles);

        var newPivotPosition = transform.position + Vector3.left * levelWidth * 0.5f + Vector3.forward * levelLength * 0.5f;
        playgroundObjectsHolder.transform.position = new Vector3(newPivotPosition.x, playgroundObjectsHolder.transform.position.y, newPivotPosition.z);
    }

    /// <summary>
    /// Creates 3D obstacles in the corresponding places
    /// </summary>
    /// <param name="obstaclesGridTiles">List of tiles, where each tile is the place of an obstace in the 2D representaion of the game, originally created by <see cref="PlaygroundController"/></param>
    private void CreateObstacles(List<Tile> obstaclesGridTiles)
    {
        for(int i=0; i<obstaclesGridTiles.Count; i++)
        {
            var XPos = (int) obstaclesGridTiles[i].tilePosition.x;
            var YPos = (int)obstaclesGridTiles[i].tilePosition.z;
            var obstacle = Instantiate(obstaclePrefab, Vector3.right * XPos + Vector3.back * YPos, Quaternion.identity, obstaclesParent);
            obstacle.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0, 360));
        }
    }

    /// <summary>
    /// Adjusts the camera height based on the level data
    /// </summary>
    private void UpdateCamera()
    {
        var newPosition = topdownCamera.transform.position;
        if(levelWidth > levelLength)
        {
            newPosition = new Vector3(newPosition.x, levelWidth * LevelWidthToCameraHeightRatio, newPosition.z);
        }
        else
        {
            newPosition = new Vector3(newPosition.x, levelLength * LevelLengthToCameraHeightRatio, newPosition.z);
        }
        topdownCamera.transform.position = newPosition;
        topdownCamera.GetComponent<Camera>().farClipPlane = newPosition.y * cameraFarClipToHeightRatio;
    }
    
    /// <summary>
    /// Adjusts the Width/Length of the 3D Floor of the world
    /// </summary>
    private void UpdatePlaygroundBackground()
    {
        var newPlaygroundScale = new Vector3(levelWidth *backgroundSizeToLevelWidthRatio, playgroundBackground.transform.localScale.y, levelLength * backgroundSizeToLevelWidthRatio);
        playgroundBackground.GetComponent<Transform>().localScale = newPlaygroundScale;

        var backgroundMaterial = playgroundBackground.GetComponent<Renderer>().material;
        var currTextureOffset = backgroundMaterial.GetTextureScale("_MainTex");
        playgroundBackground.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(currTextureOffset.x * newPlaygroundScale.x, currTextureOffset.y * newPlaygroundScale.z));
    }

    /// <summary>
    /// Creates boarder tiles
    /// </summary>
    private void CreateBoarders()
    {
        float YPos = boardersParent.transform.position.y;
        boardersParent.transform.position = Vector3.left * (levelWidth / 2 + 1) + Vector3.forward * (levelLength / 2 + 1) + Vector3.up * YPos;

        //Create upper/bottom boarders
        for(int i=0; i<levelWidth+2; i++)
        {
            var boarderTile = Instantiate(boarderTilePrefab, boardersParent);
            boarderTile.transform.localPosition = Vector3.right * i;

            boarderTile = Instantiate(boarderTilePrefab, boardersParent);
            boarderTile.transform.localPosition = Vector3.right * i + Vector3.back * (levelLength + 1);
        }

        //Create left/right boarders
        for(int i=1; i<levelLength+1; i++)
        {
            var boarderTile = Instantiate(boarderTilePrefab, boardersParent);
            boarderTile.transform.localPosition = Vector3.back * i;

            boarderTile = Instantiate(boarderTilePrefab, boardersParent);
            boarderTile.transform.localPosition = Vector3.back * i + Vector3.right * (levelWidth + 1);
        }


    }
}
