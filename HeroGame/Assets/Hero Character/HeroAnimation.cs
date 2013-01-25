using UnityEngine;
using System.Collections;

public class HeroAnimation : MonoBehaviour {
	
	float runSpeedScale = 1.0f;
	float dodgeSpeedScale = 2.0f;
	float lungeSpeedScale = 3.0f;
	bool wasBlocking = false;
	
	// Use this for initialization
	void Start () 
	{
		
		//by default loop all animations
		animation.wrapMode = WrapMode.Loop;
		animation["run"].layer = -1;
		animation["idle"].layer = -2;
		animation.SyncLayer(-1);
		
		// the jump animation if clamped and overrides all others.
		animation["dodge"].layer = 10;
		animation["dodge"].wrapMode = WrapMode.Once;
		
		animation["block"].layer = -1;	
		animation["block"].wrapMode = WrapMode.Loop;
		
		animation["uppercut"].speed = lungeSpeedScale;
		var uppercut = animation["uppercut"];
		uppercut.wrapMode = WrapMode.Once;
		
		//we are in full control here. Don't let any other animations play when we start
		animation.Stop();
		animation.Play("idle");
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		HeroController controller = GetComponent<HeroController>();
		HeroController.CharacterState currentState = controller.GetCharacterState();
		
		//Debug.Log(currentState);
		if(currentState == HeroController.CharacterState.Trotting)
		{
			animation.CrossFade("run");
		}
		else if(currentState == HeroController.CharacterState.Dodging)
		{
			animation.Play("dodge");
		}
		else if(currentState == HeroController.CharacterState.Blocking)
		{
			animation.CrossFade("block");
		}
		// fade out trotting
		else
		{
			animation.Blend("run", 0.0f, 0.3f);	
			animation.Blend("block", 0.0f, 0.3f);

		}
		
		animation["run"].normalizedSpeed = runSpeedScale;
		animation["dodge"].normalizedSpeed = dodgeSpeedScale;
		animation["uppercut"].normalizedSpeed = lungeSpeedScale;

	}
}
