using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HeroController))]

public class HeroCombat : MonoBehaviour {
	
	public float dodgeRollDistance = 8.0f;
	
	public Transform enemySpawnerTarget;
	private Vector3 targetDirection = Vector3.zero;
	private Vector3 centerOffset = Vector3.zero;
	
	private HeroController controller;
	private EnemyBehaviour targetEnemy;
	
	private bool hasTarget;
	private float? targetDistance;
	
	private bool isDodging;
	private Vector3 dodgeStartPos = Vector3.zero;
	
	private bool isAttacking;
	private Vector3 attackStartPos = Vector3.zero;
	
	private bool isBlocking;
	
	float punchSpeed = 1.0f;
	float punchHitTime = 0.2f;
	//Vector3 punchPosition = new Vector3(0.0f, 2.0f, 1.0f);
	Vector3 punchPosition = new Vector3(0.0f, 0.0f, 1.0f);
	float punchRadius = 0.8f;
	int punchHitPoints = 1;
	
	
	// Use this for initialization
	void Start () 
	{
		controller = GetComponent<HeroController>();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Vector3 direction = new Vector3(controller.transform.position.x + v, controller.transform.position.y, controller.transform.position.z - h);
		if(controller.IsMovingHero())
		{
			targetDirection = (controller.GetDirection());
		}
		Debug.DrawLine (transform.position, targetDirection + controller.GetPosition(), Color.red, 1.0f);
		
		TargetAquisition();
		//Debug.Log(targetEnemy);
		if (Input.GetButtonDown ("Punch"))
		{
			if(isAttacking == false)
			{
				isAttacking = true;		
				attackStartPos = controller.transform.position;
				
				if(targetEnemy)
				{
					
					//Debug.DrawLine (Vector3.zero, something.normalized, Color.green, 1.0f);
					Vector3 enemyPosition = targetEnemy.transform.position;
					enemyPosition.y = 0.0f;
					Debug.DrawLine (controller.transform.position + new Vector3(0, 1, 0), enemyPosition + new Vector3(0, 1, 0), Color.green, 1.0f);
					Vector3 something = enemyPosition - controller.transform.position;
					something.y = 0.0f;
					controller.SetTargetParameters(something.normalized * 2.0f, attackStartPos, true);
					
				}
				else
				{
					Debug.DrawLine (controller.transform.position, targetDirection + controller.GetPosition(), Color.red, 1.0f);
					targetDirection = controller.transform.forward * 2.0f;
					controller.SetTargetParameters(targetDirection, attackStartPos, false);
				}
				controller.ChangeState(5);
			}
		}
		AttackTarget();
		if (Input.GetButtonDown("Dodge"))
		{
			if(isDodging == false)
			{
				if(isBlocking == true)
				{
					isBlocking = false;	
				}
				isDodging = true;
				dodgeStartPos = controller.transform.position;
				if(!controller.IsMovingHero())
				{
					targetDirection = controller.transform.forward * 2.0f;
				}
				controller.SetDodgeParameters(targetDirection, dodgeStartPos);
				controller.ChangeState(6);
			}
		}
		DodgeRoll();
		if (Input.GetAxisRaw("Block") < -0.1f)
		{
			if(isBlocking == false && isDodging == false)
			{
				isBlocking = true;
				controller.ChangeState(7);
				
			}	
		}
		else if(Input.GetAxisRaw("Block") > -0.1f && Input.GetAxisRaw("Block") <= 0.0f)
		{
			if(isBlocking == true)
			{
				isBlocking = false;	
				controller.ChangeState(0);
			}
		}
	}
	
	void TargetAquisition()
	{	
		bool noTarget = true;
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(GameObject tempEnemy in enemies)
		{
			var enemy = tempEnemy.GetComponent<EnemyBehaviour>();
			float tempTargetDistance = Vector3.Distance(tempEnemy.transform.position, targetDirection + controller.GetPosition());
			if(	tempTargetDistance <= 4.0f && (tempTargetDistance <= targetDistance || targetDistance == null))
			{
				if(targetEnemy != enemy && !enemy.IsDead())
				{
					targetEnemy = enemy;
					enemy.SetTargeted();
					targetDistance = tempTargetDistance;
					hasTarget = true;
				}
				noTarget = false;
				
			}
			else if(tempTargetDistance > 4.0f || tempTargetDistance > targetDistance || targetDistance == null)
			{
				enemy.SendMessage("SetUntargeted");
			}
		}
		if(noTarget == true)
		{
			hasTarget = false;	
		}
		
		if(hasTarget == false)
		{
			targetDistance = null;	
			targetEnemy = null;
		}
	}
	
	void AttackTarget()
	{
		if(isAttacking)
		{
			animation.CrossFadeQueued("uppercut", 0.1f, QueueMode.PlayNow);
			//yield WaitForSeconds(punchHitTime);
			Vector3 attackPos = transform.TransformPoint(punchPosition);
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			
			foreach(GameObject tempEnemy in enemies)
			{
				if (tempEnemy == null)
					continue;
				
				if (Vector3.Distance(tempEnemy.transform.position, attackPos) < punchRadius)
				{
					var enemy = tempEnemy.GetComponent<EnemyBehaviour>();
					enemy.SendMessage("Damage", punchHitPoints);
					// Play sound.
					/*
					if (punchSound)
						audio.PlayOneShot(punchSound);
					*/
				}
			}
			
			if(controller.DoneAttacking())
			{
				isAttacking = false;	
			}
		}
	}
	
	void DodgeRoll()
	{
		if(isDodging)
		{
			if(controller.DoneDodging())
			{
				isDodging = false;	
			}
		}
	}
	/*
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.TransformPoint(punchPosition), punchRadius);
	}
	*/
}
