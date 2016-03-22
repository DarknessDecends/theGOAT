using UnityEngine;
using System.Collections;

//parent class of PlayerController and EnemyController
public abstract class Actor : MonoBehaviour {
	public float health = 500;
	public float speed = 10;
	public float knockbackResist = 5;

	protected bool recentHit = false;
	protected new SpriteRenderer renderer;
	protected new Rigidbody2D rigidbody;
	protected Animator animator;

	protected virtual void baseStart() {
		this.renderer = GetComponent<SpriteRenderer>();
		this.animator = GetComponent<Animator>();
		this.rigidbody = GetComponent<Rigidbody2D>();
	}

	// these have to be implemented in child classes
	public virtual void hurt(float damage, Vector2 direction, float knockback) {
		if (health - damage <= 0) {
			die();
		} else {
			health -= damage;
			transform.Translate((direction.normalized * knockback) / knockbackResist);
			recentHit = true;
			renderer.color = Color.red;
			Invoke("hitTimeOut", 0.03f);
		}
	}

	private void hitTimeOut() {
		renderer.color = Color.white;
		recentHit = false;
	}

	protected abstract void die();
}
