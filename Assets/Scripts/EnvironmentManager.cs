using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{    
    const float LevelWidthToCameraHeightRatio = 0.85f;

    const float LevelLengthToCameraHeightRatio = 1.40f;

    const float cameraFarClipToHeightRatio = 1.1f;

    const float backgroundSizeToLevelWidthRatio = 0.1f;

    private int levelWidth, levelLength;


    [SerializeField]
    private GameObject playgroundBackground;

    [SerializeField]
    private Camera topdownCamera;

    [SerializeField]
    private Transform boardersParent;

    [SerializeField]
    private GameObject boarderTilePrefab;

    [SerializeField]
    private Transform playgroundObjectsHolder;

    [SerializeField]
    private Transform obstaclesParent;

    [SerializeField]
    private GameObject obstaclePrefab;
    

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
    
    private void UpdatePlaygroundBackground()
    {
        var newPlaygroundScale = new Vector3(levelWidth *backgroundSizeToLevelWidthRatio, playgroundBackground.transform.localScale.y, levelLength * backgroundSizeToLevelWidthRatio);
        playgroundBackground.GetComponent<Transform>().localScale = newPlaygroundScale;

        var backgroundMaterial = playgroundBackground.GetComponent<Renderer>().material;
        var currTextureOffset = backgroundMaterial.GetTextureScale("_MainTex");
        playgroundBackground.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(currTextureOffset.x * newPlaygroundScale.x, currTextureOffset.y * newPlaygroundScale.z));
    }

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
