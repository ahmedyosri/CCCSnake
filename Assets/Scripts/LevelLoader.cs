using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Loads and parses Level files
/// </summary>
public class LevelLoader : MonoBehaviour
{
    /// <summary>
    /// 2D int array that contains the parsed level
    /// </summary>
    private List<List<int>> loadedLevel;

    /// <summary>
    /// The loaded level length
    /// </summary>
    public int LevelLength
    {
        get
        {
            return loadedLevel == null ? 0 : loadedLevel.Count;
        }
    }

    /// <summary>
    /// The loaded level width
    /// </summary>
    public int LevelWidth
    {
        get
        {
            return loadedLevel == null ? 0 : loadedLevel[0].Count;
        }
    }
    
    /// <summary>
    /// The loaded level data
    /// </summary>
    public List<List<int>> LoadedLevel
    {
        get
        {
            return loadedLevel;
        }
    }
    
    /// <summary>
    /// Loads and parses a level file from the resources, if failed, it loads a default map
    /// </summary>
    /// <param name="levelFilePath">Path of level file in Resources folder</param>
    /// <returns>The loaded file data, can also be accessed from <see cref="LevelLoader.LoadedLevel"/> </returns>
    public List<List<int>> LoadLevel(string levelFilePath)
    {
        if (!TryLoadFromTextFile(levelFilePath))
        {
            print("Failed to load level file, loading default map");

            loadedLevel = new List<List<int>> {
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            new List<int> { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
        };

        }

        return loadedLevel;
    }

    /// <summary>
    /// Tries to load Level file from resources/FilePath, if the file didn't fit the level file pattern, it fail and returns false
    /// </summary>
    /// <param name="FilePath">Level file path to be loaded, file should be in Resrouces</param>
    /// <returns>True if loading and parsing file successeded, false otherwise.</returns>
    bool TryLoadFromTextFile(string FilePath)
    {
        var levelFileData = Resources.Load(GameProperties.levelFilePath) as TextAsset;

        if(levelFileData == null)
        {
            print(string.Format("Level file not found : {0}", GameProperties.levelFilePath));
            return false;
        }

        string[] LevelTokens = levelFileData.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); ;

        if (LevelTokens == null || LevelTokens.Length < 2)
        {
            print(string.Format("Error parsing level file : {0}", GameProperties.levelFilePath));
            return false;
        }

        int levelLength, levelWidth;

        levelLength = levelWidth = -1;

        int.TryParse(LevelTokens[0], out levelLength);
        int.TryParse(LevelTokens[1], out levelWidth);

        if (levelLength == -1 || levelWidth == -1)
        {
            print("Invalid length/width");
            return false;
        }

        if(LevelTokens.Length < 12)
        {
            print("Level file has less than 10 rows");
            return false;
        }

        if(LevelTokens.Length - 2 != levelLength)
        {
            print("Level data don't match Level Length");
            return false;
        }


        loadedLevel = new List<List<int>>();

        for (int i=2; i<LevelTokens.Length; i++)
        {
            var rowValues = LevelTokens[i].Split(' ');
            if(rowValues.Length != levelWidth)
            {
                print("Level data don't match Level Width");
                return false;
            }

            loadedLevel.Add(new List<int>(levelWidth));
            for (int j = 0; j < rowValues.Length; j++)
            {
                int tileValue = -1;
                int.TryParse(rowValues[j], out tileValue);

                if(tileValue < 0)
                {
                    print(string.Format("Invalid tile value = {0} at {1},{2}.", tileValue, i, j));
                    return false;
                }
                loadedLevel[i-2].Add(tileValue);
            }
        }

        return true;
    }
}
