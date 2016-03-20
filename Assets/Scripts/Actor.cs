using UnityEngine;
using System.Collections;

//parent class of PlayerController and EnemyController
public abstract class Actor : MonoBehaviour {
	public float health = 500;
	public float speed = 10;

	protected bool recentHit = false;
	protected new SpriteRenderer renderer;
	protected new Rigidbody2D rigidbody;
	protected Animator animator;

	// these have to be implemented in child classes
	public abstract void hurt(float damage, Vector2 direction, float knockback);
	protected abstract void hitTimeOut();
}
