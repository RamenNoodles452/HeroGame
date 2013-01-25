using UnityEngine;
using System.Collections;
//using System.Collections.generic;

[RequireComponent(typeof(CharacterController))]

public class HeroController : MonoBehaviour {
	
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip jumpAnimation;
	public AnimationClip dodgeAnimation;
	
	public float walkAnimationMaxSpeed = 1.75f;
	public float trotAnimationMaxSpeed = 1.0f;
	public float runAnimationMaxSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;
	public float dodgeAnimationSpeed = 1.0f;
	
	private Animation _animation;
	
	public enum CharacterState
	{
		Idle = 0,
		Trotting = 2,
		Running = 3,
		Jumping = 4,
		Attacking = 5,
		Dodging = 6,
		Blocking = 7
	};
	
	private CharacterState _characterState;
	
	//after trotAfterSeconds of walking, we trop with trotSpeed
	float trotSpeed = 6.0f;
	// when pressing "Fire3" button (cmd) we start running
	float runSpeed = 9.0f;
	
	float inAirControlAcceleration = 3.0f;
	
	// how high do we jump pressing jump and letting go immediately
	float jumpHeight = 4.0f;
	// we add _extraJumpHeight meters on top when holding the button down longer while jumping
	float extraJumpHeight = 2.5f;
	
	// the gravity of the character
	float gravity = 20.0f;
	// the gravity in controlled descent mode
	float controlledDescentGravity = 2.0f;
	float speedSmoothing = 10.0f;
	float rotateSpeed = 500.0f;
	float trotAfterSeconds = 3.0f;
	
	bool canJump = true;

	private float jumpRepeatTime = 0.05f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;
	
	// The camera doesnt start following the target immediately but waits for a split second to avoid too much waving around.
	private float lockCameraTimer = 0.0f;
	
	// The current move direction in x-z
	private Vector3 moveDirection = Vector3.zero;
	// Direction for when the character does a dodge roll
	private Vector3 dodgeDirection = Vector3.zero;
	//direction for when the character 
	private Vector3 targetDirection = Vector3.zero;
	
	// The current vertical speed
	private float verticalSpeed = 0.0f;
	// The current x-z move speed
	private float moveSpeed = 0.0f;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 
	
	// Are we jumping? (Initiated with jump button and not grounded yet)
	private bool jumping = false;
	private bool jumpingReachedApex = false;
	
	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;
	// Is the user pressing any keys?
	private bool isMoving = false;
	// Last time the jump button was clicked down
	private float lastJumpButtonTime = -10.0f;
	// Last time we performed a jump
	private float lastJumpTime = -1.0f;
	
	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	
	private Vector3 inAirVelocity = Vector3.zero;
	
	private float lastGroundedTime = 0.0f;
	
	private bool isControllable = true;
	
	private Vector3 targetPoint;
	
	private Vector3 dodgeStartPos;
	private float dodgeDistance = 4.0f;
	
	private Vector3 attackStartPos;
	private float attackDistance = 2.0f;
	private bool hasTarget;
	
