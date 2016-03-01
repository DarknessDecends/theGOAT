using UnityEngine;
using System.Collections;

public class Ranged : Weapon{

	public int speed; //how fast the arrow travels
	public GameObject projectilePrefab;

	private float cooldownTimer;

	void Start()
	{
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
		if (cooldownTimer <= 0)
		{
			//make new bullet
			Projectile shot = (Instantiate(projectilePrefab) as GameObject).GetComponent<Projectile>();
			shot.transform.position = transform.position;
			shot.transform.rotation *= angle; //add the angles (some prefabs have rotation.y == -45)
			shot.damage = this.damage; //set damage
									   //accelerate bullet
			Vector2 shotVector = angle * Vector3.right * speed; //rotate "right" vector by angle
			shot.GetComponent<Rigidbody2D>().AddForce(shotVector, ForceMode2D.Impulse);
			cooldownTimer = cooldown; //reset cooldownTimer
		}
	}

}
