﻿using UnityEngine;
using System.Collections;


// This script describes unit function and it's attributes.
public class UnitController : MonoBehaviour 
{

	// The firing range of our unit. Range = 50.0f
	private float rangeSqr = 2500.0f;

	// The firing rate of our unit. How many seconds between attacks.
	private float firingRate = 2.0f;
	// How many time passed from last firing.
	private float timePassed = 0.0f;
	
	// Time needed for rotation so the unit faces enemy.
	private float damping = 2.0f;

	// The life points of the unit.
	private float hitPoints = 100.0f;

	// The strength of the attack of the unit.
	private float attackForce = 10.0f;

	// The gameobject of a shot.
	public GameObject shot;

	// The transform object where to spawn shots.
	public Transform shotSpawn;


	// The targeted unit which unit wants to attack.
	private GameObject target;


	// Unit navigation agent.
	private NavMeshAgent navAgent;
	
	void Start () 
	{
		this.navAgent = this.GetComponent<NavMeshAgent> ();
	}

	void Update () 
	{
		timePassed += Time.deltaTime;
	
	}

	void FixedUpdate ()
	{
		// We have to call an attack function - we must follow unit and attack it repeatedly.
		if (target) 
		{
			attack (target);
		}
	}

	public void setTarget (GameObject go)
	{
		this.target = go;
	}

	// Moves unit into desired position.
	public void moveTo (Vector3 pos)
	{
		navAgent.SetDestination (pos);
	}

	public void fire ()
	{
		GameObject firedShot;
		firedShot = Instantiate(shot, shotSpawn.position, this.transform.rotation) as GameObject;
		firedShot.GetComponent<Shot>().setOwner(this.gameObject);
		firedShot.GetComponent<Shot>().fire(this.attackForce);
	}

	public void attack (GameObject go)
	{
		if (go != null)
		{
			float distanceSqr = (this.transform.position - go.transform.position).sqrMagnitude;
			// Enemy is in the firing range.
			if (distanceSqr < this.rangeSqr) 
			{
				this.navAgent.ResetPath ();
				// Rotate towards enemy.
				Quaternion lookAtRotation = Quaternion.LookRotation
					(go.transform.position - this.transform.position);
				this.transform.rotation = Quaternion.Slerp
					(this.transform.rotation, lookAtRotation, Time.deltaTime/damping);
				// Alternative is:
				//this.transform.LookAt(go.transform.position, Vector3.back);
				
				// Do some firing.
				if (this.timePassed > this.firingRate)
				{
					this.timePassed = 0.0f;
					this.fire ();
				}
				
			}
			// Follow the enemy!
			else
			{
				this.moveTo(go.transform.position);
			}
		}

	}

	public float getAttackForce ()
	{
		return this.attackForce;
	}

	public float getHitPoints ()
	{
		return this.hitPoints;
	}

	public void setHitPoints (float f)
	{
		this.hitPoints = f;
	}

	// Called when unit reach below 0 hp.
	// Destroys object and does some preparation
	// plus explosions later.
	public void die ()
	{
		GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		gc.givePointsForKilling(this.transform.gameObject);
		gc.deleteUnit(this.transform.gameObject);
		foreach (Transform child in this.transform)
		{
			Destroy(child.gameObject);
		}
		Destroy(this.transform.gameObject);
	}
}
