using UnityEngine;
using System.Collections;
using System;

public class Ranged : Weapon {
	public int speed; //how fast the arrow travels
	public GameObject projectilePrefab;

	private float cooldownTimer;

	public override void activate() {
		gameObject.SetActive(true);
		baseStart();
		cooldownTimer = 0; //paces bow shots
	}

	void Update()
	{
		if (cooldownTimer > 0)
		{
			cooldownTimer -= Time.deltaTime;
		}
	}

	override public void attack(Quaternion angle)
	{
		//Debug.Log("cooldownTimer: "+cooldownTimer+", cooldown: "+cooldown);
		if (cooldownTimer <= 0)
		{
			int numShots = Mathf.Clamp((int)(-cooldownTimer/cooldown), 1, 10);
			//generate multiple bullets if more than one cooldown cycle has passed in a single frame
			for (int i = 0; i < numShots; i++) {
				//make new bullet
				Projectile shot = (Instantiate(projectilePrefab) as GameObject).GetComponent<Projectile>();

				Vector3 shotVector = angle * Vector3.right; //get unit vector pointing from player to mouse

				shot.transform.position = transform.position + shotVector * 0.1f; //spawn the projectile a little bit in front of the player

				shot.transform.rotation *= angle; //rotate the shot sprite by angle (some prefabs have rotation.y == -45)

				shot.damage = this.damage; //set damage

				//accelerate bullet
				shot.GetComponent<Rigidbody2D>().AddForce(shotVector*speed, ForceMode2D.Impulse);
				cooldownTimer = cooldown; //reset cooldownTimer
			}
		}
	}

}