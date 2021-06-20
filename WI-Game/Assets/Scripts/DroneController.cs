using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneController : MonoBehaviour
{
	[Header("Circling Movement Settings")]

	[Tooltip("The delay to look at the direction of the player.")]
	public float rotationDamping = 10.0f;
	[Tooltip("Approximately the time the drone will take to reach the target.")]
	public float smoothDampTime = 3.0f;

	[Header("Straight Movement Settings")]

	[Tooltip("The distance in which the movement will change from circling to straight.")]
	public float straightFollowDistance = 5.0f;
	public float straightFollowSpeed = 4.0f;

	[Header("Vertical Movement Settings")]

	[Tooltip("The angle, in radians, that guide the vertical movement.")]
	public float sineAngle = 1.75f;
	public float minYPosition = 1.3f;
	public float maxYPosition = 6.0f;

	private float distanceReducer = 2.0f;
	private float yBaseValue = 5.0f;

	[Header("Materials")]

	[Tooltip("The material which the drone will change when phasing through objects.")]
	public Material phasingMaterial;

	[Header("Particles")]

	[Tooltip("The explosion particle that runs when drone is killed.")]
	public ParticleSystem explosion;
	[Tooltip("The particles emitted when done a critical hit.")]
	public ParticleSystem critHighlight;

	[Header("Mechanics")]
	public float armor;

	[HideInInspector]
	public float maxArmor;

	private Vector3 currentVelocity = Vector3.zero;
	private float time = 0;
	private float orientation = 1;
	private bool isDead = false;

	private Material defaultMaterial;
	private MeshRenderer rend;
	private Material[] mats;

	private GameObject player;
	private GameManager gameManager;
	private AudioSource[] audioSources;
	private AudioClip audioBoom;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
		audioSources = GetComponents<AudioSource>();
		audioBoom = audioSources[1].clip;
		audioSources[0].mute = true;
		mats = rend.materials;
		defaultMaterial = mats[0];
    }

    // Start is called before the first frame update
	private void Start()
	{
		player = GameObject.Find("Player");
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		IncreaseDifficulty(gameManager.droneWaveStat);
	}

	private void SetMaxes()
	{
		maxArmor = armor;
	}

    // Update is called once per frame
    private void FixedUpdate()
    {
		if (!gameManager.isGameActive)
			audioSources[0].mute = true;

		if (gameManager.isGameActive && !isDead)
		{
			audioSources[0].mute = false;

			Vector3 lookDir = player.transform.position - transform.position;
			Quaternion rotation = Quaternion.LookRotation(lookDir);
			Quaternion slerpedRotation = Quaternion.Slerp(
				transform.rotation,
				rotation,
				Time.deltaTime * rotationDamping
			);
			float angle = Quaternion.Angle(transform.rotation, slerpedRotation);
			transform.rotation = slerpedRotation;

			time += Time.deltaTime;
			if (time > 10.0f)
			{
				orientation = -orientation;
				time = 0;

			}

			float dist = Vector3.Distance(player.transform.position, transform.position);

			if (dist < straightFollowDistance)
			{
				Vector3 moveDirection = (player.transform.position - transform.position).normalized;
				transform.position += moveDirection * straightFollowSpeed * Time.deltaTime;
			}
			else
			{
				transform.position = Vector3.SmoothDamp(transform.position,
					player.transform.position,
					ref currentVelocity,
					smoothDampTime);

				transform.position += orientation * transform.right.normalized * Time.deltaTime * (dist/distanceReducer);
			}

			float clampedSine = (0.5f * (1 + Mathf.Sin(sineAngle * Time.time)));
			float maxY = dist/distanceReducer;
			if (maxY < minYPosition)
				maxY = minYPosition;
			if (maxY > maxYPosition)
				maxY = maxYPosition;
			transform.position = new Vector3(transform.position.x,
					Mathf.Clamp((transform.position.y + yBaseValue) * clampedSine, minYPosition, maxY),
					transform.position.z);
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
		if (!other.gameObject.CompareTag("Bullet"))
		{
			mats[0] = phasingMaterial;
			rend.materials = mats;
		}

		if (gameManager.isGameActive && other.gameObject == player && !isDead)
		{
			player.GetComponent<PlayerController>().PlayDeathSound();
			gameManager.GameOver();
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (!other.gameObject.CompareTag("Bullet"))
		{
			mats[0] = defaultMaterial;
			rend.materials = mats;
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

		if (audioSources[0].isPlaying)
			audioSources[0].mute = true;

		if ((armor - dmg) <= 0 && !isDead)
		{
			armor = 0;
			StartCoroutine(DeleteEnemy());
		}
		else
			armor -= dmg;

		gameManager.ResetLerpFactorOnLazyBar("armor");
	}

	private IEnumerator DeleteEnemy()
	{
		isDead = true;

		yield return new WaitForSeconds(0.2f);

		if (!audioSources[1].isPlaying)
			audioSources[1].PlayOneShot(audioBoom);

		explosion.Play();
		yield return new WaitForSeconds(0.3f);

		Destroy(gameObject);
	}

	private void IncreaseDifficulty(float droneWaveStat)
	{
		armor += droneWaveStat;

		SetMaxes();
	}
}
