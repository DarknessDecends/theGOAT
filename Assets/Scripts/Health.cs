using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour {

	private PlayerController player;
	Text text;
	
	// Use this for initialization
	void Start () {
		player = PlayerController.instance;
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Health: " + (int)player.health;
	}
}
