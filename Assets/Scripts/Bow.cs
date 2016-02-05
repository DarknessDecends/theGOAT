using UnityEngine;
using System.Collections;

public class Bow : Weapon {
	public int arrowSpeed; //how fast the arrow travels
	public GameObject arrowPrefab;

	private float cooldownTimer;

	void Start() {
		cooldownTimer = 0; //paces bow shots
	}

	void Update() {
		Debug.Log(cooldown + ", " + cooldownTimer);
		if (cooldownTimer > 0) {
			cooldownTimer -= Time.deltaTime;
		}
	}

	override public void attack(Quaternion angle) {
		if (cooldownTimer <= 0) {
			//make new bullet
			GameObject arrow = Instantiate(arrowPrefab, transform.position, angle) as GameObject;
			//accelerate bullet
			arrow.GetComponent<Rigidbody2D>().AddForce(arrow.transform.right * 500f);
			cooldownTimer = cooldown; //reset cooldownTimer
		}
	}
	
	//method that returns the weapons damage
	public int getDamage(){
		return damage;
	}
	
	public void killObject(){
		Destroy(gameObject);
	}
	
}
