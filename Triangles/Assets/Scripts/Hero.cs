using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Hero : Mob
{

    public int obstacleDamage = 1;
    public int pointsPerEnergy = 10;
    public float restartLevelDelay = 1f;
    public Text statusText;

    // TODO clean this up
    // public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
    // public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
    // public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
    // public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
    // public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
    // public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
    // public AudioClip gameOverSound;				      //Audio clip to play when player dies.

    // private Animator animator;
    private int energy;


    protected override void Start()
    {
        // animator = GetComponent<Animator>();

        //Get the current food point total stored in GameManager.instance between levels.
        energy = GameManager.instance.energyPoints;
        // statusText.text = "Energy: " + energy;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.energyPoints = energy;
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        int horizontal = 0;  	//Used to store the horizontal move direction.
        int vertical = 0;		//Used to store the vertical move direction.

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            OnMove<Obstacle>(horizontal, vertical);
        }
    }

    protected override void OnMove<T>(int xDir, int yDir)
    {
        energy--;
        // statusText.text = "Energy: " + energy;
        base.OnMove<T>(xDir, yDir);
        RaycastHit hit;

        if (Move(xDir, yDir, out hit))
        {
            // SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        CheckGameOver();
        GameManager.instance.playersTurn = false;
    }

    protected override void Interact<T>(T component)
    {
        Obstacle hitObstacle = component as Obstacle;
        hitObstacle.DamageObstacle(obstacleDamage);
        // animator.SetTrigger("heroStrike");
    }

    private void OnTriggerEnter(Collider item)
    {
        switch (item.tag)
        {
            case "Exit":
                Invoke("Restart", restartLevelDelay);
                enabled = false;
                break;
            case "Energy":
                energy += pointsPerEnergy;
                // statusText.text = "+" + pointsPerEnergy + " Food: " + energy;
                // SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
                item.gameObject.SetActive(false);
                break;
                // case "Soda":
                //     food += pointsPerSoda;
                //     statusText.text = "+" + pointsPerSoda + " Food: " + food;
                //     SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
                //     item.gameObject.SetActive(false);
                //     break;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    public void TakeDamage(int damage)
    {
        // animator.SetTrigger("heroHurt");
        energy -= damage;
        // statusText.text = "-" + damage + " Energy: " + energy;
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (energy <= 0)
        {
            energy = 100;
            enabled = false;
            // SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.GameOver();
        }

    }
}
