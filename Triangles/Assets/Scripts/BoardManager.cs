using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [System.Serializable]
    public class Count
    {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.


        //Assignment constructor.
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count obstacleCount = new Count(5, 9);
    public Count energyCount = new Count(1, 5);
    public GameObject exit;
    public GameObject groundTile;
    public GameObject obstacleTile;
    public GameObject wallTile;
    public GameObject[] enemyTiles;
    public GameObject[] energyTiles;

    [Range(0, 1)]
    public float outlinePercent;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitGridList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, 0.34f, y));
            }
        }
    }

    void BuildBoard()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {

                GameObject mapObject;

                // Set the position for the tile/object
                Vector3 tilePosition = new Vector3(x, 0, y);

                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    // This is a wall
                    mapObject = wallTile;
                }
                else {
                    mapObject = groundTile;
										mapObject.transform.localScale = Vector3.one * (1-outlinePercent);
                }

                GameObject instance = Instantiate(mapObject, tilePosition, Quaternion.Euler(Vector3.right * 90)) as GameObject;

                instance.transform.SetParent(boardHolder);

            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject tile, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Instantiate(tile, randomPosition, Quaternion.identity);
        }
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum){
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++) {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BuildBoard();
        InitGridList();

        LayoutObjectAtRandom(obstacleTile, obstacleCount.minimum, obstacleCount.maximum);
        LayoutObjectAtRandom(energyTiles, energyCount.minimum, energyCount.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        Instantiate(exit, new Vector3(columns - 1, 0.01f, rows - 1), Quaternion.Euler(Vector3.right * 90));
    }

}
