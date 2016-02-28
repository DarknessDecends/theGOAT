using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {

    Text scoreField;
    private PlayerController player;
   
    // Use this for initialization
    void Start() {
        player = GameObject.FindObjectOfType<PlayerController>();
        scoreField = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        scoreField.text = "Score: " + player.getScore();
    }
}
