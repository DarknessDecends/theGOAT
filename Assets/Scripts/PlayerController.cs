using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public static PlayerController instance;

	public float speed;
	public float maxHealth;
	public Weapon[] weapons;
	public float health;
	public int score = 0;
	public int highestUpgrade;
	public int deaths = 0;

	private Vector2 movementVector;
	private bool recentHit;
	private new SpriteRenderer renderer;
	private Rigidbody2D rigidBody;
	private Animator animator;
	private LevelManager levelManager;

	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start() {
		health = maxHealth;
		this.renderer = GetComponent<SpriteRenderer>();
		this.animator = GetComponent<Animator>();
		this.rigidBody = GetComponent<Rigidbody2D>();
		levelManager = GameObject.FindObjectOfType<LevelManager>();

		weapons = GetComponentsInChildren<Weapon>(true);

		recentHit = false;
	}

	void Update() {
		if (recentHit == false) {
			//arrowkeys change velocity
			movementVector = Vector2.zero;
			if (Input.GetKey("up") || Input.GetKey("w")) {
				movementVector += Vector2.up;
			}
			if (Input.GetKey("down") || Input.GetKey("s")) {
				movementVector += Vector2.down;
			}
			if (Input.GetKey("left") || Input.GetKey("a")) {
				movementVector += Vector2.left;
			}
			if (Input.GetKey("right") || Input.GetKey("d")) {
				movementVector += Vector2.right;
			}
			rigidBody.velocity = movementVector.normalized*speed;
		}
		//play walk animation if moving
		animator.SetBool("moving", rigidBody.velocity != Vector2.zero);

		//get mouse XY
		Vector2 mouseXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		//rotate character to face direction of mouse
		if (mouseXY.x < transform.position.x) {
			transform.rotation = Quaternion.Euler(0, 180, 0);
		} //end if
		else if (mouseXY.x > transform.position.x) {
			transform.rotation = Quaternion.Euler(0, 0, 0);
		} //end else if

		//left click
		if (Input.GetMouseButton(0)) {
			//calculate angle btwn player & mouse
			Quaternion angle = Quaternion.FromToRotation(Vector3.right, mouseXY - new Vector2(transform.position.x, transform.position.y));
			if (weapons != null) {
				string debugString = "";
				foreach (Weapon weapon in weapons) {
					if (weapon.isActiveAndEnabled) {
						weapon.attack(angle);
					}
					debugString += weapon.name + ", ";
				} //end for
				Debug.Log(debugString);
			} //end if
		} //end if
	} //end update

	public void hurt(float damage) {
		if (health - damage <= 0) {
			deaths++;
			levelManager.LoadLevel("Death");
		} else {
			health -= damage;
			recentHit = true;
			renderer.color = Color.red;
			Invoke("hitTimeOut", 0.03f);

		}
	}

	void hitTimeOut() {
		renderer.color = Color.white;
		recentHit = false;
	}

	public int getScore() {
		return score;
	}

	public void setScore(int val) {
		score = val;
	}

	public void Score(int val) {
		if (score + val >= 0) {
			score += val;
		} else {
			score = 0;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == LayerMask.NameToLayer("Pickups")) {
			int childnum = -1;
			switch (collider.gameObject.name) {
				case "SwordPickup":
					childnum = 0;
					break;
				case "BowPickup":
					childnum = 1;
					break;
				case "AirStaffPickup":
					childnum = 2;
					break;
				case "Earth Staff Pickup":
					childnum = 3;
					break;
				case "Water Staff Pickup":
					childnum = 4;
					break;
				case "Fire Staff Pickup":
					childnum = 5;
					break;
				case "Battle Axe Pickup":
					childnum = 6;
					break;
				case "Mace Pickup":
					childnum = 7;
					break;
				case "Club Pickup":
					childnum = 8;
					break;
				case "Basic Staff Pickup":
					childnum = 9;
					break;
			}
			Weapon child = transform.GetChild(childnum).GetComponent<Weapon>();
			child.gameObject.SetActive(true);
			Destroy(collider.gameObject);
		} //end if

		if (collider.gameObject.layer == LayerMask.NameToLayer("FakeWall")) {
			string tag = collider.gameObject.tag;
			var objects = GameObject.FindGameObjectsWithTag(tag);
			foreach (var obj in objects) {
				Destroy(obj);
			} //end foreach
		} //end if
	} //end onTriggerEnter2D
} //end Class