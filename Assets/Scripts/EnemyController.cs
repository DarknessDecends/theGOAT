using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	public float health = 500;
	public float speed = 500;
	public int scoreWorth;
	public int movementChangeTime=500;
	public int detectionRange;
	public int topDamage;
	public int bottomDamage;
	public float knockback;

	private Transform player;
	private bool detected;
	private int directionchange = 0;
	private int horizontalMovement;
	private int verticalMovement;
	private Rigidbody2D rigidBody;
	private Animator animator;
	private new SpriteRenderer renderer;
	private bool recentHit = false;

	void Start() {
		player = PlayerController.instance.transform;
		rigidBody = this.GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		renderer = GetComponent<SpriteRenderer>();
	}

	void Update () {
		detected = false; //the player hasnt been found
		if (player != null) { //if they player exists
			if (recentHit == false) { //if the enemy hasn't recently been hit
				if (Vector3.Distance(player.position, transform.position) <= detectionRange) { //if the enemy can see him run at him
					Vector2 dir = player.position - transform.position;
					RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRange);
					Debug.DrawLine(transform.position, hit.point, Color.red);
					if (hit.transform == player) {
						detected = true;
						this.rigidBody.velocity = dir.normalized * speed * 4 * Time.deltaTime;
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
					velocity += Vector2.right * horizontalMovement * speed * Time.deltaTime;
					//change vertical velocity
					velocity += Vector2.up * -verticalMovement * speed * Time.deltaTime;
					this.rigidBody.velocity = velocity; //set new velocity

					//play slither animation if moving
					//animator.SetBool("moving", rigidBody.velocity != Vector2.zero);
				}
				if (this.rigidBody.velocity.x < 0) { //if their going left face left if their going right face right
					transform.rotation = Quaternion.Euler(0, 180, 0);
				} else {
					transform.rotation = Quaternion.Euler(0, 0, 0);
				}

			}
		}
	}
	
	public void hurt(float damage) {
		health -= damage;
		if (health <= 0) {
			player.GetComponentInParent<PlayerController>().Score(scoreWorth);
			Destroy(gameObject);
		} else {
			recentHit = true;
			renderer.color = Color.red;
			Invoke("hitTimeOut", 0.03f);
		}
	}

	void OnCollisionEnter2D(Collision2D collider){
		if (detected && collider.transform == player) { //if enemy sees player an is touching him
			PlayerController foundPlayer = collider.gameObject.GetComponent<PlayerController>(); ;
			foundPlayer.hurt(Random.Range(bottomDamage, topDamage + 1)); //hit the player
			Vector2 atPlayer = (foundPlayer.transform.position - this.transform.position).normalized*knockback;

			//foundPlayer.GetComponent<Rigidbody2D>().AddForce(atPlayer, ForceMode2D.Impulse); //.velocity += atPlayer; //apply knockback
			collider.gameObject.GetComponent<Rigidbody2D>().velocity += (GetComponent<Rigidbody2D>().velocity)*knockback; //apply knockback
			Debug.Log(atPlayer);
		}
	}

	void hitTimeOut() {
		renderer.color = Color.white;
		recentHit = false;
	}

}
