using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float m_JumpForce = 750f;                          // Amount of force added when the player jumps. In case of Rigidbody gravity scale is 5
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] public float m_RunSpeed = 40;
	[SerializeField] public float m_RollSpeed = 15;
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] public Animator m_Animator;
	

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	Rigidbody2D m_Rigidbody2D;
	bool m_FacingRight = true;			// For determining which way the player is currently facing.
	Vector3 m_TempMovementVelocity = Vector3.zero;
	float m_HorizontalMoveSpeed = 0f;
	bool m_Grounded;					// Whether or not the player is grounded.
	bool m_IsJumping;
	bool m_IsRolling;
	bool m_IsFalling;
	float m_InAirDuration;
	float m_RollDuration;
	PhysicsMaterial2D m_PhysicsMaterial;


	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_PhysicsMaterial = new PhysicsMaterial2D();
		m_PhysicsMaterial.name = "PlayerPhysicsMaterial2D";
		GetComponent<CircleCollider2D>().sharedMaterial = m_PhysicsMaterial;
	}

	// Update is called once per frame
	void Update () 
	{
		// Horizontal moving input
		m_HorizontalMoveSpeed = Input.GetAxisRaw("Horizontal") * m_RunSpeed;
		m_Animator.SetFloat("Speed", Mathf.Abs(m_HorizontalMoveSpeed));

		
		if (Input.GetButtonDown("Jump"))
		{
			m_IsJumping = true;
		}

		if(Input.GetButtonDown("Roll")) {
			m_IsRolling = true;
		}

		AnimationHandling();

	}

	void FixedUpdate ()
	{

		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLanding();
			}
		}

		// Adding friction the player standing still or moving
		if (m_HorizontalMoveSpeed == 0)
		{
			m_PhysicsMaterial.friction = 5;
		}
		if (m_HorizontalMoveSpeed != 0 || m_IsRolling)
		{
			m_PhysicsMaterial.friction = 0;
		}
		GetComponent<CircleCollider2D>().sharedMaterial = m_PhysicsMaterial;


		// Move our character
		Move(m_HorizontalMoveSpeed * Time.fixedDeltaTime, m_IsJumping, m_IsRolling);

		
	}

    private void LateUpdate()
    {
		m_Animator.SetBool("IsJumping", m_IsJumping);
		m_Animator.SetBool("IsRolling", m_IsRolling);
		m_Animator.SetBool("IsFalling", m_IsFalling);

	}

	public void Move(float move, bool jump, bool roll)
	{
		// If rolling
		if (roll)
		{
			// Player start rolling to the designed destination
			if (m_FacingRight)
				m_Rigidbody2D.AddForce(new Vector2(10f * m_RollSpeed , 0f));
			else
				m_Rigidbody2D.AddForce(new Vector2(-10f * m_RollSpeed, 0f));

			move = 0;
		}

		// Only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl )
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_TempMovementVelocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void OnLanding()
	{
		//Debug.Log("InAir duration was " + m_InAirDuration + " seconds long.");
		m_IsJumping = false;
		m_IsFalling = false;
		m_InAirDuration = 0f;
	}

	void AnimationHandling()
	{
		// If the player is in the air start the timer
		if (!m_Grounded)
		{
			m_InAirDuration += Time.deltaTime;
		}

		if (m_IsRolling)
		{
			m_RollDuration += Time.deltaTime;

			if (m_RollDuration > AnimationLength("Soldier-Chainmail-Roll"))
			{
				m_IsRolling = false;
				m_RollDuration = 0f;
			}
		}

		if (m_InAirDuration > AnimationLength("Soldier-Chainmail-Jump"))
		{
			if (!m_IsFalling)
			{
				m_IsJumping = false;
				m_IsFalling = true;
			}
		}

		if (m_Rigidbody2D.velocity.y < 0 && !m_Grounded)
		{
			if (!m_IsFalling)
			{
				m_IsJumping = false;
				m_IsFalling = true;
			}
		}

		if (m_InAirDuration > 5 && m_InAirDuration < 5.5)
		{
			Debug.Log("Player falling too long BRAH.");
		}
	}

	float AnimationLength(string name) {
		float time = 0;
		RuntimeAnimatorController ac = m_Animator.runtimeAnimatorController;   

		for (int i = 0; i < ac.animationClips.Length; i++)
			if (ac.animationClips[i].name == name)
				time = ac.animationClips[i].length;

		return time;
 	}
}