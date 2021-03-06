using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muzzle : MonoBehaviour {

	public GameObject player;
	Player playerScript;
	public GameObject gun;
	Gun gunScript;

	// Use this for initialization
	void Start () {
		playerScript = player.GetComponent<Player> ();
		gunScript = gun.GetComponent<Gun> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gunScript.currentGun.weaponName == "m4a4") {
			if (playerScript.direction == -1) {
				transform.localPosition = new Vector3 (-0.4f, 0.048f, 0);
			} else if (playerScript.direction == 1) {
				transform.localPosition = new Vector3 (0.4f, 0.048f, 0);
			}
		}
		if (gunScript.currentGun.weaponName == "ak47") {
			if (playerScript.direction == -1) {
				transform.localPosition = new Vector3 (-0.448f, 0.048f, 0);
			} else if (playerScript.direction == 1) {
				transform.localPosition = new Vector3 (0.448f, 0.048f, 0);
			}
		}
		if (gunScript.currentGun.weaponName == "deagle") {
			if (playerScript.direction == -1) {
				transform.localPosition = new Vector3 (-0.35f, 0.048f, 0);
			} else if (playerScript.direction == 1) {
				transform.localPosition = new Vector3 (0.35f, 0.048f, 0);
			}
		}
	}
}
