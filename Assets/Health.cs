using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public PlayerController player;
	TextMesh text;
	// Use this for initialization
	void Start () {
		text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		//text.text = "Health: " + (int)player.health;
	}
}
