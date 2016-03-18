using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public static PlayerController instance;

	public float speed;
	public float maxHealth;
	public List<Weapon> weapons;
	public float health;
	public int score = 0;
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

		weapons = new List<Weapon>();

		recentHit = false;
	}

	void Update () {
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
		if (mouseXY.x < transform.position.x){
			transform.rotation = Quaternion.Euler(0, 180, 0);
		} //end if
		else if (mouseXY.x > transform.position.x){
			transform.rotation = Quaternion.Euler(0, 0, 0);
		} //end else if

		//left click
		if (Input.GetMouseButton(0)) {	
			//calculate angle btwn player & mouse
			Quaternion angle = Quaternion.FromToRotation(Vector3.right, mouseXY - new Vector2(transform.position.x, transform.position.y));
			if (weapons != null) {
				for (int i = 0; i < weapons.Count; i++) {
					weapons[i].attack(angle);
				} //end for
			} //end if
		} //end if
	} //end update

	void FixedUpdate() {

	}

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
			string name = collider.gameObject.name;
			if (name == "Basic Staff Pickup") {
				Weapon newWep = GetComponent<Weapon>();
				if (!weapons.Contains(newWep)) {
					weapons.Add(newWep);     //add basic Staff
				} else {
					GetComponent<Weapon>().cooldown /= 2;
				}
				Destroy(collider.gameObject);
			} //end if
			if (name == "SwordPickup") {
			   weapons.Add(transform.GetChild(0).GetComponent<Weapon>()); //add sword
			   transform.GetChild(0).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "BowPickup") {
			   weapons.Add(transform.GetChild(1).GetComponent<Weapon>()); //add sword
			   transform.GetChild(1).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "AirStaffPickup") {
			   weapons.Add(transform.GetChild(2).GetComponent<Weapon>()); //add sword
			   transform.GetChild(2).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "Earth Staff Pickup") {
			   weapons.Add(transform.GetChild(3).GetComponent<Weapon>()); //add sword
			   transform.GetChild(3).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "Water Staff Pickup") {
			   weapons.Add(transform.GetChild(4).GetComponent<Weapon>()); //add sword
			   transform.GetChild(4).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "Fire Staff Pickup") {
			   weapons.Add(transform.GetChild(5).GetComponent<Weapon>()); //add sword
			   transform.GetChild(5).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "Battle Axe Pickup") {
			   weapons.Add(transform.GetChild(6).GetComponent<Weapon>()); //add sword
			   transform.GetChild(6).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "Mace Pickup") {
			   weapons.Add(transform.GetChild(7).GetComponent<Weapon>()); //add sword
			   transform.GetChild(7).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
		   if (name == "Club Pickup") {
			   weapons.Add(transform.GetChild(8).GetComponent<Weapon>()); //add sword
			   transform.GetChild(8).gameObject.SetActive(true);
			   Destroy(collider.gameObject);
		   } //end if
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