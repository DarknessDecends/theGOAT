using UnityEngine;
using System.Collections;

public class MeleeEnemyController : EnemyController {

    void Start() {
        baseStart();
    }

    void Update() {
        baseUpdate();
    }

    void OnCollisionEnter2D(Collision2D collider) {
        baseOnCollisionEnter2D(collider);
    }


    protected override void attack() {
        this.rigidbody.velocity = dir.normalized * speed* 4;
    }

}
