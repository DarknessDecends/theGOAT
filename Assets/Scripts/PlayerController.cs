using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public float speed;
	public float health;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	
	private Rigidbody2D rigidBody;
	private List<Weapon> weapons = new List<Weapon>();
	
	
	void Start() {
		this.rigidBody = this.GetComponent<Rigidbody2D>();
		weapons.Add(this.gameObject.GetComponent<Weapon>());
	}

	void Update () {
		Debug.Log(weapons[0]);
		//initial valocity 0
		Vector2 velocity = Vector2.zero;
		//arrow keys change horizontal velocity
		velocity += Vector2.right*Input.GetAxis("Horizontal")*speed*Time.deltaTime;
		//arrow keys change vertical velocity
		velocity += Vector2.up*Input.GetAxis("Vertical")*speed*Time.deltaTime;
		rigidBody.velocity = velocity; //set new velocity
	
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
					currentWep.GetComponent<Rigidbody2D>().AddForce(attack.transform.right * 500f);
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