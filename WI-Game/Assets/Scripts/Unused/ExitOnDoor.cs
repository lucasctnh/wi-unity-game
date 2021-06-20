using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ExitOnDoor : MonoBehaviour
{
	public TMP_Text pressEText;

	private GameObject player;
	private GameManager gameManager;

    // Start is called before the first frame update
	private void Start()
	{
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		// gameManager.sceneToGo = "Without Props";
	}

    void Awake()
    {
		player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
		float distance = Vector3.Distance(player.transform.position, transform.position);
		if (distance < 2.5f)
		{
			pressEText.gameObject.SetActive(true);

			if (Input.GetKeyDown (KeyCode.E))
			{
				gameManager.buyBreak = false;
				gameManager.skip = true;
				SceneManager.LoadScene("Loading Scene");
			}
		}
		else
			pressEText.gameObject.SetActive(false);
    }
}
