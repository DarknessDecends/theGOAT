﻿using UnityEngine;
using System.Collections.Generic;

public class BossController : MonoBehaviour
{

    public float health = 2500;
    public float maxHealth = 2500;
    public float speed = 300;
    public int scoreWorth;
    public int topTouchDamage;
    public int bottomTouchDamage;
    public int eyeDamage;
    public float cooldown;
    public float touchKnockback;
    public List<int> roomCoords = new List<int>();
    public Vector3 resetPoint;
    public GameObject projectilePrefab;

    private AnimatorStateInfo currentBaseState;
    private Transform player;
    private bool detected;
    private Rigidbody2D rigidBody;
    private Animator animator;
    private new SpriteRenderer renderer;
    private int thrown = 0;
    private bool timeSwitched = false; //whether or not the timer has switched to being the score yet

    static int attackEnd = Animator.StringToHash("Base Layer.attackEnd");

    // Use this for initialization
    void Start()
    {
        player = PlayerController.instance.transform;
        rigidBody = this.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health/maxHealth < .25) {
            speed = 60;
            cooldown = 2.66f;
        } else if (health/maxHealth < .5) {
            speed = 40;
            cooldown = 4.33f;
        }



        detected = false;
        if (player != null)
        { //if they player exists
            if (player.position.x > roomCoords[0] && //roomCoords[0] is left x coord
                player.position.x < roomCoords[1] && //roomCoords[0] is right x coord
                player.position.y > roomCoords[2] && //roomCoords[0] is bottom y coord
                player.position.y < roomCoords[3]) //roomCoords[0] is top y coord
            {//player is in boss area
                detected = true;

                if (timeSwitched == false){ //when boss room is entered change UI to suit if it hasn't already been switched 
                    FindObjectOfType<Clock>().convertToScore(); //set players score into time
                    FindObjectOfType<ScoreKeeper>().gameObject.SetActive(false);
                    FindObjectOfType<Canvas>().transform.GetChild(3).gameObject.SetActive(true);
                    timeSwitched = true;
                }

                Vector2 dir = player.position - transform.position;
                if (thrown == 2) {
                    this.rigidBody.velocity = dir.normalized * speed * 4 * Time.deltaTime;
                } else if (thrown == 0) {
                    this.rigidBody.velocity = new Vector2(0f,0f);
                    animator.SetBool("throw", true);
                    thrown = 1;
                    Invoke("thrownTimeOut", cooldown);
                }
                currentBaseState = animator.GetCurrentAnimatorStateInfo(0);
                if (currentBaseState.fullPathHash == attackEnd && thrown == 1) {
                    animator.SetBool("throw", false);
                    thrown = 2;
                    attack(Quaternion.FromToRotation(Vector3.right, dir));
                }
            }
            if (!detected)
            { //return to spawn, reset health
                health = maxHealth;
                int roundx = Mathf.FloorToInt(resetPoint.x - transform.position.x);
                int roundy = Mathf.FloorToInt(resetPoint.x - transform.position.y);
                Vector2 dir = new Vector2(roundx,roundy);

                this.rigidBody.velocity = dir.normalized * speed * 4 * Time.deltaTime;
            }
            if (Mathf.Abs(this.rigidBody.velocity.x) > Mathf.Abs(this.rigidBody.velocity.y))
            {
                if (this.rigidBody.velocity.x > 0)
                {
                    animator.SetBool("isLeft", true);
                    animator.SetBool("isUp", false);
                    animator.SetBool("isDown", false);
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else {
                    animator.SetBool("isLeft", true);
                    animator.SetBool("isUp", false);
                    animator.SetBool("isDown", false);
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
            else {
                if (this.rigidBody.velocity.y > 0)
                {
                    animator.SetBool("isLeft", false);
                    animator.SetBool("isUp", true);
                    animator.SetBool("isDown", false);
                }
                else {
                    animator.SetBool("isLeft", false);
                    animator.SetBool("isUp", false);
                    animator.SetBool("isDown", true);
                }
            }
        }
    }

    public void hurt(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            player.GetComponentInParent<PlayerController>().Score(scoreWorth);
            Destroy(gameObject);
        }
        else {
            renderer.color = Color.red;
            Invoke("hitTimeOut", 0.03f);
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (detected && collider.transform == player)
        { //if enemy sees player an is touching him
                PlayerController foundPlayer = collider.gameObject.GetComponent<PlayerController>(); ;
                foundPlayer.hurt(Random.Range(bottomTouchDamage, topTouchDamage + 1)); //hit the player
                Vector2 atPlayer = (foundPlayer.transform.position - this.transform.position).normalized * touchKnockback;

                foundPlayer.GetComponent<Rigidbody2D>().AddForce(atPlayer, ForceMode2D.Impulse); //.velocity += atPlayer; //apply knockback
                Debug.Log(atPlayer);
        }
    }

    void hitTimeOut() {
        renderer.color = Color.white;
    }

    void thrownTimeOut() {
        thrown = 0;
    }

    public void attack(Quaternion angle) {
        //make new eye
        Projectile shot = (Instantiate(projectilePrefab) as GameObject).GetComponent<Projectile>();
        shot.transform.position = transform.position;
        shot.transform.rotation *= angle; //add the angles (some prefabs have rotation.y == -45)
        shot.damage = this.eyeDamage; //set damage
                                   //accelerate bullet
        Vector2 shotVector = angle * Vector3.right * speed; //rotate "right" vector by angle
        shot.GetComponent<Rigidbody2D>().AddForce(shotVector, ForceMode2D.Impulse);
    }
}