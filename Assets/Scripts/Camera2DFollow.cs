using System;
using UnityEngine;

	public class Camera2DFollow : MonoBehaviour {
	private Transform target;

	// Use this for initialization
	private void Start() {
		target = PlayerController.instance.transform;
	}


	// Update is called once per frame
	private void Update() {
		Vector2 mouseXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 targetXY = target.position; //convert to vector2
		Vector2 thisXY = targetXY + (mouseXY - targetXY)/6; //median between player and mouse
		transform.position = new Vector3(thisXY.x, thisXY.y, transform.position.z);
	}
}
