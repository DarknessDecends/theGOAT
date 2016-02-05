﻿using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	public float health = 500;
	public float speed = 500;
	public int movementChangeTime=500;
	public int detectionRange;
	public Transform player;
	
	private int directionchange = 0;
	private int horizontalMovement;
	private int verticalMovement;
	private Rigidbody2D rigidBody;
	
	void Start() {
		rigidBody = this.GetComponent<Rigidbody2D>();
	}

	void Update () {
		bool detected = false;
		if (Vector3.Distance(player.position, transform.position) <= detectionRange) {
			Vector2 dir = player.position - transform.position;
			RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRange);
			Debug.DrawLine(transform.position, hit.point, Color.red);
			if (hit.transform == player) {
				detected = true;
				this.rigidBody.velocity = dir.normalized*speed*4*Time.deltaTime;
			}
		}
		if (!detected) {
			if (directionchange == 0) {
				horizontalMovement=Random.Range(-1, 2);
				verticalMovement=Random.Range(-1, 2);
				directionchange=movementChangeTime;
			} else {
				directionchange--;
			}

			//initial valocity 0
			Vector2 velocity = Vector2.zero;
			//arrow keys change horizontal velocity
			velocity += Vector2.right*horizontalMovement*speed*Time.deltaTime;
			//arrow keys change vertical velocity
			velocity += Vector2.up*-verticalMovement*speed*Time.deltaTime;
			this.rigidBody.velocity = velocity; //set new velocity
		}
	}
	
	public void hurt(float damage) {
		health -= damage;
		if (health <= 0) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D collider){
		
	}
}
