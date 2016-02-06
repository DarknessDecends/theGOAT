using UnityEngine;
using System.Collections;
using System;

public class Sword : Weapon {
	public override void attack(Quaternion angle) {
		transform.Rotate(new Vector3(0, 0, -(360/cooldown)*Time.deltaTime)); //one full spin per "cooldown" amount seconds
	}

	void OnTriggerEnter2D(Collider2D collider) {
		EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
		if (enemy) {
			enemy.hurt(damage);
		}
	}
}
