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

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Pickups")) {
            string name = collider.gameObject.name;
            if (name == "Basic Staff Pickup") {
                weapons.Add(GetComponent<Weapon>()); //add basic Staff
                Destroy(collider.gameObject);
            } //end if
            if (name == "SwordPickup") {
               weapons.Add(transform.GetChild(0).GetComponent<Weapon>()); //add sword
               transform.GetChild(0).gameObject.SetActive(true);
               Destroy(collider.gameObject);
            } //end if
        } //end if
    } //end onTriggerEnter2D
} //end Class