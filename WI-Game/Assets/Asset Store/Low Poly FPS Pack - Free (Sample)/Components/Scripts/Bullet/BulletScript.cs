using UnityEngine;
using System.Collections;

// ----- Low Poly FPS Pack Free Version -----
public class BulletScript : MonoBehaviour {

	[Range(5, 100)]
	[Tooltip("After how long time should the bullet prefab be destroyed?")]
	public float destroyAfter;
	[Tooltip("If enabled the bullet destroys on impact")]
	public bool destroyOnImpact = false;
	[Tooltip("Minimum time after impact that the bullet is destroyed")]
	public float minDestroyTime;
	[Tooltip("Maximum time after impact that the bullet is destroyed")]
	public float maxDestroyTime;

	[Header("Impact Effect Prefabs")]
	public Transform [] metalImpactPrefabs;

	[Header("Out of Bounds Range")]
	public float outBoundsXZ = 20.0f;
	public float outBoundsTop = 9.0f;
	public float outBoundsBot = -2.0f;

	private GameManager gameManager;

	private void Awake()
	{
		Physics.IgnoreLayerCollision(15, 16, true);
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
	}

	private void Start ()
	{
		//Start destroy timer
		StartCoroutine (DestroyAfter ());
	}

	private void Update ()
	{
		Physics.IgnoreLayerCollision(15, 16, true);
	}

	// private void OnTriggerEnter(Collider other)
	// {

	// 	Debug.Log("trigger: " + other.transform.root.gameObject.name);
	// 	//If bullet collides with "Enemy" tag
	// 	if (other.transform.tag == "Enemy")
	// 	{
	// 		other.transform.gameObject.GetComponent
	// 			<EnemyController>().isHit = true;
	// 		gameManager.UpdateScore(1);
	// 		Destroy(gameObject);
	// 	}
	// }

	//If the bullet collides with anything
	private void OnCollisionEnter (Collision collision)
	{
		//If destroy on impact is false, start
		//coroutine with random destroy timer
		if (!destroyOnImpact)
		{
			StartCoroutine (DestroyTimer ());
		}
		//Otherwise, destroy bullet on impact
		else
		{
			Destroy (gameObject);
		}

		//If bullet collides with "Metal" tag
		if (collision.transform.tag == "Metal")
		{
			//Instantiate random impact prefab from array
			Instantiate (metalImpactPrefabs [Random.Range
				(0, metalImpactPrefabs.Length)], transform.position,
				Quaternion.LookRotation (collision.contacts [0].normal));
			//Destroy bullet object
			Destroy(gameObject);
		}

		//If bullet collides with "Target" tag
		if (collision.transform.tag == "Target")
		{
			//Toggle "isHit" on target object
			collision.transform.gameObject.GetComponent
				<TargetScript>().isHit = true;
			//Destroy bullet object
			Destroy(gameObject);
		}

		if (collision.transform.tag == "Prop")
		{
			Instantiate (metalImpactPrefabs [0], transform.position,
				Quaternion.LookRotation (collision.contacts [0].normal));

			Destroy(gameObject);
		}

		if (collision.transform.tag == "Enemy Walk" || collision.transform.tag == "Enemy Run")
		{
			EnemyController controller = collision.transform.gameObject.GetComponent<EnemyController>();
			controller.HitFor(gameManager.bulletDamage);

			if (controller.armor != 0)
			{
				Instantiate (metalImpactPrefabs [1], transform.position,
					Quaternion.LookRotation (collision.contacts [0].normal));
			}

			Destroy(gameObject);
		}

		if (collision.transform.tag == "Enemy Drone")
		{
			collision.transform.gameObject.GetComponent
				<DroneController>().HitFor(gameManager.bulletDamage);

			Instantiate (metalImpactPrefabs [1], transform.position,
				Quaternion.LookRotation (collision.contacts [0].normal));

			Destroy(gameObject);
		}

		//If bullet collides with "ExplosiveBarrel" tag
		if (collision.transform.tag == "ExplosiveBarrel")
		{
			//Toggle "explode" on explosive barrel object
			collision.transform.gameObject.GetComponent
				<ExplosiveBarrelScript>().explode = true;
			//Destroy bullet object
			Destroy(gameObject);
		}
	}

	private IEnumerator DestroyTimer ()
	{
		//Wait random time based on min and max values
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));
		//Destroy bullet object
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter ()
	{
		//Wait for set amount of time
		yield return new WaitForSeconds (destroyAfter);
		//Destroy bullet object
		Destroy (gameObject);
	}
}
// ----- Low Poly FPS Pack Free Version -----