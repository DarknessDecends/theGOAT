using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
	
	[HideInInspector]
	public float damage;

	void Update() {
	
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")){
			Destroy(gameObject);
		} else {
			EnemyController enemy = collider.gameObject.GetComponent<EnemyController>();
			if (enemy) {
				enemy.hurt(damage);
				Destroy(gameObject);
			}
		}
	}
}
