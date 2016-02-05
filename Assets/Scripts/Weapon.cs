using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public int damage;
	public GameObject WeaponPrefab;
	public bool ranged;
	
	//method that returns the weapons damage
	public int getDamage(){
		return damage;
	}
	
	public void killObject(){
		Destroy(gameObject);
	}
	
}
