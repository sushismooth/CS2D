using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public GameObject player;
	//speed that camera follows cursor
	public float speed = 5.0f;
	//max distance that camera can be from player
	public float viewDistance = 1;

	void Start () {
	}

	void FixedUpdate () {
		Vector3 playerPos = player.transform.position;

		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePos.z = player.transform.position.z;

		Vector3 center = (playerPos + mousePos) / 2;

		Vector3 targetPos;
		if (Vector3.Distance (playerPos, center) > viewDistance) {
			targetPos = playerPos + ((center - playerPos).normalized * viewDistance);
		} else {
			targetPos = center;
		}

		Vector3 cameraPosNoHeight = transform.position;
		cameraPosNoHeight.z = 0;

		Vector3 newCameraPosNoHeight = Vector3.Lerp(cameraPosNoHeight, targetPos, speed * Time.deltaTime);

		transform.position = new Vector3 (newCameraPosNoHeight.x, newCameraPosNoHeight.y, -10);
	}
}
