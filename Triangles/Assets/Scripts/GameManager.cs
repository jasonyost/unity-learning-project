using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardManager;
    public int energyPoints = 100;
    [HideInInspector]
    public bool playersTurn = true;
    public bool isGameOver = false;

    private Text levelTitle;
    private GameObject levelCard;
    private GameObject resetButton;

    private int level = 1;
    private List<Enemy> enemies = new List<Enemy>();
    private bool enemyTurn;
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
        // SoundManager.instance.musicSource.Stop();
        levelTitle.text = "After " + level + " days, you starved to death.";
        resetButton.SetActive(true);
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
        // levelCard = GameObject.Find("LevelCard");
        // levelTitle = GameObject.Find("LevelTitle").GetComponent<Text>();
        // resetButton = GameObject.Find("ResetButton");

        // levelTitle.text = "Day " + level;
        // levelCard.SetActive(true);
        // resetButton.SetActive(false);

        // Invoke("HideLevelCard", levelStartDelay);

        enemies.Clear();
        boardManager.SetupScene(level);
    }

    void HideLevelCard()
    {
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
