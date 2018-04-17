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

	public AudioClip emptyclip_fire;
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
	public bool wasMouse1Pressed;

	public gun currentGun;
	int currentWeaponSlot; // 1 = primary, 2 = secondary
	int ammoInClip;
	gun primaryGun;
	int primaryGunAmmo;
	gun secondaryGun;
	int secondaryGunAmmo;
	float timeTilNextShot;
	float reloading;
	bool tooCloseToShoot;
	float minimumShootDistance = 0.6f;

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
		public string fireMode;
		public int damage;
		public int clipSize;
		public float reloadTime;
		public float timeBetweenShots;
		public float bulletSpeed;
		public float maxRange;
		public float falloffRange;
		public float falloffAmount;
		public float accuracy;
		public float recoilRate; //recoil per shot
		public float recoilRecoveryRate; //recoil recovered per second
		public AudioClip fireSoundClip;
		public AudioClip reloadSoundClip;

		public gun(){
		}

		public gun (string _weaponName, string _fireMode, int _damage, int _clipSize, float _reloadTime, float _timeBetweenShots, float _bulletSpeed, float _maxRange, 
					float _falloffRange, float _falloffAmount, float _accuracy, float _recoilRate, float _recoilRecoveryRate, AudioClip _fireSoundClip, AudioClip _reloadSoundClip){
			weaponName = _weaponName;
			fireMode = _fireMode;
			damage = _damage;
			clipSize = _clipSize;
			reloadTime = _reloadTime;
			timeBetweenShots = _timeBetweenShots;
			bulletSpeed = _bulletSpeed;
			maxRange = _maxRange;
			falloffRange = _falloffRange;
			falloffAmount = _falloffAmount;
			accuracy = _accuracy;
			recoilRate = _recoilRate;
			recoilRecoveryRate = _recoilRecoveryRate;
			fireSoundClip = _fireSoundClip;
			reloadSoundClip = _reloadSoundClip;
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
		StartingLoadout ();
	}

	void Update () {
		mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePosition.z = 0;

		GunDirection ();
		CheckCrosshairDistance ();
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
		wasMouse1Pressed = wasMouseButton1Pressed ();
	}
		
	bool wasMouseButton1Pressed(){
		if (Input.GetMouseButtonDown (0)) {
			return true;
		} else if (Input.GetMouseButtonUp (0)) {
			return false;
		} else {
			return wasMouse1Pressed;
		}
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

		myAnimator.SetInteger ("direction", playerScript.direction);
	}

	void CheckCrosshairDistance(){
		if (Vector3.Distance (gunPivot.transform.position, mousePosition) > minimumShootDistance) {
			tooCloseToShoot = false;
		} else if (Vector3.Distance (gunPivot.transform.position, mousePosition) <= minimumShootDistance){
			tooCloseToShoot = true;
		}
	}

	void Shoot (gun gun){
		if (currentGun.fireMode == "auto" || currentGun.fireMode == "semi" && !wasMouse1Pressed) {
			if (Input.GetMouseButton (0) && newGunFlicker <= gunFlickerDuration - newGunDisableDuration && !tooCloseToShoot && timeTilNextShot <= 0 && reloading <= 0) {
				if (ammoInClip > 0) {
					SpawnBullet ();
					SpawnMuzzleFlash (muzzle.transform.position, this.transform.rotation, muzzleFlashDuration, playerScript.direction);
					SpawnSoundEffect (currentGun.fireSoundClip, muzzle.transform.position, soundEffectDuration, soundEffectVolume, soundEffectPitch);
			
					ammoInClip -= 1;
					timeTilNextShot = currentGun.timeBetweenShots;
					spreadFromRecoil += currentGun.recoilRate;
					if (spreadFromRecoil > currentGun.recoilRate * 10) {
						spreadFromRecoil = currentGun.recoilRate * 10;
					}
				} else if (ammoInClip == 0 || (ammoInClip < 0 && !wasMouse1Pressed)) {
					SpawnSoundEffect (emptyclip_fire, muzzle.transform.position, soundEffectDuration, soundEffectVolume, soundEffectPitch);
					ammoInClip = -1;
				}
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
		spread = (spreadFromRecoil * spreadFromVelocity * distance/10) + ((spreadFromVelocity - 1)/3) - currentGun.accuracy;
		if (spread < 0) {
			spread = 0;
		}
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
				if (currentGun != primaryGun) {
					EquipNewGun (1);
				}
			}
			if (Input.GetKeyDown (KeyCode.Alpha2)) {
				if (currentGun != secondaryGun) {
					EquipNewGun (2);
				}
			}
		}
	}

	void EquipNewGun (int slot){
		reloading = 0;
		if (currentWeaponSlot == 1) {
			primaryGunAmmo = ammoInClip;
		} else if (currentWeaponSlot == 2) {
			secondaryGunAmmo = ammoInClip;
		}
		if (slot == 1) {
			currentGun = primaryGun;
			ammoInClip = primaryGunAmmo;
			currentWeaponSlot = 1;
		} else if (slot == 2) {
			currentGun = secondaryGun;
			ammoInClip = secondaryGunAmmo;
			currentWeaponSlot = 2;
		}
		newGunFlicker = gunFlickerDuration;
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

	void PickupNewGun(gun gun, int slot){
		if (slot == 1) {
			primaryGun = gun;
			primaryGunAmmo = gun.clipSize;
		} else if (slot == 2) {
			secondaryGun = gun;
			secondaryGunAmmo = gun.clipSize;
		}
	}

	void StartingLoadout(){
		PickupNewGun (m4a4, 1);
		PickupNewGun (ak47, 2);
		EquipNewGun (1);
	}

	void PopulateGuns (){
		m4a4 = new gun(
			"m4a4", //weapon name
			"auto", //fireMode
			25, 	//damage
			30, 	//clipSize
			2.5f,	//reloadTime
			0.1f,	//timeBetweenShots - in seconds
			0.3f,	//bulletSpeed
			30f, 	//maxRng - maximum range before bullet dissapears
			10f,	//falloffRng - range that damage starts falling off
			0.5f, 	//falloffAmt - after falloffRng, amount that damage decreases per unit travelled
			0.05f,	//accuracy - subtracted from spread
			0.25f, 	//recoilRate - amount of recoil per shot
			1.0f,		//recoilRecoveryRate - amount recoil recovers per second
			m4a4_fire, //fireSoundClip
			null //m4a4_reload //reloadSoundClip
		);
		ak47 = new gun(
			"ak47", //weapon name
			"auto", //fireMode
			30, 	//damage
			30, 	//clipSize
			2.5f,	//reloadTime
			0.125f,	//timeBetweenShots - in seconds
			0.35f,	//bulletSpeed
			30f, 	//maxRng - maximum range before bullet dissapears
			15f,	//falloffRng - range that damage starts falling off
			0.4f, 	//falloffAmt - after falloffRng, amount that damage decreases per unit travelled
			0f,	//accuracy - subtracted from spread
			0.3f, 	//recoilRate - amount of recoil per shot
			0.8f,		//recoilRecoveryRate - amount recoil recovers per second
			ak47_fire, //fireSoundClip
			null //ak47_reload //reloadSoundClip
		);
		deagle = new gun(
			"deagle", //weapon name
			"semi", //fireMode
			50, 	//damage
			7, 		//clipSize
			2f,		//reloadTime
			0.5f,	//timeBetweenShots - in seconds
			0.4f,	//bulletSpeed
			50f, 	//maxRng - maximum range before bullet dissapears
			5f,		//falloffRng - range that damage starts falling off
			1.0f, 	//falloffAmt - after falloffRng, amount that damage decreases per unit travelled
			0.0f,	//accuracy - subtracted from spread
			0.7f, 	//recoilRate - amount of recoil per shot
			0.3f,		//recoilRecoveryRate - amount recoil recovers per second
			deagle_fire, //fireSoundClip
			null //deagle_reload //reloadSoundClip
		);
	}

	void AnimationFunc(){
		if (reloading > 0) {
			myAnimator.SetBool ("reloading", true);
		} else {
			myAnimator.SetBool ("reloading", false);
		}

		if (currentGun.fireMode == "auto" || currentGun.fireMode == "semi" && !wasMouse1Pressed) {
			if (Input.GetMouseButton (0) && (ammoInClip > 0 || (ammoInClip == 0 && timeTilNextShot > 0)) && newGunFlicker <= gunFlickerDuration - newGunDisableDuration && !tooCloseToShoot) {
				myAnimator.SetBool ("shooting", true);
			} else {
				myAnimator.SetBool ("shooting", false);
			}
		} else {
			myAnimator.SetBool ("shooting", false);
		}
	}

	void UpdateUI(){
		Crosshair.scale = spread * 10;
		if (ammoInClip >= 0) {
			ammoText.text = ammoInClip.ToString ();
		} else if (ammoInClip == -1) {
			ammoText.text = "0";
		}
		clipText.text = currentGun.clipSize.ToString();
	}
}

