using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundEffectScript : MonoBehaviour {
	AudioSource myAudioSource;
	public AudioClip audioClip;
	public float volume;
	public float pitch;
	public float duration;

	// Use this for initialization
	void Start () {
		myAudioSource = GetComponent<AudioSource> ();
		myAudioSource.volume = volume;
		myAudioSource.pitch = pitch;
		myAudioSource.clip = audioClip;
		myAudioSource.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		duration -= Time.deltaTime;
		if (duration <= 0) {
			Destroy (this.gameObject);
		}
	}
}
