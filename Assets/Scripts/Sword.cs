using UnityEngine;
using System.Collections;
using System;

public class Sword : Weapon {

	public int swingSpeed;
	private int change;
	private bool upSwing;
	public float knockback;

	void Start() {
		change = 60;
		upSwing = false;
	}

	public override void attack(Quaternion angle) {

		if (change <= 0) {
			upSwing = true;
		} else if (change >= 70) {
			upSwing = false;
		}

		transform.rotation = Quaternion.Euler(0, 0, angle.eulerAngles.z - change);
		if (upSwing) {
			change += swingSpeed;
		} else {
			change -= swingSpeed;
		}

	}

	void OnTriggerEnter2D(Collider2D collider) {
		EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
		if (enemy) {
			enemy.hurt(damage);
			Vector2 atEnemy = (enemy.transform.position - this.transform.position).normalized*knockback;
			enemy.GetComponent<Rigidbody2D>().velocity += atEnemy; //apply knockback
		}
	}
}

	
