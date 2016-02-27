using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	private AudioSource audioSource;
	private const int bpm = 176; //Beats Per Minute
	private const float spb = 60f/bpm; //Beats Per Second (multiplying this by any number of beats tells you how long those beats take to play)
	private const float firstLoopStartTime = 2*2*spb; //2 bars * 2 beats per bar * bps
	private const float firstLoopEndTime = (32*2+1)*spb; //(32 bars * 2 beats per bar + 1 beat because shostakovich is an asshole) * bps

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (audioSource.time >= firstLoopEndTime) {
			audioSource.time = firstLoopStartTime;
		}
	}
}
