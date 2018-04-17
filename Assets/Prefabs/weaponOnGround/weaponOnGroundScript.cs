using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponOnGroundScript : MonoBehaviour {

	SpriteRenderer mySpriteRenderer;
	public GameObject playerGun;
	Gun gunScript;

	public LayerMask raycastLayer;
	gun attachedGun;
	int attachedGunSlot;
	Vector3 playerLocation;
	Vector3 direction;
	public float pickupDistance;

	bool wasEPressed;
	public float ePressed;
	public float requiredEPressed;


	// Use this for initialization
	void Start () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		gunScript = playerGun.GetComponent<Gun> ();
	}

	// Update is called once per frame
	void Update () {
		checkPlayerNearby ();
		attachedGun = gunScript.m4a4;
		attachedGunSlot = 1;
	}

	void LateUpdate(){
		checkWasEPressed ();
	}

	void checkPlayerNearby(){
		playerLocation = gunScript.player.transform.position - new Vector3 (0, 0.1f, 0);;
		direction = (playerLocation - transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast (transform.position, direction, pickupDistance, raycastLayer);
		Debug.DrawLine (transform.position, playerLocation);
		if (hit.collider != null && hit.collider.gameObject.layer == 8) {
			mySpriteRenderer.color = new Color (1, 1, 1, 1);
			if (Input.GetKey (KeyCode.E) && wasEPressed) {
				ePressed += Time.deltaTime;
				if (ePressed >= requiredEPressed) {
					gunScript.PickupNewGun (attachedGun, attachedGunSlot);
					ePressed = 0;
				}
			} else {
				ePressed = 0;
			}
		} else {
			mySpriteRenderer.color = new Color (1, 1, 1, 0.5f);
			ePressed = 0;
		}
	}

	void checkWasEPressed (){
		if (Input.GetKey (KeyCode.E)) {
			wasEPressed = true;
		} else {
			wasEPressed = false;
		}
	}
}
