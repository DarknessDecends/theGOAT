﻿using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {
	public int level = 0;
	public int damage;
	public float cooldown;

	public abstract void attack(Quaternion angle);
}