using UnityEngine;
using System.Collections;

public class CivilianBehavior : MonoBehaviour {
	
	public int health = 20;
	float maxHealth;
	bool flee;
	bool scared;
	bool targeted = false;
	bool dead = false;
	Vector3 destination;
	Vector3 direction;
	Vector3 temp;
	
	private GameObject player;

	
	//sound code here
	public AudioSource[] sounds;
	public AudioSource agh = null;
	public AudioSource charley_brown_agh = null;
	public AudioSource ow = null;
	public AudioSource ugh = null;
	public AudioSource death_by_constipation = null;
	public AudioSource block_haha = null;
	public AudioSource block_hwah = null;
	public AudioSource block_huah = null;
	public AudioSource escape = null;
	public AudioSource freakout = null;
	public AudioSource girlyscream = null;
	public AudioSource punch = null;
	public AudioSource squeakyscream = null;
	
	
	
	// Use this for initialization
	void Start ()
	{
		//get sounds
		sounds = GetComponents<AudioSource>();
		
		//get player access
		player = GameObject.FindGameObjectWithTag("Player");
		
		maxHealth = health;
		
		flee = false; //civilian near scary player
		scared = false; //civilian hostile to player
		
		foreach(AudioSource temp in sounds)
		{
			if(agh == null)
			{
				agh = temp;
			}
			else if (charley_brown_agh == null)
			{
				charley_brown_agh = temp;	
			}
			else if (ow == null)
			{
				ow = temp;
			}
			else if (ugh == null)
			{
				ugh = temp;
			}
			else if (death_by_constipation == null)
			{
				death_by_constipation = temp;
			}
			else if (block_haha == null)
			{
				block_haha = temp;
			}
			else if (block_hwah == null)
			{
				block_hwah = temp;
			}
			else if (block_huah == null)
			{
				block_huah = temp;
			}
			else if (escape == null)
			{
				escape = temp;
			}
			else if (freakout == null)
			{
				freakout = temp;
			}
			else if (girlyscream == null)
			{
				girlyscream = temp;
			}
			else if (punch == null)
			{
				punch = temp;
			}
			else
			{
				squeakyscream = temp;
			}
		}
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
																		//civilianAI
			if (flee == false)
			{
				
				//calculate vector to get to destination
				temp = destination - transform.position;
				direction = temp.normalized;
				transform.position = Vector3.MoveTowards(transform.position, destination, 10);
				
				//go towards destination
				
			}
			if (scared == true && Vector3.Distance(transform.position, player.transform.position) < 10)
			{
				flee = true;
			}
			if ( Vector3.Distance(transform.position, player.transform.position) >= 10)
			{
				flee = false;
			}
			if (flee == true)
			{
				//run opposite direction of hero
				//calculate vector from player to citizen
				temp = transform.position - player.transform.position;
				direction = temp.normalized;
				transform.position = Vector3.MoveTowards(transform.position, destination, 10);
				
			
				
			}
		
	}
	
	
	public void Damage(int amount)
	{
		//Don't do negative damage
		if (amount <= 0)
			return;
		
		health -= amount;
		if (health <= 0)
		{
			dead = true;
			Destroy(gameObject);
			
			//Let our parent(s) know that we've died
			SendMessageUpwards("EnemyKilled", SendMessageOptions.DontRequireReceiver);
		}	
		
		scared = true;
			//set new animation
			//set new destination to be closest building
			
			//send message to all cops that civilian was attacked
			//foreach(
	}
	
	public void SetTargeted()
	{
		targeted = true;
	}
	public void SetUntargeted()
	{
		targeted = false;
	}
	public bool IsDead()
	{
		return dead;	
	}
	
	public void SetDestination(Vector3 ndestination)
	{
		destination = ndestination;
	}
	
}
