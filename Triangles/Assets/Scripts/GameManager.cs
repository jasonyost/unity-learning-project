using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    // time between level load
    public float levelStartDelay = 2f;
    // time between turn delay
    public float turnDelay = .1f;
    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
    public BoardManager boardManager; //Store a reference to our BoardManager which will set up the level.
    // Starting energy, when depleted, game over
    public int energyPoints = 100;
    [HideInInspector] // hide this in the editor
    public bool playersTurn = true;
    [HideInInspector] // hide this in the editor
    public bool enemyTurn;
    public bool isGameOver = false;
    public AudioClip gameOverSound; // reference to the game over sound

    private Text levelTitle;
    private GameObject levelCard;
    // TODO implment reset button
    // private GameObject resetButton;

    private int level = 1;
    private List<Enemy> enemies = new List<Enemy>();
    private bool doingSetup;

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

    private void OnLevelWasLoaded(int index)
    {
        if (!isGameOver)
        {
            level++;
        }
        InitGame();
    }

    [ContextMenu("Game Over")]
    public void GameOver()
    {
        SoundManager.instance.musicSource.Stop();
        SoundManager.instance.PlaySingle(gameOverSound);
        levelTitle.text = "After " + level + " levels, you ran out of energy.";
        // resetButton.SetActive(true);
        levelCard.SetActive(true);
        isGameOver = true;
    }

    public void ResetGame()
    {
        instance.level = 1;
        // SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene("Main");
    }

    void InitGame()
    {
        // doingSetup = true;
        isGameOver = false;
        levelCard = GameObject.Find("LevelCard");
        levelTitle = GameObject.Find("LevelTitle").GetComponent<Text>();
        // resetButton = GameObject.Find("ResetButton");

        levelCard.SetActive(true);
        levelTitle.text = "Level " + level;

        // resetButton.SetActive(false);

        Invoke("HideLevelCard", levelStartDelay);

        enemies.Clear();
        boardManager.SetupScene(level);
    }

    void HideLevelCard()
    {
        levelTitle.text = "";
        levelCard.SetActive(false);
        doingSetup = false;
    }

    void Update()
    {
        if (playersTurn || enemyTurn || doingSetup || isGameOver)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    IEnumerator MoveEnemies()
    {
        enemyTurn = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemyTurn = false;
    }

}
