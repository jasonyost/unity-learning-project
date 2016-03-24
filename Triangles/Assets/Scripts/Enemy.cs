using UnityEngine;
using System.Collections;

// Inherits the MOb class. Script to manage the movement and interaction of enemies
public class Enemy : Mob
{
    // Amount of damage an enemy does to the hero on hit, each enemy prefab can have it's own value
    public int heroDamage;

    // A transform to keep track of the player position
    private Transform target;
    // Enemies only move once every other turn
    // TODO the skipping of turns and number of turns skipped should be set to each enemy prefab like damage
    private bool skipTurn;

    // Array of sounds to play on attack
    public AudioClip[] attackSounds;

    // material and color vars to allow enemies to change color during attack animation
    Material skinMaterial;
    Color originalColour;

    // Called on start of prefab, see Unity3D docs
    protected override void Start()
    {
        // instantiate our vars to the current enemy
        skinMaterial = GetComponent<Renderer> ().material;
        originalColour = skinMaterial.color;
        // Add the enemy to the list, during movement the list is iterated and each enemy takes a turn
        GameManager.instance.AddEnemyToList(this);

        // find the transform of the player and set the target var
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //Start overrides the virtual Start function of the base class.
        base.Start();
    }

    //Override the OnMove function of Mob to include functionality needed for Enemy to skip turns.
    //See comments in Mob for more on how base OnMove function works.
    protected override void OnMove<T>(int xDir, int yDir)
    {
        // enemies skip a turn each round
        // TODO rather to skip turns and the number of turns to skip should be set by the enemy
        if (skipTurn)
        {
            skipTurn = false;
            return;
        }

        base.OnMove<T>(xDir, yDir);
        skipTurn = true;
    }

    //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // decide the direction to move, the target is the hero, on this turn
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            // targetting the Z plane as we are rotated 90 degrees
            yDir = target.position.z > transform.position.z ? 1 : -1;
        }
        else {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        OnMove<Hero>(xDir, yDir);

    }

    //Interact is called if Enemy attempts to move into a space occupied by a Player, it overrides the Interact function of Mob and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
    protected override void Interact<T>(T component)
    {
        //Declare hero and set it to equal the encountered component.
        Hero hero = component as Hero;
        //Call the TakeDamage function of hero passing it heroDamage, the amount of energy to be subtracted.
        hero.TakeDamage(heroDamage);
        // Start an animation Coroutine
        StartCoroutine(Attack(hero));
        // Play the attack sound
        SoundManager.instance.RandomizeSfx (attackSounds);
    }

    // Play an attack animation
    IEnumerator Attack(Component hitObstacle)
    {
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = hitObstacle.transform.position;

        float attackSpeed = 3;
        float percent = 0;

        GameManager.instance.enemyTurn = true;
        skinMaterial.color = Color.blue;
        while (percent <= 1)
        {

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
        skinMaterial.color = originalColour;
        GameManager.instance.enemyTurn = false;
    }
}
