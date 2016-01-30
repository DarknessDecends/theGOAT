using UnityEngine;
using System.Collections;

public class basicShot : MonoBehaviour {
	
	public float damage = 100f;
	
	public float GetDamage(){
		return damage;
	}
	
	public void Hit(){
		Destroy(gameObject);
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		Debug.Log("hit");
		if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")){
			Destroy(gameObject);
		}
	}
}
