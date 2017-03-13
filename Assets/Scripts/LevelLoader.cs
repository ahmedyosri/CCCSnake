using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private List<List<int>> loadedLevel;

    public int LevelLength { get { return loadedLevel.Count; } }
    public int LevelWidth { get { return loadedLevel[0].Count; } }
    
    public List<List<int>> LoadedLevel { get { return loadedLevel; } }
    
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

    bool TryLoadFromTextFile(string FilePath)
    {
        var levelFileData = Resources.Load(GameProperties.levelFilePath) as TextAsset;

        if(levelFileData == null)
        {
            print(string.Format("Level file not found : {0}", GameProperties.levelFilePath));
            return false;
        }

        string[] LevelTokens = levelFileData.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); ;


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
