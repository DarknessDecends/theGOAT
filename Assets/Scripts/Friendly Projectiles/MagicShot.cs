﻿using UnityEngine;
using System.Collections;

public class MagicShot : MonoBehaviour {

    public float knockback;
    [HideInInspector]
    public float damage;
    public Sprite boltExplosion;

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")){
            GetComponent<SpriteRenderer>().sprite = boltExplosion;
            GetComponent<Rigidbody2D>().velocity = (new Vector2(0, 0));
            Invoke("selfDestroy", 0.25f);
        }
        else {
            EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
            if (enemy){
                enemy.hurt(damage);
                collider.gameObject.GetComponent<Rigidbody2D>().velocity += (GetComponent<Rigidbody2D>().velocity)*knockback; //apply knockback
                GetComponent<SpriteRenderer>().sprite = boltExplosion;
                GetComponent<Rigidbody2D>().velocity = (new Vector2(0, 0));
                Invoke("selfDestroy", 0.25f);
            }
        }
    }

    public void selfDestroy(){
        Destroy(gameObject);
    }
}
