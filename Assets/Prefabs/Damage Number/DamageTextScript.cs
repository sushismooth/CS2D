using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextScript : MonoBehaviour {
	TextMesh myTextMesh;
	MeshRenderer myMeshRenderer;
	public int damage;
	public float duration;
	float startDuration;
	public float minimumCharacterSize;
	public float damageToCharacterSize;
	float alpha = 1.0f;

	// Use this for initialization
	void Start () {
		myTextMesh = GetComponent<TextMesh> ();
		myMeshRenderer = GetComponent<MeshRenderer> ();
		//if (damage == 0) {
		//	Destroy (this.gameObject);
		//}
		myMeshRenderer.sortingLayerName = "Text";
		myMeshRenderer.sortingOrder = 5;
		myTextMesh.text = damage.ToString ();
		startDuration = duration;
		myTextMesh.characterSize = minimumCharacterSize + (damage * damageToCharacterSize);
	}
	
	// Update is called once per frame
	void Update () {
		duration -= Time.deltaTime;
		alpha -= 1 / startDuration * Time.deltaTime;
		myTextMesh.color = new Color (0.1f, 0.1f, 0.1f, alpha);
		if (duration <= 0) {
			Destroy (this.gameObject);
		}
	}
}
