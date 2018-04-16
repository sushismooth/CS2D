using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	TrailRenderer myTrailRenderer;

	public float speed;
	public int damage;
	public int initialDamage;
	public float maxRange;
	public float falloffRange;
	public float falloffAmount;
	public Vector3 spawnLocation;

	public float distanceTravelled;
	public Vector3 target;
	public Vector3 direction;

	public float trailLength;

	void Start () {
		myTrailRenderer = GetComponent<TrailRenderer> ();

		initialDamage = damage;
		myTrailRenderer.time = speed * trailLength;

		transform.position = spawnLocation;
		target.z = 0;
		direction = (target - spawnLocation).normalized;

	}

	void FixedUpdate () {
		transform.position += direction * speed;
		distanceTravelled += speed;
		if (distanceTravelled > falloffRange) {
			damage = initialDamage - Mathf.RoundToInt((distanceTravelled - falloffRange) * falloffAmount);
			if (distanceTravelled > maxRange) {
				Destroy (this.gameObject);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D collider){
		if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacles")) {
			Destroy (this.gameObject);
		}
	}
}
