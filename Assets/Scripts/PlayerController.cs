using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public float speed;
	public float health;
	
	private Rigidbody2D rigidBody;
	private Animator animator;
	private List<Weapon> weapons;
	
	
	void Start() {
		this.animator = GetComponent<Animator>();
		this.rigidBody = this.GetComponent<Rigidbody2D>();

		weapons = new List<Weapon>();
		weapons.Add(this.gameObject.GetComponent<Weapon>());
	}

	void Update () {
		//rrowkeys change velocity
		Vector2 velocity = Vector2.zero;
		if (Input.GetKey("up") || Input.GetKey("w")) {
			velocity += Vector2.up;
		}
		if (Input.GetKey("down") || Input.GetKey("s")) {
			velocity += Vector2.down;
		}
		if (Input.GetKey("left") || Input.GetKey("a")) {
			velocity += Vector2.left;
		}
		if (Input.GetKey("right") || Input.GetKey("d")) {
			velocity += Vector2.right;
		}
		rigidBody.velocity = velocity.normalized*speed*Time.deltaTime;
		
		//play walk animation if moving
		animator.SetBool("moving", rigidBody.velocity != Vector2.zero);

		//left click
		if (Input.GetMouseButton(0)) {
			//get mouse XY
			Vector2 mouseXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//calculate angle btwn player & mouse
			Quaternion angle = Quaternion.FromToRotation(Vector3.right, mouseXY - new Vector2(transform.position.x, transform.position.y));
			for (int i = 0; i < weapons.Count; i++){
				weapons[i].attack(angle);
			}
		}
		
	}
	void OnCollision2D(Collision2D collision){
		
	}
}