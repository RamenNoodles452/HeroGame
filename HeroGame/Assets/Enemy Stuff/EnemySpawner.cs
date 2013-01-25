using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
	public Transform target;
	
	public float spawnChance = 0.01f;
	public Vector3 spawnArea = new Vector3(9f, 5f, 9f);
	public Transform civilianSpawn;
	public EnemyBehaviour robberPrefab;
	public EnemyBehaviour copPrefab;
	public EnemyBehaviour hostileCopPrefab;
	public EnemyBehaviour civilianPrefab;
	private List<EnemyBehaviour> enemyList = new List<EnemyBehaviour>();
	
	private HeroController controller;
	private bool isSpawning = true;
	
	// Use this for initialization
	void Start () 
	{
		if(target)
		{
			controller = target.GetComponent<HeroController>();	
		}
		if(!controller)
		{
			Debug.Log("Please assign a target to the camera that has a Hero Controller script component.");
		}
	}
		
	
	void Update()
	{
		
		//Randomly spawn enemies
		if (Random.value < spawnChance)
		{
			SpawnEnemy();
		}
		/*
		if(isSpawning == true)
		{
			for(int i = 0; i < 5; i++)
			{
				SpawnEnemy();	
			}
			isSpawning = false;
		}
		*/
		/*
		for(int i = 0; i < enemyList.Count; i++)
		{
			enemyList[i].transform.LookAt(controller.transform);	
		}
		*/
		//TargetAquisition(enemyList);
	}
	
	void SpawnEnemy()
	{
		//Pick a position in a box around this object
		Vector3 pos = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
		pos.Scale(civilianSpawn.transform.position);
		
		//Spawn the enemy and put them in the right spot
		int derp = Random.Range(0, 3);
		if(derp == 0)
		{
			EnemyBehaviour e = (EnemyBehaviour)Instantiate(civilianPrefab);
			e.transform.position = transform.position + pos;
			
			//Parent them to us, so this object can receive messages on death
			e.transform.parent = transform;
		}
		if(derp == 1)
		{
			EnemyBehaviour e = (EnemyBehaviour)Instantiate(robberPrefab);
			e.transform.position = transform.position + pos;
			
			//Parent them to us, so this object can receive messages on death
			e.transform.parent = transform;
		}
		if(derp == 2)
		{
			EnemyBehaviour e = (EnemyBehaviour)Instantiate(copPrefab);
			e.transform.position = transform.position + pos;
			
			//Parent them to us, so this object can receive messages on death
			e.transform.parent = transform;
		}
		if(derp == 3)
		{
			EnemyBehaviour e = (EnemyBehaviour)Instantiate(hostileCopPrefab);
			e.transform.position = transform.position + pos;
			
			//Parent them to us, so this object can receive messages on death
			e.transform.parent = transform;
		}
	}
	/*
	void TargetAquisition(List<EnemyBehaviour> enemyList)
	{
		Vector3 targetVector = (controller.GetDirection() + controller.GetPosition());
		
		for(int i = 0; i < enemyList.Count; i++)
		{
			if(	Vector3.Distance(enemyList[i].transform.position, targetVector) <= 5.0f)
			{
				enemyList[i].SetTargeted();
			}
			else if(Vector3.Distance(enemyList[i].transform.position, targetVector) > 5.0f)
			{
				enemyList[i].SetUntargeted();
			}
		}
	}
	*/
	public List<EnemyBehaviour> GetEnemyList()
	{
		return enemyList;	
	}
	public void RemoveEnemy(EnemyBehaviour enemyToRemove)
	{
		enemyList.Remove(enemyToRemove);
	}
	
}