	// Use this for initialization
	void Start () 
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		dodgeDirection = transform.TransformDirection(Vector3.forward);
	}
	
	void UpdateSmoothedMovementDirection ()
	{
		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded();
		
		
		//forward vector relative to the camera along the xz plane
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
		
		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");
		
		// are we moving backwards or looking backwards
		if(v < -0.2f)
		{
			movingBack = true;
		}
		else
		{
			movingBack = false;		
		}
		
		bool wasMoving = isMoving;
		isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
		
		// target direction relative to the camera
		Vector3 targetDirection = h * right + v * forward;
		targetPoint = 5.0f * h * right + 5.0f * v * forward;
		
		//grounded controls
		if(grounded)
		{
			//lock camera for short period when transitioning moving and standing still
			lockCameraTimer += Time.deltaTime;
			if(isMoving != wasMoving)
			{
				lockCameraTimer = 0.0f;	
			}
			
			//we store speed and direction seperately
			// so that when the character stands still we still have a valid forward direction
			// move direction is always normalized, and we only update it if there is user input
			if(targetDirection != Vector3.zero)
			{

				// otherwise smoothly turn towards target direction
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = moveDirection.normalized;
			
			}
			
			//smooth the speed based on the current target direction
			float curSmooth = speedSmoothing * Time.deltaTime;
			
			//chose target speed
			//* We want to support analog input but make sure you can't walk faster diagonally than just forward or sideways
			float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			
			if(_characterState != CharacterState.Dodging && _characterState != CharacterState.Attacking && _characterState != CharacterState.Blocking)
			{
				_characterState = CharacterState.Idle;
				
				// pick speed modifier
				if(Input.GetKey(KeyCode.LeftShift))
				{
					targetSpeed *= runSpeed;
					_characterState = CharacterState.Running;
				}
				else if(IsMovingHero())
				{
					targetSpeed *= trotSpeed;
					_characterState = CharacterState.Trotting;
				}

				moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
				
			}
		}
		
		//in air controls
		else
		{
			// lock camera while in air
			if(jumping)
			{
				lockCameraTimer = 0.0f;	
			}
			
			if(isMoving)
			{
				inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;	
			}
		}
	}
	
	void ApplyGravity()
	{
		// herp derp only move them if your character is actually controllable
		if(isControllable) 
		{
			//apply gravity
			bool jumpButton = Input.GetButton("Jump");
			
			// When we reach the apex of the jump we send out a message
			if(jumping && !jumpingReachedApex && verticalSpeed <= 0.0f)
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			
			if(IsGrounded())
			{
				verticalSpeed = -10.0f;	
			}
			else
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
		}
	}
	
	void ApplyJumping()
	{
		if(lastJumpTime + jumpRepeatTime > Time.time)
		{
			return;	
		}
		if (IsGrounded())
		{
			// Jump
			// only when pressing the button down
			// with a timeout so you can press the button slightly before landing
			if(canJump && Time.time < lastJumpButtonTime + jumpTimeout)
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed for the character to reach at the apex
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}
		
		if (Input.GetButtonDown ("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		
		UpdateSmoothedMovementDirection();
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity ();
		
		// Apply jump logic
		ApplyJumping();
		
		// Calculate actual motion
		
		Vector3 movement = moveDirection * moveSpeed + new Vector3(0.0f, verticalSpeed, 0.0f) + inAirVelocity;
		movement *= Time.deltaTime;
		//Debug.DrawLine(transform.position, dodgeDirection, Color.blue, 1.0f);
		if(_characterState == CharacterState.Dodging)
		{
			movement = dodgeDirection * 10.0f + new Vector3(0.0f, verticalSpeed, 0.0f) + inAirVelocity;
			movement *= Time.deltaTime;

			if(Vector3.Distance(dodgeStartPos, transform.position) >= dodgeDistance)
			{
				_characterState = CharacterState.Idle;
			}
		}
		else if(_characterState == CharacterState.Attacking)
		{
			movement = targetDirection * 8.0f + new Vector3(0.0f, verticalSpeed, 0.0f) + inAirVelocity;
			movement *= Time.deltaTime;
			
			if(Vector3.Distance(attackStartPos, transform.position) >= attackDistance)
			{
				_characterState = CharacterState.Idle;
				hasTarget = false;
			}
		}
		else if(_characterState == CharacterState.Blocking)
		{
			movement = Vector3.zero + new Vector3(0.0f, verticalSpeed, 0.0f) + inAirVelocity;
			movement *= Time.deltaTime;
		}
		//Debug.Log(_characterState);
		
		// move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		// ANIMATION SECTOR!
		
		// rotate the character to the move direction
		if(IsGrounded())
		{
			if(_characterState == CharacterState.Idle || _characterState == CharacterState.Trotting || _characterState == CharacterState.Running || _characterState == CharacterState.Jumping)
			{
				transform.rotation = Quaternion.LookRotation(moveDirection);
			}
			else if(_characterState == CharacterState.Attacking && hasTarget)
			{
				transform.rotation = Quaternion.LookRotation(targetDirection);
				moveDirection = targetDirection;
			}
			else if(_characterState == CharacterState.Dodging)
			{
				transform.rotation = Quaternion.LookRotation(dodgeDirection);
				moveDirection = dodgeDirection;
			}
		}
		else
		{
			Vector3 xzMove = movement;
			xzMove.y = 0.0f;
			if(xzMove.sqrMagnitude > 0.001)
			{
				transform.rotation = Quaternion.LookRotation(xzMove);	
			}
		}
		
		
		// we are in jump mode, but just became grounded
		if(IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if(jumping)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
		//Debug.Log((collisionFlags & CollisionFlags.CollidedBelow));
		
	}
	
	bool IsGrounded () 
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	public bool IsJumping ()
	{
		return jumping;
	}
	
	public bool IsMovingHero()
	{
		if(Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
		{
			return true;	
		}
		return false;
	}
	
	public Vector3 GetDirection () 
	{
		//return targetPoint;
		return targetPoint.normalized * 2.0f;
	}
	
	public Vector3 GetPosition()
	{
		return transform.position;
	}
	public void SetDodgeParameters(Vector3 direction, Vector3 position)
	{
		dodgeDirection = direction;
		dodgeStartPos = position;
	}
	public void SetTargetParameters(Vector3 direction, Vector3 position, bool hasTargetInput)
	{
		targetDirection = direction;
		attackStartPos = position;
		hasTarget = hasTargetInput;
	}
	public void ChangeState(int state)
	{
		if(state == 0)
		{
			_characterState = CharacterState.Idle;	
		}
		else if(state == 6)
		{
			_characterState = CharacterState.Dodging;
		}
		else if(state == 5)
		{
			_characterState = CharacterState.Attacking;
		}
		else if(state == 7)
		{
			_characterState = CharacterState.Blocking;	
		}
	}
	public bool DoneDodging()
	{
		return (Vector3.Distance(dodgeStartPos, transform.position) >= dodgeDistance);
	}
	
	public bool DoneAttacking()
	{
		return (Vector3.Distance(attackStartPos, transform.position) >= attackDistance);
	}
	public CharacterState GetCharacterState()
	{
		return _characterState;
	}	
}
