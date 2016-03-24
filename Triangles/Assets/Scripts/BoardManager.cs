using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

// Script to manage setup of the board
public class BoardManager : MonoBehaviour
{
    // A Count object to use with the board manager
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

    // width of the board
    public int columns = 8;
    // height of the board
    public int rows = 8;


    public Count obstacleCount = new Count(5, 9); // min/max number of obstacle prefabs to load into the level used in LayoutObstacleAtRandom
    public Count energyCount = new Count(1, 5); // min/max number of energy powerups to load into the level used in LayoutObjectAtRandom
    public Count floorTileCount = new Count(1, 3); // min/max number of floor tile prefabs to load into the level used in LayoutFloorTileAtRandom
    public GameObject exit; // holder for the exit tile prefab
    // TODO this should not be a separate object as it is a floor tile.
    public GameObject reset; // holder for the reset tile object
    public GameObject groundTile; // holder for the white ground tile used to draw the floor of the board
    // TODO this should support multiple obstacles at some point
    public GameObject obstacleTile; // holder for the obstacle prefab
    public GameObject wallTile; // holder for the wall tile prefab
    public GameObject[] enemyTiles; // holder for enemy prefabs to load
    public GameObject[] energyTiles; // holder for energy power ups to load
    public GameObject[] floorTiles; // holder for the various floor tiles

    [Range(0, 1)]
    public float outlinePercent; // Each tile laid out can have an outline percent for aesthetics

    private Transform boardHolder; // keep the scene organized in the editor
    private List<Vector3> gridPositions = new List<Vector3>(); // holds which positions on the board are open when laying out tiles and enemies

    // Initializes the gridPositions list with size of the board, minus one for the wall on the outer edge
    // Since this is top down the Z axis represents how far away from the ground the object is
    // Upward movement is not possible for this game so all objects will be placed slighly above the floor tiles
    void InitGridList()
    {
        // clear the list of any previous positions
        gridPositions.Clear();

        // iterate the columns
        for (int x = 1; x < columns - 1; x++)
        {
            // for each column iterate the rows
            for (int y = 1; y < rows - 1; y++)
            {
                // add the position, looking at a 3D scene top down
                gridPositions.Add(new Vector3(x, 0.34f, y));
            }
        }
    }

    // This method lays out the ground tiles and places the wall around the edge
    void BuildBoard()
    {
        // instantiate the boardHolder
        boardHolder = new GameObject("Board").transform;
        // interate columns
        for (int x = -1; x < columns + 1; x++)
        {
            // for each column interate the rows
            for (int y = -1; y < rows + 1; y++)
            {
                // set a holder for the type of object
                GameObject mapObject;

                // Set the position for the tile/object
                // The Z axis represents the up/down plane, since this is the ground we set it to 0
                Vector3 tilePosition = new Vector3(x, 0, y);

                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    // This is the edge of the board, place a wall tile
                    mapObject = wallTile;
                }
                else {
                    // This the inner board place a floor tile
                    mapObject = groundTile;
                    // apply the outline percent for aesthetics
                    mapObject.transform.localScale = Vector3.one * (1 - outlinePercent);
                }

                // Instantiate the game object, dealing with a 3D world top down we rotate the tile object 90 degrees on the x axis
                GameObject instance = Instantiate(mapObject, tilePosition, Quaternion.Euler(Vector3.right * 90)) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // Returns a random position from the available gridPositions and removes the returned position from the list
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // Places an obstacle on the board and random
    void LayoutObstacleAtRandom(GameObject obstacleTile, int minimum, int maximum)
    {
        // Pick a random number from the min max range
        int objectCount = Random.Range(minimum, maximum + 1);

        // iterate the object count
        for (int i = 0; i < objectCount; i++)
        {
            // return a random position to place the obstacle at
            Vector3 randomPosition = RandomPosition();
            // Instantiate the prefab and the position, in this case the prefab is a square so no need to rotate it.
            Instantiate(obstacleTile, randomPosition, Quaternion.identity);
        }
    }

    // Places a power up at a random position on the board
    // TODO this should just call the other method, this probably isn't even needed
    void LayoutObjectAtRandom(GameObject tile, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Instantiate(tile, randomPosition, Quaternion.identity);
        }
    }

    // Places a power up at a random position on the board
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // Places a floor tile prefab at a random position on the board
    void LayoutFloorTilesAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
      int objectCount = Random.Range(minimum, maximum + 1);

      for (int i = 0; i < objectCount; i++)
      {
          Vector3 randomPosition = RandomPosition();
          GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
          Instantiate(tileChoice, new Vector3(randomPosition.x, 0.01f, randomPosition.z), Quaternion.Euler(Vector3.right * 90));
      }
    }

    // Places a floor tile prefab at a random position on the board
    // Places a power up at a random position on the board
    // TODO this should just call the other method, this probably isn't even needed
    void LayoutFloorTileAtRandom(GameObject tile, int minimum, int maximum)
    {
      int objectCount = Random.Range(minimum, maximum + 1);

      for (int i = 0; i < objectCount; i++)
      {
          Vector3 randomPosition = RandomPosition();
          Instantiate(tile, new Vector3(randomPosition.x, 0.01f, randomPosition.z), Quaternion.Euler(Vector3.right * 90));
      }
    }

    // Parent method to build the board (floor and walls) and add power ups, enemies and floor tiles
    // floor tiles are interactable
    public void SetupScene(int level)
    {
        // Layout floor and walls
        BuildBoard();

        // Instantiate a list of available positions
        InitGridList();

        // Layout obstacles, power ups and floor tiles
        LayoutObstacleAtRandom(obstacleTile, obstacleCount.minimum, obstacleCount.maximum);
        LayoutObjectAtRandom(energyTiles, energyCount.minimum, energyCount.maximum);
        LayoutFloorTilesAtRandom(floorTiles, floorTileCount.minimum, floorTileCount.maximum);

        // 12.5% chance of a Power reset tile per level
        // TODO probably better way to do this
        int resetChance = Random.Range(0, 7);
        if (resetChance == 0)
        {
          LayoutFloorTileAtRandom(reset, 1, 1);
        }

        // Place enemies on a logarithmic scale
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        // Finally always place the exit tile in the upper right rotated 90 degrees on x so it is flat
        Instantiate(exit, new Vector3(columns - 1, 0.01f, rows - 1), Quaternion.Euler(Vector3.right * 90));
    }

}
