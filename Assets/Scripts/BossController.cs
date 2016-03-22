using UnityEngine;
using System.Collections.Generic;
using System;

public class BossController : Actor {
	public static BossController instance;

	public float maxHealth = 2500;
	public int scoreWorth;
	public int topTouchDamage;
	public int bottomTouchDamage;
	public int eyeDamage;
	public float cooldown;
	public float touchKnockback;
	public List<int> roomCoords = new List<int>();
	public Vector3 resetPoint;
	public GameObject projectilePrefab;
	public bool detected;

	private AnimatorStateInfo currentBaseState;
	private Transform player;
	private int thrown = 0;

	static int attackEnd = Animator.StringToHash("Base Layer.attackEnd");

	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		if (instance != this) {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start() {
		player = PlayerController.instance.transform;
		rigidbody = this.GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		renderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update() {
		detected = false;
		if (player != null) { //if they player exists
			Vector2 atPlayer = player.position - transform.position; //vector from boss to player
			if (player.position.x > roomCoords[0] && //roomCoords[0] is left x coord
				player.position.x < roomCoords[1] && //roomCoords[0] is right x coord
				player.position.y > roomCoords[2] && //roomCoords[0] is bottom y coord
				player.position.y < roomCoords[3]) //roomCoords[0] is top y coord
			{//player is in boss area
				detected = true;

				if (thrown == 2) {
					this.rigidbody.velocity = atPlayer.normalized * speed * 4 * Time.deltaTime;
				} else if (thrown == 0) {
					this.rigidbody.velocity = Vector2.zero; ;
					animator.SetBool("throw", true);
					thrown = 1;
					Invoke("thrownTimeOut", cooldown);
				}
				currentBaseState = animator.GetCurrentAnimatorStateInfo(0);
				if (currentBaseState.IsName("attackEnd") && thrown == 1) {
					animator.SetBool("throw", false);
					thrown = 2;
					attack(Quaternion.FromToRotation(Vector3.right, atPlayer));
				}
			}
			if (!detected) { //return to spawn, reset health
				health = maxHealth;
				int roundx = Mathf.FloorToInt(resetPoint.x - transform.position.x);
				int roundy = Mathf.FloorToInt(resetPoint.x - transform.position.y);
				Vector2 dir = new Vector2(roundx, roundy);

				this.rigidbody.velocity = dir.normalized * speed * 4 * Time.deltaTime;
			}
			//if distance between x's is greater than distance between y's
			if ((Mathf.Abs((this.transform.position.x - player.position.x)) > Mathf.Abs((this.transform.position.y - player.position.y))) && detected) {
				animator.SetBool("isHorizontal", true);
				animator.SetBool("isUp", false);
				animator.SetBool("isDown", false);

				if (atPlayer.x < 0) {
					transform.rotation = Quaternion.Euler(0, 180, 0);
				} else {
					transform.rotation = Quaternion.Euler(0, 0, 0);
				}

			} else if ((Mathf.Abs((this.transform.position.x - player.position.x)) < Mathf.Abs((this.transform.position.y - player.position.y))) && detected) {
				if (this.transform.position.y < player.position.y) {
					animator.SetBool("isHorizontal", false);
					animator.SetBool("isUp", true);
					animator.SetBool("isDown", false);
				} else {
					animator.SetBool("isHorizontal", false);
					animator.SetBool("isUp", false);
					animator.SetBool("isDown", true);
				}
			}
		}
	}

	override public void hurt(float damage, Vector2 direction, float knockback) {
		health -= damage;
		if (health <= 0) {
			die();
		} else {
			rigidbody.velocity += direction.normalized * knockback;

			renderer.color = Color.red;
			Invoke("hitTimeOut", 0.03f);

			//increase boss difficulty
			if (health / maxHealth < .25) {
				speed = 60;
				cooldown = 2.66f;
			} else if (health / maxHealth < .5) {
				speed = 40;
				cooldown = 4.33f;
			}
		}
	}

	override protected void die() {
		player.GetComponentInParent<PlayerController>().Score(scoreWorth);
		Destroy(gameObject);
	}

	void OnCollisionEnter2D(Collision2D collider) {
		if (detected && collider.transform == player) { //if enemy sees player an is touching him
			PlayerController foundPlayer = collider.gameObject.GetComponent<PlayerController>();

			Vector2 atPlayer = (foundPlayer.transform.position - this.transform.position);
			foundPlayer.hurt(UnityEngine.Random.Range(bottomTouchDamage, topTouchDamage + 1), atPlayer, touchKnockback); //hit the player

			Debug.Log(atPlayer);
		}
	}

	void thrownTimeOut() {
		thrown = 0;
	}

	public void attack(Quaternion angle) {
		//make new eye
		Projectile shot = (Instantiate(projectilePrefab) as GameObject).GetComponent<Projectile>();
		shot.transform.position = transform.position;
		shot.transform.rotation *= angle; //add the angles (some prefabs have rotation.y == -45)
		shot.damage = this.eyeDamage; //set damage
									  //accelerate bullet
		Vector2 shotVector = angle * Vector3.right * speed; //rotate "right" vector by angle
		shot.GetComponent<Rigidbody2D>().AddForce(shotVector, ForceMode2D.Impulse);
	}
}