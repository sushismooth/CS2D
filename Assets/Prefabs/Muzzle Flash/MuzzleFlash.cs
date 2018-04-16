using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour {
	SpriteRenderer mySpriteRenderer;
	public float duration;

	// Use this for initialization
	void Start () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		transform.localScale = new Vector3 (Random.Range (0.8f, 1.2f),Random.Range (0.8f, 1.2f),1);
		mySpriteRenderer.color = new Color (1,1,1,Random.Range(0.5f,1.0f));
	}
	
	// Update is called once per frame
	void Update () {
		duration -= Time.deltaTime;
		if (duration <= 0) {
			Destroy (this.gameObject);
		}
	}
}
