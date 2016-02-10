using UnityEngine;
using System.Collections;
using System;

public class Sword : Weapon {

    public int swingSpeed;
    private int change;
    private bool upSwing;

    void Start() {
        change = 60;
        upSwing = false;
    }

    public override void attack(Quaternion angle){
        //transform.Rotate(new Vector3(0, 0, -(360/cooldown)*Time.deltaTime)); //one full spin per "cooldown" amount seconds
                
        if (change <= 0 ){
            upSwing = true;
        }else if (change >= 70){
            upSwing = false;
        }

        transform.rotation = Quaternion.Euler(0, 0, angle.eulerAngles.z - change);
        if (upSwing){
            change+= swingSpeed;
        }else{
            change-= swingSpeed;
        }

    }

	void OnTriggerEnter2D(Collider2D collider) {
		EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
		if (enemy) {
			enemy.hurt(damage);
		}
	}
}
