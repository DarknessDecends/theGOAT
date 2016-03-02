using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHealthMeter : MonoBehaviour {

    Text bossHealth;
    private BossController boss;

    // Use this for initialization
    void Start() {
        boss = GameObject.FindObjectOfType<BossController>();
        bossHealth = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (boss.health == 0 || boss == null) {
            bossHealth.text = "Boss Health: DEAD!";
        } else {
            bossHealth.text = "Boss Health: " + boss.health;
        } //end if
    } //end Update
}
