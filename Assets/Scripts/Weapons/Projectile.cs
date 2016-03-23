using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float knockback;
	[HideInInspector]
	public float damage;
	public Sprite boltExplosion;
	private Vector2 constantVelocity;
	private new Rigidbody2D rigidbody;
	

	void Start() {
		rigidbody = GetComponent<Rigidbody2D>();
		constantVelocity = rigidbody.velocity;
	}

	void Update() {
		if (rigidbody != null) {
			rigidbody.velocity = constantVelocity;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		OnTriggerEnter2D(collision.collider);
	}
	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == LayerMask.NameToLayer("Wall") && !collider.isTrigger) {
			this.stop();
		} else {
			if (collider.gameObject.GetComponent<BossController>() != null) { //if its a boss do damage to the boss
				BossController target = collider.gameObject.GetComponent<BossController>();
				if (target) {
					target.hurt(damage, rigidbody.velocity, knockback);
					this.stop();
				} //end if
			} else if (collider.gameObject.GetComponent<EnemyController>() != null) { //if its an Enemy hurt the enemy
				EnemyController target = collider.gameObject.GetComponent<EnemyController>();
				if (target) {
					target.hurt(damage, rigidbody.velocity, knockback);
					this.stop();
				} //end if
			} else if (collider.gameObject.GetComponent<PlayerController>() != null) { //if its The player hurt the player
				PlayerController target = collider.gameObject.GetComponent<PlayerController>();
				if (target) {
					target.hurt(damage, rigidbody.velocity, knockback);
					this.stop();
				} //end if
			} //end if
		} //end if
	}

	private void stop() {
		rigidbody.velocity = new Vector2(0, 0);
		Destroy(rigidbody);
		Invoke("selfDestroy", 0.25f);
	}

	public void selfDestroy(){
		Destroy(gameObject);
	}
}
