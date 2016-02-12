using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Hero : Mob
{

    public int obstacleDamage = 1;
    public int pointsPerEnergy = 10;
    public int pointsPerPower = 20;
    public float restartLevelDelay = 1f;
    public Text statusText;

    public AudioClip[] moveSounds;
    public AudioClip[] gainEnergySounds;
    public AudioClip resetSound;
    public AudioClip drainSound;
    public AudioClip shieldSound;

    private int energy;
    private bool hasShield = false;

    Material skinMaterial;
    Color originalColour;


    protected override void Start()
    {
        skinMaterial = GetComponent<Renderer>().material;
        originalColour = skinMaterial.color;
        //Get the current food point total stored in GameManager.instance between levels.
        energy = GameManager.instance.energyPoints;
        statusText.text = "Energy: " + energy;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.energyPoints = energy;
    }

    void Update()
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
        statusText.text = "Energy: " + energy;
        base.OnMove<T>(xDir, yDir);
        RaycastHit hit;

        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeWalkSfx(moveSounds);
        }
        CheckGameOver();
        GameManager.instance.playersTurn = false;
    }

    protected override void Interact<T>(T component)
    {
        Obstacle hitObstacle = component as Obstacle;
        hitObstacle.DamageObstacle(obstacleDamage);
        StartCoroutine(Attack(hitObstacle));
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
                statusText.text = "+" + pointsPerEnergy + " Energy: " + energy;
                SoundManager.instance.RandomizeSfx(gainEnergySounds);
                item.gameObject.SetActive(false);
                break;
            case "Power":
                energy += pointsPerPower;
                statusText.text = "+" + pointsPerPower + " Energy: " + energy;
                SoundManager.instance.RandomizeSfx(gainEnergySounds);
                item.gameObject.SetActive(false);
                break;
            case "Reset":
                energy = 100;
                statusText.text = "-Energy Reset-" + " Energy: " + energy;
                SoundManager.instance.RandomizeSfx(resetSound);
                item.gameObject.SetActive(false);
                break;
            case "Drain":
                int drain = energy / 2;
                energy -= drain;
                statusText.text = "-" + drain + " Energy: " + energy;
                SoundManager.instance.RandomizeSfx(drainSound);
                item.gameObject.SetActive(false);
                break;
            case "Shield":
                hasShield = true;
                statusText.color = new Color32(36, 136, 206, 255);
                statusText.text = "Shielded" + " Energy: " + energy;
                SoundManager.instance.RandomizeSfx(shieldSound);
                item.gameObject.SetActive(false);
                break;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    public void TakeDamage(int damage)
    {
        if (hasShield)
        {
            statusText.color = Color.white;
            hasShield = false;
        }
        else
        {
            energy -= damage;
            statusText.text = "-" + damage + " Energy: " + energy;
            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        if (energy <= 0)
        {
            energy = 100;
            enabled = false;
            GameManager.instance.GameOver();
        }

    }

    IEnumerator Attack(Component hitObstacle)
    {
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = hitObstacle.transform.position;

        float attackSpeed = 3;
        float percent = 0;

        GameManager.instance.playersTurn = true;
        skinMaterial.color = Color.red;
        while (percent <= 1)
        {

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
        skinMaterial.color = originalColour;
        GameManager.instance.playersTurn = false;
    }
}
