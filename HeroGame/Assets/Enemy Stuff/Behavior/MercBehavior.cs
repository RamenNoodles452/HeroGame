using UnityEngine;
using System.Collections;

public class MercBehavior : MonoBehaviour {
	
	public int health = 20;
	float maxHealth;
	
	bool combat;
	Vector3 destination;
	Vector3 direction;
	Vector3 temp;
	
	private GameObject player;
	
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
	
	bool targeted = false;
	bool dead = false;

	// Use this for initialization
	void Start () {
		
		//Used for coloring
		maxHealth = health;
		
		//get sounds
		sounds = GetComponents<AudioSource>();
		
		//get player access
		player = GameObject.FindGameObjectWithTag("Player");
		
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
		
		maxHealth = health;
		
		combat = false;//in combat
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//mercAI
			if (combat == false)
			{
				//head toward player
			}
			else
			{
				//shoot periodically at player
			}
			if (true/*player leaves area*/)
			{
				combat = false;
			}
			if (true /*player within area*/)
			{
				combat = true;
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
