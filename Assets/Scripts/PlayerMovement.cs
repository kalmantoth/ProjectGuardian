using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;
	public bool roll = false;
	float airDuration = 0f;
	float rollDuration = 0f;
	bool isFalling = false;
	
	// Update is called once per frame
	void Update () 
	{

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		
		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
			isFalling = false;
			animator.SetBool("IsJumping", true);
			Debug.Log("Jump animation is " + AnimationLength("Soldier-Chainmail-Jump") + " seconds long.");
		}

		if (Input.GetButtonDown("Crouch"))
		{
			crouch = true;
		} else if (Input.GetButtonUp("Crouch"))
		{
			crouch = false;
		}

		if(Input.GetButtonDown("Roll")) {
			roll = true;
			horizontalMove = 0;
			animator.SetBool("IsRolling", true);
			Debug.Log("Roll animation is " + AnimationLength("Soldier-Chainmail-Roll") + " seconds long.");
		}
		
		// If the player is in the air start the timer
		if(!controller.GetGrounded())
		{ 
			airDuration += Time.deltaTime;
		}

		if(roll){

			rollDuration += Time.deltaTime;

			if (rollDuration > AnimationLength("Soldier-Chainmail-Roll"))
			{
				roll = false;
				rollDuration = 0f;
				animator.SetBool("IsRolling", false);
			}
		}

		if(airDuration > AnimationLength("Soldier-Chainmail-Jump")){
			isFalling = true;
			animator.SetBool("IsJumping", false);
			animator.SetBool("IsFalling", true);
		}

		
	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, roll);
		jump = false;
		Debug.Log("airDuration iss " + airDuration);
	}

	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
		animator.SetBool("IsFalling", false);
		airDuration = 0f;
	}

	float AnimationLength(string name) {
		float time = 0;
		RuntimeAnimatorController ac = animator.runtimeAnimatorController;   

		for (int i = 0; i < ac.animationClips.Length; i++)
			if (ac.animationClips[i].name == name)
				time = ac.animationClips[i].length;

		return time;
 	}
}