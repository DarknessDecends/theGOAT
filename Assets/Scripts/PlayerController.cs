using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public float speed;
	public float health;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	
	private Rigidbody2D rigidBody;
	private List<Weapon> weapons;
	
	
	void Start() {
		this.rigidBody = this.GetComponent<Rigidbody2D>();
		weapons = new List<Weapon>();
		weapons.Add(this.gameObject.GetComponent<Weapon>());
	}

	void Update () {
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
	
		//left click
		if (Input.GetMouseButton(0)) {
			//get mouse XY
			Vector2 mouseXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//calculate spawn angle
			Quaternion angle = Quaternion.FromToRotation(Vector3.right, mouseXY - new Vector2(transform.position.x, transform.position.y));
			for (int i = 0; i< weapons.Count; i++){
				Weapon currentWep = weapons[i];
				//make new bullet
				GameObject attack = Instantiate(currentWep.WeaponPrefab, transform.position, angle) as GameObject;
				if (currentWep.ranged){
					//accelerate bullet
					attack.GetComponent<Rigidbody2D>().AddForce(attack.transform.right * 500f);
				}
			}
		}
		
	}
	void OnCollision2D(Collision2D collision){
		//if (GetComponent<Collider>().gameObject.GetComponent<Weapon>()){
		//	weapons.Add();
		//}
	}
}