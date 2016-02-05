using UnityEngine;
using System;

public class Clock : MonoBehaviour {
	public float time;

	private const int musicBPM = 88; //Beats Per Minute
	private TextMesh textMesh;

	// Use this for initialization
	void Start () {
		time *= 60f; //60 seconds per minute
		textMesh = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		//match music Beats Per Minute
		time -= Time.deltaTime*(musicBPM/60f); //decrement by musicBPM (e.g. 88 clock ticks per second)

		//Match actual time, not music
		//time -= Time.deltaTime; //decrement time

		textMesh.text = String.Format("{0:0}:{1:00}", Mathf.Floor(time/60), time % 60);

	}
}
