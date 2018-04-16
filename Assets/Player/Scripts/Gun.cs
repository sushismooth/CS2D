using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour {

	//Components
	SpriteRenderer mySpriteRenderer;
	Animator myAnimator;

	//GameObjects
	public GameObject player;
	Player playerScript;
	public GameObject muzzle;
	public GameObject crosshair;
	public GameObject gunPivot;

	//Bullet
	public GameObject bulletPrefab;

	//MuzzleFlash
	public GameObject muzzleFlashPrefab;
	float muzzleFlashDuration = 0.03f;

	//SoundEffects
	public GameObject soundEffectPrefab;
	public float soundEffectDuration;
	public float soundEffectVolume;
	public float soundEffectPitch;

	public AudioClip m4a4_fire;
	public AudioClip ak47_fire;
	public AudioClip deagle_fire;

	//UI Elements
	public Text ammoText;
	public Text clipText;

	//Layers
	int playerLayer = 8;
	int obstacleLayer = 9;

	//Variables
	Vector3 mousePosition;

	public gun currentGun;
	int ammoInClip;
	float timeTilNextShot;
	float reloading;

	float spread;
	float spreadFromVelocity;
	float spreadFromRecoil;
	Vector3 spreadVector3;

	float newGunDisableDuration = 1f;
	float newGunFlicker;
	float newGunFlickerInterval;
	float gunFlickerDuration = 1f;
	float gunFlickerIntervalDuration = 0.25f;

	//Class for Guns
	public class gun {
		public string weaponName;
		public int damage;
		public int clipSize;
		public float reloadTime;
		public float timeBetweenShots;
		public float bulletSpeed;
		public float maxRange;
		public float falloffRange;
		public float falloffAmount;
		public float recoilRate; //recoil per shot
		public float recoilRecoveryRate; //recoil recovered per second
		public AudioClip fireSoundClip;

		public gun(){
		}

		public gun (string name, int dmg, int clip, float reloadT, float tbs, float bulletSp, float maxRng, float falloffRng, float falloffAmt, float rclRt, float rclRecov, AudioClip fireSnd){
			weaponName = name;
			damage = dmg;
			clipSize = clip;
			reloadTime = reloadT;
			timeBetweenShots = tbs;
			bulletSpeed = bulletSp;
			maxRange = maxRng;
			falloffRange = falloffRng;
			falloffAmount = falloffAmt;
			recoilRate = rclRt;
			recoilRecoveryRate = rclRecov;
			fireSoundClip = fireSnd;
		}
	}

	//Declaring Guns
	gun m4a4;
	gun ak47;
	gun deagle;

	void Start () {
		mySpriteRenderer = GetComponent<SpriteRenderer> ();
		myAnimator = GetComponent<Animator> ();
		playerScript = player.GetComponent<Player> ();

		PopulateGuns ();
		gun defaultGun = m4a4;
		EquipNewGun (defaultGun);
	}

	void Update () {
		mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePosition.z = 0;

		GunDirection ();
		Shoot (currentGun);
		Spread ();
		TimeBetweenShots ();
		Reload ();
		Reloading ();

		ChangingGuns ();
		GunFlicker ();

		AnimationFunc();
		UpdateUI();
	}

	void LateUpdate(){
		FinishChangingGun ();
	}

	void GunDirection(){
		Vector3 target = mousePosition - new Vector3 (0,0.05f,0);
		target.z = 0;

		if (playerScript.direction == -1) {
			mySpriteRenderer.flipX = true;
			gunPivot.transform.localPosition = new Vector2 (-0.02f, -0.28f);
			transform.localPosition = new Vector2 (-0.1f, -0.01f);
			gunPivot.transform.right = (target - gunPivot.transform.position) * -1;

		} else if (playerScript.direction == 1) {
			mySpriteRenderer.flipX = false;
			gunPivot.transform.localPosition = new Vector2 (0.02f, -0.28f);
			transform.localPosition = new Vector2 (0.1f, -0.01f);
			gunPivot.transform.right = target - gunPivot.transform.position;
		}
		//myAnimator.SetFloat ("animationOffset", myAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime);
		myAnimator.SetInteger ("direction", playerScript.direction);
	}

	void Shoot (gun gun){
		if (Input.GetMouseButton (0) && ammoInClip > 0 && newGunFlicker <= gunFlickerDuration - newGunDisableDuration) {
			if (timeTilNextShot <= 0 && reloading <= 0) {
				SpawnBullet ();

				ammoInClip -= 1;
				timeTilNextShot = currentGun.timeBetweenShots;
				spreadFromRecoil += currentGun.recoilRate;
				if (spreadFromRecoil > currentGun.recoilRate * 10) {
					spreadFromRecoil = currentGun.recoilRate * 10;
				}

				SpawnMuzzleFlash (muzzle.transform.position, this.transform.rotation, muzzleFlashDuration, playerScript.direction);
				SpawnSoundEffect (currentGun.fireSoundClip, muzzle.transform.position, soundEffectDuration, soundEffectVolume, soundEffectPitch);
			}
		}
	}

	void SpawnBullet(){
		GameObject bullet = (GameObject)Instantiate (bulletPrefab, transform.position, Quaternion.identity);
		Bullet bulletScript = bullet.GetComponent<Bullet> ();
		bulletScript.speed = currentGun.bulletSpeed;
		bulletScript.damage = currentGun.damage;
		bulletScript.maxRange = currentGun.maxRange;
		bulletScript.falloffRange = currentGun.falloffRange;
		bulletScript.falloffAmount = currentGun.falloffAmount;
		bulletScript.spawnLocation = muzzle.transform.position;
		bulletScript.target = mousePosition + spreadVector3;
	}

	void SpawnMuzzleFlash(Vector3 position, Quaternion rotation, float duration, int direction){
		GameObject muzzleFlash = (GameObject)Instantiate (muzzleFlashPrefab, position, rotation);
		MuzzleFlash muzzleFlashScript = muzzleFlash.GetComponent<MuzzleFlash> ();
		muzzleFlashScript.duration = duration;
		if (direction == -1) {
			muzzleFlash.GetComponent<SpriteRenderer> ().flipX = true;
		} else if (direction == 1) {
			muzzleFlash.GetComponent<SpriteRenderer> ().flipX = false;
		}
	}

	void SpawnSoundEffect (AudioClip audioClip, Vector3 position, float duration, float volume, float pitch){
		GameObject soundEffect = (GameObject)Instantiate (soundEffectPrefab, position, transform.rotation);
		soundEffectScript soundEffectScript = soundEffect.GetComponent<soundEffectScript> ();
		soundEffectScript.duration = duration;
		soundEffectScript.volume = volume;
		soundEffectScript.pitch = pitch;
		soundEffectScript.audioClip = audioClip;
	}

	void TimeBetweenShots(){
		if (timeTilNextShot > 0) {
			timeTilNextShot -= Time.deltaTime;
		}
	}

	void Reload(){
		if (Input.GetKeyDown (KeyCode.R) && ammoInClip < currentGun.clipSize && reloading <= 0) {
			reloading = currentGun.reloadTime;
		}
	}

	void Reloading(){
		if (reloading > 0) {
			reloading -= Time.deltaTime;
			if (reloading <= 0) {
				ammoInClip = currentGun.clipSize;
			}
		}
	}

	void Spread(){
		float distance = Vector3.Distance(muzzle.transform.position, mousePosition);
		spreadFromVelocity = (Vector3.Magnitude(player.GetComponent<Rigidbody2D> ().velocity)/3) + 1;
		spread = (spreadFromRecoil * spreadFromVelocity * distance/10) + ((spreadFromVelocity - 1)/3);
		spreadVector3 = new Vector3 (Random.Range (-spread, spread), Random.Range (-spread, spread), 0);

		if (spreadFromRecoil > 0) {
			spreadFromRecoil = spreadFromRecoil / (Time.deltaTime + 1) - (currentGun.recoilRecoveryRate * Time.deltaTime);
			if (spreadFromRecoil < 0) {
				spreadFromRecoil = 0;
			}
		}
	}

	void ChangingGuns(){
		if (newGunFlicker <= 0) {
			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				if (currentGun != m4a4) {
					EquipNewGun (m4a4);
				}
			}
			if (Input.GetKeyDown (KeyCode.Alpha2)) {
				if (currentGun != ak47) {
					EquipNewGun (ak47);
				}
			}
			if (Input.GetKeyDown (KeyCode.Alpha3)) {
				if (currentGun != deagle) {
					EquipNewGun (deagle);
				}
			}
		}
	}

	void EquipNewGun (gun gun){
		currentGun = gun;
		newGunFlicker = gunFlickerDuration;
		ammoInClip = currentGun.clipSize;
		myAnimator.SetBool ("changingGun", true);

		if (currentGun.weaponName == "m4a4") {
			myAnimator.SetInteger ("currentGun", 1);
		}
		if (currentGun.weaponName == "ak47") {
			myAnimator.SetInteger ("currentGun", 2);
		}
		if (currentGun.weaponName == "deagle") {
			myAnimator.SetInteger ("currentGun", 3);
		}
	}

	void GunFlicker(){
		newGunFlicker -= Time.deltaTime;
		if (newGunFlicker > 0) {
			if (newGunFlickerInterval > 0) {
				newGunFlickerInterval -= Time.deltaTime;
			} else if (newGunFlickerInterval <= 0) {
				if (mySpriteRenderer.color == new Color (1, 1, 1, 1)) {
					mySpriteRenderer.color = new Color (1, 1, 1, 0.8f);
				} else if (mySpriteRenderer.color == new Color (1, 1, 1, 0.8f))
					mySpriteRenderer.color = new Color (1, 1, 1, 1);
				newGunFlickerInterval = gunFlickerIntervalDuration;
			}
		}
		if (newGunFlicker <= 0){
			mySpriteRenderer.color = new Color (1, 1, 1, 1);
		}

	}

	void FinishChangingGun(){
		if (myAnimator.GetBool ("changingGun") == true) {
			myAnimator.SetBool ("changingGun", false);
		}
	}

	void PopulateGuns (){
		m4a4 = new gun(
			"m4a4", //weapon name
			25, 	//damage
			30, 	//clipSize
			2.5f,	//reloadTime
			0.1f,	//timeBetweenShots - in seconds
			0.3f,	//bulletSpeed
			30f, 	//maxRng - maximum range before bullet dissapears
			10f,	//falloffRng - range that damage starts falling off
			0.5f, 	//falloffAmt - after falloffRng, amount that damage decreases per unit travelled
			0.25f, 	//recoilRate - amount of recoil per shot
			1.0f,		//recoilRecoveryRate - amount recoil recovers per second
			m4a4_fire //fireSoundClip
		);
		ak47 = new gun(
			"ak47", //weapon name
			30, 	//damage
			30, 	//clipSize
			2.5f,	//reloadTime
			0.125f,	//timeBetweenShots - in seconds
			0.35f,	//bulletSpeed
			30f, 	//maxRng - maximum range before bullet dissapears
			15f,	//falloffRng - range that damage starts falling off
			0.5f, 	//falloffAmt - after falloffRng, amount that damage decreases per unit travelled
			0.4f, 	//recoilRate - amount of recoil per shot
			1.0f,		//recoilRecoveryRate - amount recoil recovers per second
			ak47_fire //fireSoundClip
		);
		deagle = new gun(
			"deagle", //weapon name
			50, 	//damage
			7, 		//clipSize
			2f,		//reloadTime
			0.5f,	//timeBetweenShots - in seconds
			0.4f,	//bulletSpeed
			50f, 	//maxRng - maximum range before bullet dissapears
			5f,		//falloffRng - range that damage starts falling off
			1f, 	//falloffAmt - after falloffRng, amount that damage decreases per unit travelled
			0.7f, 	//recoilRate - amount of recoil per shot
			0.3f,		//recoilRecoveryRate - amount recoil recovers per second
			deagle_fire //fireSoundClip
		);
	}

	void AnimationFunc(){
		if (reloading > 0) {
			myAnimator.SetBool ("reloading", true);
		} else {
			myAnimator.SetBool ("reloading", false);
		}

		if (Input.GetMouseButton (0) && (ammoInClip > 0 || timeTilNextShot > 0) && newGunFlicker <= gunFlickerDuration - newGunDisableDuration) {
			myAnimator.SetBool ("shooting", true);
		} else{
			myAnimator.SetBool ("shooting", false);
		}
	}

	void UpdateUI(){
		Crosshair.scale = spread * 10;
		ammoText.text = ammoInClip.ToString();
		clipText.text = currentGun.clipSize.ToString();
	}
}

