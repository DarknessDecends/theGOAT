using UnityEngine;
using System.Collections;

public class BasicStaffPickup : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PlayerController.instance.deaths != 0) {
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
