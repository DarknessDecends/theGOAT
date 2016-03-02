using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float knockback;
    [HideInInspector]
    public float damage;
    public Sprite boltExplosion;

    void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            GetComponent<SpriteRenderer>().sprite = boltExplosion;
            GetComponent<Rigidbody2D>().velocity = (new Vector2(0, 0));
            Invoke("selfDestroy", 0.25f);
        } else {
            if (collider.gameObject.GetComponent<BossController>() != null) { //if its a boss do damage to the boss
                BossController target = collider.gameObject.GetComponent<BossController>();
                if (target) {
                    target.hurt(damage);
                    collider.gameObject.GetComponent<Rigidbody2D>().velocity += (GetComponent<Rigidbody2D>().velocity) * knockback; //apply knockback
                    GetComponent<SpriteRenderer>().sprite = boltExplosion;
                    GetComponent<Rigidbody2D>().velocity = (new Vector2(0, 0));
                    Invoke("selfDestroy", 0.25f);
                } //end if
            } else if (collider.gameObject.GetComponent<EnemyController>() != null) { //if its an Enemy hurt the enemy
                EnemyController target = collider.gameObject.GetComponent<EnemyController>();
                if (target) {
                    target.hurt(damage);
                    collider.gameObject.GetComponent<Rigidbody2D>().velocity += (GetComponent<Rigidbody2D>().velocity) * knockback; //apply knockback
                    GetComponent<SpriteRenderer>().sprite = boltExplosion;
                    GetComponent<Rigidbody2D>().velocity = (new Vector2(0, 0));
                    Invoke("selfDestroy", 0.25f);
                } //end if
            } else if (collider.gameObject.GetComponent<PlayerController>() != null) { //if its The player hurt the player
                PlayerController target = collider.gameObject.GetComponent<PlayerController>();
                if (target) {
                    target.hurt(damage);
                    collider.gameObject.GetComponent<Rigidbody2D>().velocity += (GetComponent<Rigidbody2D>().velocity) * knockback; //apply knockback
                    GetComponent<SpriteRenderer>().sprite = boltExplosion;
                    GetComponent<Rigidbody2D>().velocity = (new Vector2(0, 0));
                    Invoke("selfDestroy", 0.25f);
                } //end if
            } //end if
        } //end if
    }  //end method

    public void selfDestroy(){
        Destroy(gameObject);
    }
}
