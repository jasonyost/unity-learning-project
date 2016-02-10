using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    private BoardManager boardManager;
    private int level = 3;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
				{
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        boardManager = GetComponent<BoardManager>();

        InitGame();
    }

    void InitGame()
		{
			boardManager.SetupScene(level);
    }

}
