using UnityEngine;
using UnityEngine.UI;
using System;

public class Clock : MonoBehaviour {
	public float totalTime;

	[HideInInspector]
	public float time;

	private const int musicBPM = 88; //Beats Per Minute
	private Text text;
	private LevelManager levelManager;
    private PlayerController player;
	

	// Use this for initialization
	void Start () {
		levelManager = GameObject.FindObjectOfType<LevelManager>();
        player = GameObject.FindObjectOfType<PlayerController>();
        time = 60f * totalTime; //60 seconds per minute
		text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		//time -= Time.deltaTime*(musicBPM/60f); //decrement by musicBPM (e.g. 88 clock ticks per second)

		//Match actual time, not music
		time -= Time.deltaTime; //decrement time
		if (time <= 0) {
			levelManager.LoadLevel("TimeOut");
		}
		text.text = String.Format("{0:0}:{1:00}", Mathf.Floor(time/60), time % 60);
	}

    public void convertToScore() {
      time = ((int)(player.getScore()/1000));
    }
}
