using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	public float health = 500;
	public float speed = 500;
	public int movementChangeTime=500;
	
	private int directionchange = 0;
	private int horizontalMovement;
	private int verticalMovement;
	
	void Update () {
		if (directionchange == 0){
			horizontalMovement=Random.Range(-1,2);
			Debug.Log(horizontalMovement);
			verticalMovement=Random.Range(-1,2);
			Debug.Log(verticalMovement);
			directionchange=movementChangeTime;
		}else{
			directionchange--;
		}
		
		//initial valocity 0
		Vector2 velocity = Vector2.zero;
		//arrow keys change horizontal velocity
		velocity += Vector2.right*horizontalMovement*speed*Time.deltaTime;
		//arrow keys change vertical velocity
		velocity += Vector2.up*-verticalMovement*speed*Time.deltaTime;
		this.GetComponent<Rigidbody2D>().velocity = velocity; //set new velocity
		
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		if (collider.gameObject.GetComponent<basicShot>()){
			basicShot missile = collider.gameObject.GetComponent<basicShot>();
			if(missile){
				health -= missile.GetDamage();
				missile.Hit();
				if (health <= 0){
					Destroy(gameObject);
				}
			}
		}
	}
}
