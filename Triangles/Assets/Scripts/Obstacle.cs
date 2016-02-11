﻿using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    // Reference to the game Object

    public AudioClip[] breakSounds;
    public ParticleSystem destructEffect;
    public int obstacleHealth = 3;

    Material skinMaterial;
    void Awake(){
    }

    // Use this for initialization
    void Start () {
      skinMaterial = GetComponent<Renderer> ().material;
    }

    public void DamageObstacle(int damage)
    {
        SoundManager.instance.RandomizeSfx (breakSounds);

        skinMaterial.color = Color.black;

        // apply damage to the obstacle
        obstacleHealth -= damage;

        // disable the obstacle if the damage is greater than health
				if(obstacleHealth <= 0){
            Destroy(Instantiate(destructEffect.gameObject, transform.position, Quaternion.Euler(Vector3.right * 90)) as GameObject, destructEffect.startLifetime);
            Destroy(gameObject);
				}
    }
}
