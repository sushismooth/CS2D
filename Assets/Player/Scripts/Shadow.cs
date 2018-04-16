using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	public GameObject player;
	Player playerScript;

	void Start () {
		playerScript = player.GetComponent<Player> ();
	}

	void Update () {
		if (playerScript.direction == -1) {
			transform.localPosition = new Vector2 (-0.025f, -0.5f);
		} else if (playerScript.direction == 1) {
			transform.localPosition = new Vector2 (0.025f, -0.5f);
		}
	}
}
