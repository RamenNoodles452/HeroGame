using UnityEngine;
using System.Collections;

public class DeleteRagdoll : MonoBehaviour {
	
	private float deathTime;
	
	// Use this for initialization
	void Start () 
	{
		deathTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(deathTime + 5.0f < Time.time)
		{
			Destroy(gameObject);	
		}
	}
}
