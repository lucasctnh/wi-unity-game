using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
	//Animator component attached to enemy
	Animator anim;

	public float speed = 3.0f;
	public float damping = 1;
	public float health;
	public float armor;
	public int type;

	public ParticleSystem explosion;
	public ParticleSystem critHighlight;

	[HideInInspector]
	public float maxHealth;
	[HideInInspector]
	public float maxArmor;

	private Vector3 transformForwardBckp;
	private bool isDead = false;
	private float roomBoundsXLeft = -12.0f;
	private float roomBoundsXRight = 14.0f;
	private float roomBoundsZBack = 13.4f;
	private float roomBoundsZFront = 78.0f;
	private bool inBoundsOnce = false;
	private bool followingPlayer;

	private Rigidbody enemyRb;
	private GameObject player;
	private GameManager gameManager;
	private SpawnManager spawnManager;

	private AudioSource[] audioSources;
	private AudioClip audioScreec;
	private float walkPitchBckp;
	private float walkPitchBckpLess;

	// Awake: Here you setup the component you are on right now (the "this" object)
	// Start: Here you setup things that depend on other components.
    private void Start()
    {
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

		anim = GetComponent<Animator>();
		audioSources = GetComponents<AudioSource>();
		audioScreec = audioSources[1].clip;
		walkPitchBckp = audioSources[0].pitch;
		walkPitchBckpLess = audioSources[0].pitch - 0.2f;
		audioSources[0].mute = true;
        enemyRb = GetComponent<Rigidbody>();
        enemyRb.centerOfMass = Vector3.zero;
        enemyRb.inertiaTensorRotation = new Quaternion(0, 0, 0, 1);

		player = GameObject.Find("Player");
		spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

    	transformForwardBckp = transform.forward;

		anim.SetBool("Walk Forward", true);

		IncreaseDifficulty(gameManager.enemyWaveStats);
    }

	private void SetMaxes()
	{
		maxHealth = health;
		maxArmor = armor;
	}

    private void FixedUpdate()
    {
		if (!gameManager.isGameActive)
		{
			anim.speed = 0;
			audioSources[0].mute = true;
		}

		if (gameManager.isGameActive && !isDead)
		{
			audioSources[0].mute = false;

			bool inBounds = (transform.position.x > roomBoundsXLeft) &&
				(transform.position.x < roomBoundsXRight) &&
				(transform.position.z > roomBoundsZBack) &&
				(transform.position.z < roomBoundsZFront);

			if (inBounds || inBoundsOnce)
			{
				Physics.IgnoreLayerCollision(7, 9, false);
				FollowPlayer();
				followingPlayer = true;
				inBoundsOnce = true;
			}
			else
			{
				Physics.IgnoreLayerCollision(7, 9, true);
				MoveForward();
				followingPlayer = false;
			}
		}

		if (!gameManager.isAudioOn || !gameManager.isGameActive)
		{
			audioSources[0].mute = true;
			audioSources[1].mute = true;
		}
		else
		{
			audioSources[0].mute = false;
			audioSources[1].mute = false;
		}
    }

	private void OnCollisionEnter(Collision other)
	{
		if (gameManager.isGameActive && other.gameObject == player && !isDead)
		{
			player.GetComponent<PlayerController>().PlayDeathSound();
			gameManager.GameOver();
		}
	}

	public void HitFor(float bulletDamage)
	{
		float dmg = bulletDamage;

		float rand = Random.value;
		if (rand < gameManager.critChance)
		{
			critHighlight.Play();
			dmg *= 2;
		}

		if (followingPlayer)
		{
			if ((armor - (dmg * gameManager.armorPiercing)) <= 0 && !isDead)
			{
				armor = 0;
				if ((health - dmg) <= 0 && !isDead)
				{
					if (audioSources[0].isPlaying)
						audioSources[0].mute = true;

					audioSources[1].Stop();
					audioSources[1].PlayOneShot(audioScreec);

					health = 0;
					StartCoroutine(DeleteEnemy());
				}
				else
				{
					if (audioSources[0].isPlaying)
						audioSources[0].mute = true;

					if (!audioSources[1].isPlaying)
						audioSources[1].PlayOneShot(audioScreec);

					health -= dmg;
				}

				gameManager.ResetLerpFactorOnLazyBar("health");
			}
			else
			{
				armor -= (dmg * gameManager.armorPiercing);

				gameManager.ResetLerpFactorOnLazyBar("armor");
			}

		}
	}

	private IEnumerator DeleteEnemy()
	{
		isDead = true;
		anim.SetTrigger("Die");

		yield return new WaitForSeconds(0.3f);
		explosion.Play();
		yield return new WaitForSeconds(0.7f);

		Destroy(gameObject);
	}

	public void MoveForward()
	{
		enemyRb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
	}

	private void FollowPlayer()
	{
		if (gameObject.CompareTag("Enemy Walk"))
			anim.SetBool("Walk Forward", true);
		else if (gameObject.CompareTag("Enemy Run"))
			anim.SetBool("Run Forward", true);

		Vector3 moveDirection = (player.transform.position - transform.position).normalized;
		moveDirection.y = enemyRb.velocity.y;
		enemyRb.AddForce(moveDirection * speed * Time.deltaTime, ForceMode.VelocityChange);

		Vector3 lookDir = player.transform.position - transform.position;
		lookDir.y = 0;
		Quaternion rotation = Quaternion.LookRotation(lookDir);
		Quaternion slerpedRotation = Quaternion.Slerp(
			transform.rotation,
			rotation,
			Time.deltaTime * damping
		);
		float angle = Quaternion.Angle(transform.rotation, slerpedRotation);
		transform.rotation = slerpedRotation;
		if (angle > 0.2f)
		{
			anim.SetBool("Walk Forward", false);
			anim.SetBool("Run Forward", false);
			Vector3 rotationDirVec = Vector3.Cross(transformForwardBckp, transform.forward);
			if (rotationDirVec.y > 0f)
			{ // Rightward rotation
				anim.SetBool("Strafe Right", true);
				audioSources[0].pitch = walkPitchBckpLess;
			}
			else if (rotationDirVec.y < 0f)
			{ // Leftward rotation
				anim.SetBool("Strafe Left", true);
				audioSources[0].pitch = walkPitchBckpLess;
			}
			transformForwardBckp = transform.forward;
		}
		else
		{
			if (gameObject.CompareTag("Enemy Walk"))
				anim.SetBool("Walk Forward", true);
			else if (gameObject.CompareTag("Enemy Run"))
				anim.SetBool("Run Forward", true);
			anim.SetBool("Strafe Right", false);
			anim.SetBool("Strafe Left", false);
			audioSources[0].pitch = walkPitchBckp;
		}

		enemyRb.WakeUp();
	}

	private void IncreaseDifficulty(List<float> enemyWaveStats)
	{
		health += enemyWaveStats[0];
		armor += enemyWaveStats[1];

		SetMaxes();
	}
}
