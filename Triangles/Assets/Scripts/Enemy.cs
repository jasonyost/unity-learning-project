using UnityEngine;
using System.Collections;

public class Enemy : Mob
{

    public int heroDamage;

    private Transform target;
    private bool skipTurn;

    public AudioClip[] attackSounds;

    Material skinMaterial;
    Color originalColour;

    protected override void Start()
    {
        skinMaterial = GetComponent<Renderer> ().material;
        originalColour = skinMaterial.color;
        GameManager.instance.AddEnemyToList(this);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void OnMove<T>(int xDir, int yDir)
    {
        // zombies skip a turn each round, however this will be on a per enemy basis in the future
        if (skipTurn)
        {
            skipTurn = false;
            return;
        }

        base.OnMove<T>(xDir, yDir);
        skipTurn = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // decide the direction to move, the target is the hero, on this turn
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.z > transform.position.z ? 1 : -1;
        }
        else {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        OnMove<Hero>(xDir, yDir);

    }

    protected override void Interact<T>(T component)
    {
        Hero hero = component as Hero;
        hero.TakeDamage(heroDamage);
        StartCoroutine(Attack(hero));
        SoundManager.instance.RandomizeSfx (attackSounds);
    }

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
