using UnityEngine;
using System.Collections;

public class Staff : Weapon{

    public int boltSpeed; //how fast the arrow travels
    public GameObject boltPrefab;

    private float cooldownTimer;

    void Start()
    {
        cooldownTimer = 0; //paces bow shots
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    override public void attack(Quaternion angle)
    {
        if (cooldownTimer <= 0)
        {
            //make new bullet
            MagicShot shot = (Instantiate(boltPrefab, transform.position, angle) as GameObject).GetComponent<MagicShot>();
            shot.damage = this.damage; //set damage
                                        //accelerate bullet
            shot.GetComponent<Rigidbody2D>().AddForce(shot.transform.right * boltSpeed, ForceMode2D.Impulse);
            cooldownTimer = cooldown; //reset cooldownTimer
        }
    }

    //method that returns the weapons damage
    public int getDamage()
    {
        return damage;
    }

    public void killObject()
    {
        Destroy(gameObject);
    }

}
