using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Deaths : MonoBehaviour {

    private PlayerController player;
    Text text;

    // Use this for initialization
    void Start() {
        player = PlayerController.instance;
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        text.text = "Deaths: " + player.deaths;
    }
}
