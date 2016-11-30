﻿using UnityEngine;
using System.Collections;

public class DanglerStats : MonoBehaviour {

	public float hitpoints = 10.0f;
    public int scoreValue = 50;
    public GameObject bloodEffect;
    
    public void damageDangler (float damage) {
        hitpoints -= damage;
    }

    public void killDangler () {
        hitpoints = 0.0f;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>().modifyScore(scoreValue);
        GameObject blood = (GameObject)Instantiate(bloodEffect, transform.parent.position, new Quaternion(), transform.parent);
        blood.GetComponent<ParticleSystem>().Play();
        gameObject.SetActive(false);
        transform.GetComponent<DanglerAttack1>().beginFlinging();
    }

    void Update () {
        if (hitpoints <= 0.0f) {
            killDangler();
        }
    }
}
