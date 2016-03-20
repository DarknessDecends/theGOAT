using UnityEngine;
using System.Collections;

public class EnemyController : Actor {
	
	public int scoreWorth;
	public int movementChangeTime=500;
	public int detectionRange;
	public int topDamage;
	public int bottomDamage;
	public float knockbackDealt;
	public float knockbackRecieved;

	protected PlayerController player;
	protected Vector2 dir;


	private bool detected;
	private int directionchange = 0;
	private int horizontalMovement;
	private int verticalMovement;

	protected void baseStart() {
		player = PlayerController.instance;
		knockbackRecieved = player.highestUpgrade;
		rigidbody = this.GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		renderer = GetComponent<SpriteRenderer>();
	}

	protected void baseUpdate () {
		detected = false; //the player hasnt been found
		if (player != null) { //if they player exists
			if (recentHit == false) { //if the enemy hasn't recently been hit
				if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange) { //if the enemy can see him run at him
					dir = player.transform.position - transform.position;
					RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRange);
					Debug.DrawLine(transform.position, hit.point, Color.red);
					if (hit.transform == player.transform) {
						detected = true;
						attack();
					}
				}
				if (!detected) {
					if (directionchange == 0) {
						horizontalMovement = Random.Range(-1, 2);
						verticalMovement = Random.Range(-1, 2);
						directionchange = movementChangeTime;
					} else {
						directionchange--;
					}

					//initial valocity 0
					Vector2 velocity = Vector2.zero;
					//change horizontal velocity
					velocity += Vector2.right * horizontalMovement;
					//change vertical velocity
					velocity += Vector2.up * -verticalMovement;
					this.rigidbody.velocity = velocity.normalized * speed; //set new velocity

					//play moving animation if moving
					animator.SetBool("moving", this.rigidbody.velocity != Vector2.zero);
				}
				if (this.rigidbody.velocity.x < 0) { //if their going left face left if their going right face right
					transform.rotation = Quaternion.Euler(0, 180, 0);
				} else {
					transform.rotation = Quaternion.Euler(0, 0, 0);
				}

			}
		}
	}

	public override void hurt(float damage, Vector2 direction, float knockback) {
		health -= damage;
		if (health <= 0) {
			Destroy(gameObject);
		} else {
			rigidbody.velocity += direction.normalized * knockback;

			recentHit = true;
			renderer.color = Color.red;
			Invoke("hitTimeOut", 0.03f);
		}
	}

	protected void baseOnCollisionEnter2D(Collision2D collider){
		if (detected && collider.transform == player.transform) { //if enemy sees player an is touching him
			PlayerController foundPlayer = collider.gameObject.GetComponent<PlayerController>();

			Vector2 atPlayer = foundPlayer.transform.position - transform.position;
			foundPlayer.hurt(Random.Range(bottomDamage, topDamage + 1), atPlayer, knockbackDealt); //hit the player
		}
	}

	override protected void hitTimeOut() {
		renderer.color = Color.white;
		recentHit = false;
	}

	protected virtual void attack() {}

}
