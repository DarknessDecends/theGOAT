using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {
	public int level = 0;
	public int damage;
	public float cooldown;
	[HideInInspector]
	public float initialCooldown; //saves the initial value of cooldown

	protected void baseStart() {
		initialCooldown = cooldown;
	}

	public abstract void activate();
	public abstract void attack(Quaternion angle);
}