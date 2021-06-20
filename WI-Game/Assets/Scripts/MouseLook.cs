using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
	public Transform playerBody;
	public Transform arms;

	private float xRotation = 0f;
	private float mouseX;
	private float mouseY;
	private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
		if (gameManager.isGameActive && !PauseMenu.gameIsPaused)
		{
			Cursor.lockState = CursorLockMode.Locked;

			mouseX = Input.GetAxis("Mouse X") * gameManager.mouseSensitivity * Time.deltaTime;
			mouseY = Input.GetAxis("Mouse Y") * gameManager.mouseSensitivity * Time.deltaTime;

			xRotation -= mouseY;
			xRotation = Mathf.Clamp(xRotation, -90f, 90f);

			// Rotate camera and gun based on vertical input
			arms.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
			// Rotate the entire player based on horizontal input
			playerBody.Rotate(Vector3.up * mouseX);

			CheckRaycast();
		}
		else
			Cursor.lockState = CursorLockMode.None;
    }

	private void CheckRaycast()
	{
		Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(
			new Vector3(Screen.width/2, Screen.height/2, 0)
		);

		RaycastHit hitInfo;

		float maxDistance = 73.0f;
		float sphereRadius = 0.3f;

		int layerMask = (1 << 8) | (1 << 13) | (1 << 16);
		layerMask = ~layerMask;

		if (Physics.SphereCast(ray,
			sphereRadius,
			out hitInfo,
			maxDistance,
			layerMask))
		{
			if (hitInfo.transform.gameObject.name.Contains("Enemy"))
			{
				EnemyController controller = hitInfo.transform.gameObject.GetComponent<EnemyController>();
				gameManager.UpdateHealthbar(controller.health, controller.maxHealth, controller.type);
				gameManager.UpdateArmorbar(controller.armor, controller.maxArmor, controller.type);
				gameManager.ShowHealthbar(true);
			}
			else if (hitInfo.transform.gameObject.name.Contains("Drone"))
			{
				DroneController controller = hitInfo.transform.gameObject.GetComponent<DroneController>();
				gameManager.UpdateArmorbar(controller.armor, controller.maxArmor);
				gameManager.ShowHealthbar(true);
			}
			else
				gameManager.ShowHealthbar(false);

			if (hitInfo.transform.gameObject.CompareTag("Powerup"))
			{
				PowerupController controller = hitInfo.transform.gameObject.GetComponent<PowerupController>();
				gameManager.lookingAtPowerUp = true;
				controller.lookingAtType = controller.type;
			}
			else
				gameManager.lookingAtPowerUp = false;
		}
		// Debug.DrawRay(ray.origin, ray.direction * 50, Color.green);
	}
}
