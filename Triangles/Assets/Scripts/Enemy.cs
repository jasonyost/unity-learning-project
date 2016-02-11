using UnityEngine;
using System.Collections;

public class Enemy : Mob {

    public int heroDamage;

    // private Animator animator;
    private Transform target;
    private bool skipTurn;

    // public AudioClip attackSound1;						//First of two audio clips to play when attacking the player.
		// public AudioClip attackSound2;						//Second of two audio clips to play when attacking the player.

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        // animator = GetComponent<Animator>();
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
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }else{
						xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        OnMove<Hero>(xDir, yDir);

    }

    protected override void Interact<T>(T component)
		{
        Hero hero = component as Hero;
        hero.TakeDamage(heroDamage);
        // animator.SetTrigger("enemyStrike");
        // SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);
    }
}
