using UnityEngine;
using System.Collections;

public class WaterStaff : Ranged {

	private GameObject stream;

	// Use this for initialization
	void Start () {
		stream = Instantiate(projectilePrefab) as GameObject;
		stream.transform.parent = this.transform;
		stream.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void attack(Quaternion angle) {
		stream.SetActive(true);
		//make new bullet

		Vector2 shotVector = angle * Vector3.right; //get unit vector pointing from player to mouse

		RaycastHit2D hit = Physics2D.Raycast(transform.position, shotVector, 20);
		Debug.DrawLine(transform.position, hit.point, Color.blue);

		stream.transform.position = this.transform.position;

		stream.transform.rotation = angle * Quaternion.Euler(0, 0, -45);

		stream.transform.localScale = new Vector3(hit.distance, hit.distance, 1);
	}
}
