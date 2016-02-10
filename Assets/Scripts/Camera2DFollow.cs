using System;
using UnityEngine;

	public class Camera2DFollow : MonoBehaviour {
	public Transform target;

    private static Camera2DFollow instance;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
        if (instance != this) {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	private void Start() {
	}


	// Update is called once per frame
	private void Update() {
		Vector2 mouseXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 targetXY = target.position; //convert to vector2
		Vector2 thisXY = targetXY + (mouseXY - targetXY)/6; //median between player and mouse
		transform.position = new Vector3(thisXY.x, thisXY.y, transform.position.z);
	}
}
