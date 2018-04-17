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

	public GameObject damageTextPrefab;
	public float damageTextDuration;

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

	void OnTriggerEnter2D(Collider2D collider){
		if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacles")) {
			Destroy (this.gameObject);
		}
		if (collider.gameObject.layer == LayerMask.NameToLayer("Players")) {
			SpawnDamageText (damage, collider.transform.position, direction);
			Destroy (this.gameObject);
		}
	}

	void SpawnDamageText(int damageDone, Vector3 position, Vector3 dir){
		Vector3 randomLocation = new Vector3 (Random.Range (-0.2f, 0.2f), Random.Range (-0.2f, 0.2f), 0);
		GameObject damageText = (GameObject)Instantiate (damageTextPrefab, position + dir/2 + randomLocation, Quaternion.identity);
		DamageTextScript damageTextScript = damageText.GetComponent<DamageTextScript> ();
		damageTextScript.damage = damageDone;
		damageTextScript.duration = damageTextDuration;
	}
}
