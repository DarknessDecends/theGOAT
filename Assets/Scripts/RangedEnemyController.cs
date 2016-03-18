using UnityEngine;
using System.Collections;

public class RangedEnemyController : EnemyController {

    public int projectileSpeed; //how fast the arrow travels
    public GameObject projectilePrefab;
    public int cooldown;

    private float cooldownTimer;

    void Start() {
        baseStart();
        cooldownTimer = 0; //paces bow shots
        renderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        baseUpdate();
        if (cooldownTimer > 0) {
            cooldownTimer -= Time.deltaTime;
        }
    }
  

   override protected void attack() {
        if (cooldownTimer <= 0) {
            //make new bullet
            Projectile shot = (Instantiate(projectilePrefab) as GameObject).GetComponent<Projectile>();

            Vector3 shotVector = dir.normalized; //get unit vector pointing from player to mouse

            shot.transform.position = transform.position + shotVector * 0.1f; //spawn the projectile a little bit in front of the player

            shot.transform.rotation = Quaternion.FromToRotation(Vector2.right, dir); //rotate the shot sprite by angle (some prefabs have rotation.y == -45)

            shot.damage = Random.Range(bottomDamage, topDamage + 1); //set damage

            //accelerate bullet
            shot.GetComponent<Rigidbody2D>().AddForce(shotVector * projectileSpeed, ForceMode2D.Impulse);
            cooldownTimer = cooldown; //reset cooldownTimer
        }
    }
}

