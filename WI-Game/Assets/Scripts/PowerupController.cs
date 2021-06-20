using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerupController : MonoBehaviour
{
	public int type;

	public float baseY = 1;
	public float maxUpAndDown = 1;
	public float  toDegrees = Mathf.PI/180;

	public TMP_Text shootToText;

	[HideInInspector]
	public int lookingAtType;

	private float speed;
	private float angle;

	private GameManager gameManager;
	private TransitionsHandler transitionsHandler;

    // Start is called before the first frame update
    void Awake()
    {
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		transitionsHandler = GameObject.Find("Loading Crossfade").GetComponent<TransitionsHandler>();
    }

	private void Start()
	{
		speed = Random.Range(15, 25);
		angle = Random.Range(0, 360);
	}

    // Update is called once per frame
    void Update()
    {
		if (gameManager.lookingAtPowerUp && lookingAtType == type)
			shootToText.gameObject.SetActive(true);
		else
		{
			lookingAtType = 0;
			shootToText.gameObject.SetActive(false);
		}

		angle += speed * Time.deltaTime;
		if (angle > 360) angle -= 360;
		float movement = baseY + (maxUpAndDown * Mathf.Sin(angle * toDegrees));
		Vector3 position = transform.position;
		position.y = movement;
		transform.position = position;
    }

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Bullet"))
		{
			Destroy(gameObject);

			switch(type)
			{
				case 1:
					gameManager.bulletDamage += 0.5f;
					break;
				case 2:
					gameManager.critChance += 0.01f;
					break;
				case 3:
					gameManager.armorPiercing += 0.1f;
					break;
			}

			gameManager.addedWave = false;
			gameManager.buyBreak = false;
			gameManager.skip = true;
			transitionsHandler.TransitionTo("Without Props");
		}
	}
}
