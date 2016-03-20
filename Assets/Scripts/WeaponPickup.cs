using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collider) {
        Invoke("kill",3);
    }

    public void kill() {
        Destroy(gameObject);
    }

}
