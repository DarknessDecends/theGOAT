using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float speed = 15.0f;
	public float health = 250;
	public float bulletSpeed;
	public GameObject bullet;
	

	void Update () {
		//movement
		if(Input.GetKey(KeyCode.W)){
			this.GetComponent<Rigidbody2D>().velocity = (new Vector2(this.GetComponent<Rigidbody2D>().velocity.x,speed*Time.deltaTime));
		}else if(Input.GetKeyUp(KeyCode.W)){
			this.GetComponent<Rigidbody2D>().velocity = (new Vector2(this.GetComponent<Rigidbody2D>().velocity.x,0));
		}
		if(Input.GetKey(KeyCode.A)){
			this.GetComponent<Rigidbody2D>().velocity = (new Vector2(-speed*Time.deltaTime,this.GetComponent<Rigidbody2D>().velocity.y));
		}else if(Input.GetKeyUp(KeyCode.A)){
			this.GetComponent<Rigidbody2D>().velocity = (new Vector2(0,this.GetComponent<Rigidbody2D>().velocity.y));
		}
		if(Input.GetKey(KeyCode.S)){
			this.GetComponent<Rigidbody2D>().velocity = (new Vector2(this.GetComponent<Rigidbody2D>().velocity.x,-speed*Time.deltaTime));
		}else if(Input.GetKeyUp(KeyCode.S)){
			this.GetComponent<Rigidbody2D>().velocity =(new Vector2(this.GetComponent<Rigidbody2D>().velocity.x,0));
		}
		if(Input.GetKey(KeyCode.D)){
			this.GetComponent<Rigidbody2D>().velocity = (new Vector2(speed*Time.deltaTime,this.GetComponent<Rigidbody2D>().velocity.y));
		}else if(Input.GetKeyUp(KeyCode.D)){
			this.GetComponent<Rigidbody2D>().velocity = (new Vector2(0,this.GetComponent<Rigidbody2D>().velocity.y));
		}
		if (Input.GetMouseButtonDown(0)){
			Vector2 mousePosInGame = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x,Screen.height-Input.mousePosition.y));
			Vector2 charPos = transform.position;
			Vector2 direction = mousePosInGame - charPos;
			GameObject basicShot = Instantiate(bullet,charPos, Quaternion.Euler(0,0,Mathf.Atan2(direction.y, direction.x )*Mathf.Rad2Deg)) as GameObject;
			basicShot.GetComponent<Rigidbody2D>().velocity = direction*speed;
		}
		
	}
	void OnCollision2D(Collision2D collision){
	}
}