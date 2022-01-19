using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
	public class PlayerController : MonoBehaviour
	{

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
		bool m_FacingRight = true;          // For determining which way the player is currently facing.
		Vector3 m_TempMovementVelocity = Vector3.zero;
		float m_HorizontalMoveSpeed = 0f;
		bool m_Grounded;                    // Whether or not the player is grounded.
		bool m_IsJumping;
		bool m_IsRolling;
		bool m_IsFalling;
		bool m_IsAttacking;
		bool m_IsMovingEnabled;
		float m_InAirDuration;
		float m_RollDuration;
		float m_AttackDuration;

		public int m_ActualHealthPoint;
		public int m_MaxHealthPoint;
		public bool m_IsInvulnerable;



		PhysicsMaterial2D m_PhysicsMaterial;



		public AnimationClip[] m_WeaponAnimationClips;
		AnimatorOverrideController m_AnimatorOverrideController;


		Weapon m_ActiveWeapon;
		public WeaponType m_ActiveWeaponType;
		public float m_AttackAnimationModifier = 1f;


		private void Awake()
		{
			m_IsMovingEnabled = true;
			m_ActiveWeapon = Weapon.weapons[0];
			m_Rigidbody2D = GetComponent<Rigidbody2D>();
			m_PhysicsMaterial = new PhysicsMaterial2D();
			m_PhysicsMaterial.name = "PlayerPhysicsMaterial2D";
			GetComponent<Collider2D>().sharedMaterial = m_PhysicsMaterial;

			m_AnimatorOverrideController = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
			m_Animator.runtimeAnimatorController = m_AnimatorOverrideController;

			m_Animator.SetFloat("AttackAnimModifier", m_AttackAnimationModifier);


			// Ignore collision with enemy layer GO
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

		}

		void Update()
		{
			CheckPlayerHealth();
			InputHandling();
			AnimationHandling();
			m_ActiveWeaponType = m_ActiveWeapon.weaponType;
			m_ActiveWeapon = Weapon.weapons[(int)m_ActiveWeaponType];



		}

		void FixedUpdate()
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
			GetComponent<Collider2D>().sharedMaterial = m_PhysicsMaterial;


			// Move our character
			if (m_IsMovingEnabled)
				Move(m_HorizontalMoveSpeed * Time.fixedDeltaTime, m_IsJumping, m_IsRolling);

			

		}


		public void Move(float move, bool jump, bool roll)
		{
			// If rolling
			if (roll)
			{
				// Player start rolling to the designed destination
				if (m_FacingRight)
					m_Rigidbody2D.AddForce(new Vector2(10f * m_RollSpeed, 0f));
				else
					m_Rigidbody2D.AddForce(new Vector2(-10f * m_RollSpeed, 0f));

				move = 0;
			}

			// Only control the player if grounded or airControl is turned on
			if (m_Grounded || m_AirControl)
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

		public void OnEnemyHit(EnemyController enemyController)
		{
			Debug.Log(enemyController.name + " enemy hit with " + m_ActiveWeapon.attackDamage);
			enemyController.TakeDamage(m_ActiveWeapon.attackDamage);
		}

		public void TakeDamage(int damage)
        {
            if (!m_IsInvulnerable)
            {
				m_Animator.SetTrigger("HurtTrigger");
				m_ActualHealthPoint -= damage;
			}
        }

		public void CheckPlayerHealth()
        {

			if (m_ActualHealthPoint <= 0)
			{
				

				// Replace player camera with static camera scene root

				Camera playerCamera = this.GetComponentInChildren<Camera>();
				GameObject staticCamera = null;

				staticCamera = Instantiate(Resources.Load("Camera"), playerCamera.transform.position, Quaternion.identity) as GameObject;
				staticCamera.GetComponent<Camera>().CopyFrom(playerCamera);


				// NEED TO IMPLEMENT - play dying animation
				Destroy(this.gameObject);

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

		void InputHandling()
		{
			// Horizontal moving input
			m_HorizontalMoveSpeed = Input.GetAxisRaw("Horizontal") * m_RunSpeed;

			if (Input.GetMouseButtonDown(0))
			{
				if (!m_IsAttacking && !m_IsRolling && !m_IsFalling)
				{
					m_IsAttacking = true;
					m_IsMovingEnabled = false;
					m_Rigidbody2D.velocity = new Vector2(0, 0);
					//Debug.Log("Attack with " + m_ActiveWeapon.weaponName);
				}
			}
			if (Input.GetButtonDown("Jump"))
			{
				m_IsJumping = true;
			}

			if (Input.GetButtonDown("Roll"))
			{
				m_IsRolling = true;
			}

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				m_ActiveWeapon = Weapon.weapons[0];
				Debug.Log("Current weapon " + m_ActiveWeapon.weaponName);
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				m_ActiveWeapon = Weapon.weapons[1];
				Debug.Log("Current weapon " + m_ActiveWeapon.weaponName);
			}
		}



		void AnimationHandling()
		{
			// If the player is in the air start the timer
			if (!m_Grounded)
			{
				m_InAirDuration += Time.deltaTime;
			}

			// Attacking animation handling
			if (m_IsAttacking)
			{


				if (!this.m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
				{
					if (m_ActiveWeapon.weaponType == WeaponType.TWO_HANDED_SWORD)
					{
						m_AnimatorOverrideController["Player-Attack"] = m_WeaponAnimationClips[0];
					}
					else if (m_ActiveWeapon.weaponType == WeaponType.BOW)
					{
						m_AnimatorOverrideController["Player-Attack"] = m_WeaponAnimationClips[1];
					}


					this.m_Animator.SetTrigger("AttackTrigger");
				}

				float AttackAnimationLength = Util.AnimationLength(m_AnimatorOverrideController["Player-Attack"].name, m_Animator) / m_AttackAnimationModifier;

				if (m_AttackDuration < AttackAnimationLength)
				{
					m_AttackDuration += Time.deltaTime;
				}
				else if (m_AttackDuration > AttackAnimationLength)
				{
					//Debug.Log("Attack anim length is " + AttackAnimationLength + " || m_AttackDuration is " + m_AttackDuration);
					m_IsAttacking = false;
					m_IsMovingEnabled = true;
					m_AttackDuration = 0;

				}
			}

			// Rolling animation handling
			if (m_IsRolling)
			{
				m_IsInvulnerable = true;
				m_RollDuration += Time.deltaTime;

				if (m_RollDuration > Util.AnimationLength("Player-Roll", m_Animator))
				{
					m_IsRolling = false;
					m_IsInvulnerable = false;
					m_RollDuration = 0f;
				}
			}

			// Jumping to falling animation handling
			if (m_InAirDuration > Util.AnimationLength("Player-Jump", m_Animator))
			{
				if (!m_IsFalling)
				{
					m_IsJumping = false;
					m_IsFalling = true;
				}
			}

			// Falling animation handling
			if (m_Rigidbody2D.velocity.y < 0 && !m_Grounded)
			{
				if (!m_IsFalling)
				{
					m_IsFalling = true;
				}
			}

			if (m_InAirDuration > 5 && m_InAirDuration < 5.5)
			{
				Debug.Log("Player falling too long BRAH.");
			}

			// Updating Animator component values
			m_Animator.SetFloat("Speed", Mathf.Abs(m_HorizontalMoveSpeed));
			m_Animator.SetBool("IsJumping", m_IsJumping);
			m_Animator.SetBool("IsRolling", m_IsRolling);
			m_Animator.SetBool("IsFalling", m_IsFalling);
			m_Animator.SetFloat("AttackAnimModifier", m_AttackAnimationModifier);
			m_Animator.SetBool("IsAttacking", m_IsAttacking);

		}

		
	}
}
