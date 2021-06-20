using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviour : MonoBehaviour
{
	private AudioSource audioSource;
	private AudioClip audioPortal;

	private GameObject portalLight;
	private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        portalLight = transform.GetChild(0).gameObject;
		audioSource = GetComponent<AudioSource>();
		audioPortal = audioSource.clip;
    }

    // Update is called once per frame
    void Update()
    {
		if (!gameManager.isAudioOn || !gameManager.isGameActive)
		{
			audioSource.mute = true;
		}
		else
		{
			audioSource.mute = false;
		}
    }

	public void ChangeStateTo(string state)
	{
		if (state == "hidden")
		{
			portalLight.SetActive(false);
		}
		if (state == "active")
		{
			if (!audioSource.isPlaying)
				audioSource.PlayOneShot(audioPortal);

			portalLight.SetActive(true);
		}
	}
}
