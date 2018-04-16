using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	public static float scale;
	SpriteRenderer mySpriteRenderer;

	void Start () {
	}

	void Update () {
		Cursor.visible = false;
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePos.z = 0;

		transform.position = mousePos;
		mySpriteRenderer.size = new Vector2 (scale/5 + 0.25f, scale/5 + 0.25f);
	}
}
