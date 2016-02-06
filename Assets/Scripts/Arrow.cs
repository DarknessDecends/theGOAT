using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
	
	[HideInInspector]
	public float damage;

	void Update() {
		//Physics2D.Raycast(transform.position, prevPosition, Vector2.Distance(transform.position, prevPosition), LayerMask.NameToLayer("Wall") | LayerMask.NameToLayer("Enemy"));
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
