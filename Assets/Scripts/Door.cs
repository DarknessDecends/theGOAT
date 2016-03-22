using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	public bool lockOnEnter;

	private bool locked;
	private new BoxCollider2D collider;
	private Animator animator;

	void Start() {
		collider = GetComponent<BoxCollider2D>();
		animator = GetComponent<Animator>();
		locked = false;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (locked == false) {
			PlayerController player = other.gameObject.GetComponent<PlayerController>();
			if (player != null) {

				collider.isTrigger = true;
				animator.SetBool("open", true);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		PlayerController player = other.GetComponent<PlayerController>();
		if (player != null) {

			collider.isTrigger = false;
			animator.SetBool("open", false);

			//translate other's postion into the door's local space
			float localY = transform.InverseTransformPoint(other.transform.position).y;

			//check if other's y is above the door's y
			if (lockOnEnter && localY > 0) {
				locked = true;
			}
		}
	}

}
