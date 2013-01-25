using UnityEngine;
using System.Collections;

public class EnemyDamage : MonoBehaviour {
	
	int hitPoints = 3;

	Transform deadModelPrefab;
	
	private bool dead = false;
		
	void ApplyDamage(int damage)
	{
		// we've been hit, so play the 'struck' sound. This should be a metallic 'clang'.
		/*
		if (audio && struckSound)
			audio.PlayOneShot(struckSound);
		*/
		if (hitPoints <= 0)
			return;
	
		hitPoints -= damage;
		if (!dead && hitPoints <= 0)
		{
			//Die();
			dead = true;
		}
	}
	
	public bool IsDead()
	{
		return dead;	
	}
}
