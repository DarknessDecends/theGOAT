using UnityEngine;
using UnityEngine.UI;
using System;

public class Clock : MonoBehaviour {
	public float totalTime;
	public float time;

	private const int musicBPM = 88; //Beats Per Minute
	private Text text;
	private LevelManager levelManager;
	

	// Use this for initialization
	void Start () {
		levelManager = GameObject.FindObjectOfType<LevelManager>();
		time = 60f * totalTime; //60 seconds per minute
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		//match music Beats Per Minute
		//time -= Time.deltaTime*(musicBPM/60f); //decrement by musicBPM (e.g. 88 clock ticks per second)

		//Match actual time, not music
		time -= Time.deltaTime; //decrement time
		if (time <= 0) {
			levelManager.resetLevel();
		}
		text.text = String.Format("{0:0}:{1:00}", Mathf.Floor(time/60), time % 60);
	}
}
