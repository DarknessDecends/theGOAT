using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed;
	public float health;
	public float bulletSpeed;
	public GameObject bulletPrefab;
	private Rigidbody2D rigidBody;
	
	void Start() {
		this.rigidBody = this.GetComponent<Rigidbody2D>();
	}

	void Update () {
		//initial valocity 0
		Vector2 velocity = Vector2.zero;
		//arrow keys change horizontal velocity
		Debug.Log(Vector2.right + ", " + Input.GetAxis("Horizontal")/8000 + ", " + speed + ", " + Time.deltaTime);
		velocity += Vector2.right*Input.GetAxis("Horizontal")*speed*Time.deltaTime;
		//arrow keys change vertical velocity
		velocity += Vector2.up*Input.GetAxis("Vertical")*speed*Time.deltaTime;
		Debug.Log(velocity);
		rigidBody.velocity = velocity; //set new velocity

		//left click
		if (Input.GetMouseButton(0)) {
			//get mouse XY
			Vector2 mouseXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//calculate spawn angle
			Quaternion angle = Quaternion.FromToRotation(Vector3.right, mouseXY - new Vector2(transform.position.x, transform.position.y));
			//make new bullet
			GameObject bullet = Instantiate(bulletPrefab, transform.position, angle) as GameObject;
			//accelerate bullet
			bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.right * 500f);
		}
		
	}
	void OnCollision2D(Collision2D collision){
	}
}