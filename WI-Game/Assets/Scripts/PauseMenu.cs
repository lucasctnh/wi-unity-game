using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
	public static bool gameIsPaused = false;

	public GameObject pauseMenuUI;
	public GameObject buttonsScreen;
	public GameObject settingsScreen;

	private GameManager gameManager;
	private AudioSource audioSource;

	private int sense;
    // Start is called before the first frame update
    void Start()
    {
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameManager.isGameActive)
		{
			if (gameIsPaused)
			{
				Resume();
				settingsScreen.SetActive(false);
				buttonsScreen.SetActive(true);

				Cursor.lockState = CursorLockMode.Locked;
			}
			else
				Pause();
		}
    }

	private void Resume()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		gameIsPaused = false;
	}

	public void ResumeWithSound()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		gameIsPaused = false;
	}

	private void Pause()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		gameIsPaused = true;
	}

	public void Restart()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		Resume();

		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		gameManager.RestartGame();
	}

	public void ShowSettings()
	{
		settingsScreen.GetComponent<MenuSettings>().RefreshSettingsInfo();

		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		buttonsScreen.SetActive(false);
		settingsScreen.SetActive(true);
	}

	public void GoBack()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		settingsScreen.SetActive(false);
		buttonsScreen.SetActive(true);
	}
}
