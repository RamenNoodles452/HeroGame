using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class EnemyBehaviour : MonoBehaviour
{
	public int health = 20;
	float maxHealth;
	public int enemy_type = 0;
	bool flee;
	bool scared;
	bool combat;
	bool theone;
	public Transform civilianDestination;
	public Transform robberDestination;
	public Transform copDestination;
	public Transform hostileCopDestination;
	Vector3 direction;
	float moveSpeed = 3.0f;
	Vector3 temp;
	Vector3 moveDirection = Vector3.zero;
	
	private AudioSource[] sounds;
	private AudioSource agh = null;
	private AudioSource charley_brown_agh = null;
	private AudioSource ow = null;
	private AudioSource ugh = null;
	private AudioSource death_by_constipation = null;
	private AudioSource block_haha = null;
	private AudioSource block_hwah = null;
	private AudioSource block_huah = null;
	private AudioSource escape = null;
	private AudioSource freakout = null;
	private AudioSource girlyscream = null;
	private AudioSource punch = null;
	private AudioSource squeakyscream = null;
	
	public GameObject ragdoll;
	private GameObject player;
	private EnemySpawner spawner;
	
	bool targeted = false;
	bool dead = false;
	Color targetedColor = new Color(1.0f, 1.0f, 0.0f);
	Color untargetedColor = new Color(1.0f, 0.0f, 0.0f);
	Color deadColor = new Color(0.5f, 0.5f, 0.5f);
	
	// The current vertical speed
	private float verticalSpeed = 0.0f;
	// the gravity of the character
	float gravity = 20.0f;
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 
	
	void Start()
	{		
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		spawner = GetComponent<EnemySpawner>();
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
		//enemytype 0=citizens, 1=robbers, 2=cops, 3=hostilecops
		if (enemy_type == 0)
		{
			//set destination of citizen
			flee = false;//near scary hero
			scared = false;//scared animation
		}
		if (enemy_type == 1)
		{
			combat = false;//in combat
			theone = false;//the one who's turn it is to attack
		}
		if (enemy_type == 2)
		{
			combat = false;//in combat
		}
		
	}
	
	void Update()
	{
		//my test crap.
		if(!dead)
		{
			if(targeted == true)
			{
				renderer.material.color = targetedColor;
			}
			else if (targeted == false)
			{
				renderer.material.color = untargetedColor;	
			}
		}
		else
		{
			renderer.material.color = deadColor;
		}
		
		ApplyGravity();
		
		//Geo's AI code
		if (enemy_type == 0)
		{
																		//civilianAI
			if (flee == false)
			{
				
				//calculate vector to get to destination
				temp = civilianDestination.transform.position - transform.position;
				direction = temp.normalized;
				moveDirection = direction;
				//transform.position = Vector3.MoveTowards(transform.position, civilianDestination.transform.position, 0.1f);
				
				//go towards destination
				
			}
			else if (flee == true)
			{
				//run opposite direction of hero
				//calculate vector from player to citizen
				temp = transform.position - player.transform.position;
				direction = temp.normalized;
				moveDirection = direction;
				//transform.position = Vector3.MoveTowards(transform.position, civilianDestination.transform.position, 10);
			}
			if (scared == true && Vector3.Distance(transform.position, player.transform.position) < 10)
			{
				flee = true;
			}
			if ( Vector3.Distance(transform.position, player.transform.position) >= 10)
			{
				flee = false;
			}
			
		}
		else if (enemy_type == 1)
		{
			//robberAI
			if (combat == false)
			{
				//head for set destination off-screen
				temp = robberDestination.transform.position - transform.position;
				direction = temp.normalized;
				moveDirection = direction;
				//transform.position = Vector3.MoveTowards(transform.position, robberDestination.transform.position, 10);
			}
			if (combat == true)
			{
				if (theone == true)
				{
					//fight
					//then send message to enemyhandler to change theone
					theone = false;
				}
				else
				{
					//circle-strafe the player
				}
			}
			
		}
		else if (enemy_type == 2)
		{
			//copAI
			if (combat == false)
			{
				//head toward robber spawn
				temp = copDestination.transform.position - transform.position;
				direction = temp.normalized;
				moveDirection = direction;
				//transform.position = Vector3.MoveTowards(transform.position, copDestination.transform.position, 10);
			}
			if (combat == true)
			{
				//shoot periodically at target
			}
			if (true)
			{
				combat = true;
				//lock on to a target
			}
			else
			{
				combat = false;
			}
		}
		else if (enemy_type == 3)
		{
			//mercAI
			if (combat == false)
			{
				//head toward player
				temp = player.transform.position - transform.position;
				direction = temp.normalized;
				moveDirection = direction;
				//transform.position = Vector3.MoveTowards(transform.position, hostileCopDestination.transform.position, 10);
			}
			else
			{
				//shoot periodically at player
			}
			if (Vector3.Distance(transform.position, player.transform.position) > 10)
			{
				combat = false;
			}
			else
			{
				combat = true;
			}
		}
		
		Vector3 movement = moveDirection * moveSpeed + new Vector3(0.0f, verticalSpeed, 0.0f);
		movement *= Time.deltaTime;
		
		// move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		if(IsGrounded())
		{
			transform.rotation = Quaternion.LookRotation(moveDirection);
		}
		
		
	}
	
	public void Damage(int amount)
	{
		if(enemy_type == 0)										//innocent citizen
		{
			scared = true;
			//set new animation
			//set new destination to be closest building
			
			//send message to all cops that civilian was attacked
			//foreach(
		}
		else if (enemy_type == 1)
		{
			//robber is caught, enter combat
			combat = true;
		}
		else if (enemy_type == 2)
		{
			//turn cop into hostile cop
			enemy_type = 3;
		}
		
		//Don't do negative damage
		if (amount <= 0)
			return;
		
		health -= amount;
		if (health <= 0)
		{
			dead = true;
			RagdollDeath();
			
			//Let our parent(s) know that we've died
			SendMessageUpwards("EnemyKilled", SendMessageOptions.DontRequireReceiver);
		}	
	}
	
	void RagdollDeath()
	{
		//kill the game object
		Destroy(gameObject);
	
		//replace it with a ragdoll
		if(ragdoll)
		{
			//var deadBody = (GameObject)Instantiate(ragdoll, transform.position, transform.rotation);
			GameObject deadBody = (GameObject)Instantiate(ragdoll, transform.position, transform.rotation);
			HeroController playerController = player.GetComponent<HeroController>();
			Vector3 impuseForce = playerController.transform.forward * 1000;
			Vector3 upwards = new Vector3(0.0f, 100.0f, 0.0f);
			deadBody.rigidbody.AddForce(impuseForce + upwards, ForceMode.Impulse);
		}
	}
	void ApplyGravity()
	{
		if(IsGrounded())
		{
			verticalSpeed = -10.0f;	
		}
		else
		{
			verticalSpeed -= gravity * Time.deltaTime;
		}
		
	}
	void ApplyBumping()
	{

	}
	bool IsGrounded () 
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	bool IsTouching()
	{
		return (collisionFlags & CollisionFlags.CollidedSides) != 0;	
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
}