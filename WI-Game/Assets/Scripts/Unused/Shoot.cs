using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
	public GameObject projectilePrefab;
	public Camera viewCamera;
	public float shootSpeed = 15.0f;

	private Ray ray;
	private GameManager gameManager;
	// Start is called before the first frame update
	void Start()
	{
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Fire1") && gameManager.isGameActive)
		{
			ShootBullet();
		}
	}

	private void ShootBullet()
	{
		// Create a ray from the camera going through the middle of your screen
		float x = Screen.width / 2;
		float y = Screen.height / 2;
		ray = viewCamera.ScreenPointToRay(new Vector3(x, y));
		// Debug.DrawRay(ray.origin, ray.direction * 17.425f, Color.yellow);

		// Check whether your are pointing to something so as to adjust the direction
		RaycastHit hit;
		Vector3 targetPoint;
		if (Physics.Raycast(ray, out hit))
			targetPoint = hit.point;
		else
			targetPoint = ray.GetPoint(1000);

		GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
		bullet.GetComponent<Rigidbody>().velocity = (targetPoint - transform.position).normalized * shootSpeed;
 	}
}
