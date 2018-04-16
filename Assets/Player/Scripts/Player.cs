using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	SpriteRenderer mySpriteRenderer;
	Rigidbody2D myRigidbody;
	Animator myAnimator;
	public Vector2 velocity;
	Camera cam;
	public int direction = 1;

	float moveSpeed = 30;

	void Start () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		myRigidbody = GetComponent<Rigidbody2D> ();
		myAnimator = GetComponent<Animator> ();
		cam = Camera.main;
	}

	void Update () {
		Walk ();
		Direction ();
	}

	void FixedUpdate() {
		myRigidbody.velocity += velocity * Time.fixedDeltaTime;
	}

	void Walk() {
		velocity = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical")).normalized * moveSpeed;
		if (velocity != new Vector2 (0, 0)) 
		{
			myAnimator.Play ("CT Run");
		} 
		else 
		{
			myAnimator.Play ("CT Idle");
		}
	}

	void Direction(){
		if (Input.mousePosition.x < cam.WorldToScreenPoint (myRigidbody.position).x) 
		{
			direction = -1;
			mySpriteRenderer.flipX = true;
		} 
		else if (Input.mousePosition.x > cam.WorldToScreenPoint (myRigidbody.position).x) 
		{
			direction = 1;
			mySpriteRenderer.flipX = false;
		}

	}
}
