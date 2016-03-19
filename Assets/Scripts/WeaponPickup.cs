using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collider) {
        //Weapon thisWeapon = GetComponent<Weapon>();
        //collider.gameObject.GetComponent<PlayerController>().weapons.Add(thisWeapon);
        Invoke("kill",3);
    }

    public void kill() {
        Destroy(gameObject);
    }

}
