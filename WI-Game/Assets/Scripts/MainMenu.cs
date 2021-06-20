using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject startScreen;
	public GameObject menuScreen;
	public GameObject menuScreenButtons;
	public GameObject controlsScreen;
	public GameObject settingsScreen;

	public GameObject insideTransitionsHandler;
	public GameObject outsideTransitionsHandler;

	public AudioSource audioSource;
	private GameManager gameManager;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.5f);
		insideTransitionsHandler.GetComponent<TransitionsHandler>().StartCrossfade();
        yield return new WaitForSeconds(0.75f);

		startScreen.SetActive(false);
		menuScreen.SetActive(true);

		audioSource = GetComponent<AudioSource>();
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

	public void Play()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		outsideTransitionsHandler.SetActive(true);
		TransitionsHandler handler = outsideTransitionsHandler.GetComponent<TransitionsHandler>();
		handler.TransitionTo("Without Props");
	}

	public void SeeControls()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		menuScreenButtons.SetActive(false);
		controlsScreen.SetActive(true);
	}

	public void SeeSettings()
	{
		settingsScreen.GetComponent<MenuSettings>().RefreshSettingsInfo();

		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		menuScreenButtons.SetActive(false);
		settingsScreen.SetActive(true);
	}

	public void GoBack()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		if (controlsScreen.activeSelf)
			controlsScreen.SetActive(false);
		if (settingsScreen.activeSelf)
			settingsScreen.SetActive(false);

		menuScreenButtons.SetActive(true);
	}
}
