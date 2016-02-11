using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    // Reference to the game Object
    // public Sprite[] obstacleSprites;

    // public AudioClip chopSound1;				//1 of 2 audio clips that play when the wall is attacked by the player.
		// public AudioClip chopSound2;				//2 of 2 audio clips that play when the wall is attacked by the player.
    // public Sprite dmgSprite;
    public int obstacleHealth = 3;

    // private SpriteRenderer spriteRenderer;
    // private int spriteIndex;

    void Awake(){
        // spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
        // int obstacleSpriteIndex = Random.Range(0, obstacleSprites.Length);
        // spriteRenderer.sprite = obstacleSprites[obstacleSpriteIndex];
    }

    public void DamageObstacle(int damage)
    {
        // SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
        // string currentSpriteName = spriteRenderer.sprite.name;

        // render the obstacles damage sprite
        // spriteRenderer.sprite = dmgSprite;

        // apply damage to the obstacle
        obstacleHealth -= damage;

        // disable the obstacle if the damage is greater than health
				if(obstacleHealth <= 0){
					gameObject.SetActive(false);
				}
    }
}
