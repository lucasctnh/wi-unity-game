using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
	public GameObject gameOverScreen;
	public TMP_Text highscore;
	public GameObject canvasHealthbar;
	public GameObject[] masksHealth1;
	public GameObject[] masksHealth2;
	public GameObject[] masksHealth3;
	public GameObject fillHealth;
	public GameObject fillLazyHealth;
	public GameObject[] masksArmor1;
	public GameObject[] masksArmor2;
	public GameObject[] masksArmor3;
	public GameObject fillArmor;
	public GameObject fillLazyArmor;
	public GameObject player;
	public bool lookingAtPowerUp = false;
	public bool addedWave = false;

    public static GameManager m_instance;

	[Header("Mechanics")]
	public bool isGameActive = false;
	public int wavesCleared = 0;
	public int waveNumber = 0;
	public bool buyBreak = false;
	public bool skip = true;
	public float bulletDamage = 5;
	public float critChance = 0.01f;
	public float armorPiercing = 0.5f;

	[Header("Settings")]
	public float mouseSensitivity = 100f;
	public bool isAudioOn = true;
	public int crosshairType = 1;
	public Color whiteColor = new Color(0.99f, 0.99f, 0.99f, 1f);
	public Color redColor = new Color(0.886f, 0.333f, 0.161f, 1f);
	public Color yellowColor = new Color(0.886f, 0.842f, 0.168f, 1f);
	public Color greenColor = new Color(0.409f, 0.886f, 0.161f, 1f);
	public Color selectedColor;

	public List<float> enemyWaveStats;
	public float droneWaveStat;

	private AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
		CreateInstance();

		audioSource = gameObject.GetComponent<AudioSource>();
		selectedColor = whiteColor;
    }

	private void CreateInstance()
    {
        if (!m_instance)
            m_instance = this;
        else
            Destroy(gameObject);
    }

	public void SearchNewPlayer()
	{
		player = GameObject.Find("Player");
		if (isGameActive)
			player.GetComponent<PlayerController>().playerCanvas.gameObject.SetActive(true);
		else
			player.GetComponent<PlayerController>().playerCanvas.gameObject.SetActive(false);

		player.GetComponent<PlayerController>().UpdateScore();
	}

    // Update is called once per frame
    void Update()
    {
		if (!isAudioOn || !isGameActive)
			audioSource.mute = true;
		else
			audioSource.mute = false;

		if (PauseMenu.gameIsPaused)
			audioSource.volume = 0.05f;
		else
			audioSource.volume = 0.1f;
    }

	public void UpdateHealthbar(float currentHealth, float maxHealth, int type)
	{
		LazyBar controller = canvasHealthbar.transform.GetChild(1).GetComponent<LazyBar>();

		if (type == 1)
		{
			fillLazyHealth.transform.SetParent(masksHealth1[0].transform);
			fillHealth.transform.SetParent(masksHealth1[1].transform);

			controller.SetBars(masksHealth1[0].GetComponent<Image>(), masksHealth1[1].GetComponent<Image>());
		}
		else if (type == 2)
		{
			fillLazyHealth.transform.SetParent(masksHealth2[0].transform);
			fillHealth.transform.SetParent(masksHealth2[1].transform);

			controller.SetBars(masksHealth2[0].GetComponent<Image>(), masksHealth2[1].GetComponent<Image>());
		}
		else if (type == 3)
		{
			fillLazyHealth.transform.SetParent(masksHealth3[0].transform);
			fillHealth.transform.SetParent(masksHealth3[1].transform);

			controller.SetBars(masksHealth3[0].GetComponent<Image>(), masksHealth3[1].GetComponent<Image>());
		}

		controller.UpdateBar(currentHealth, maxHealth);
	}

	public void UpdateArmorbar(float currentArmor, float maxArmor, int type = 0)
	{
		LazyBar controller = canvasHealthbar.transform.GetChild(2).GetComponent<LazyBar>();

		if (type == 0)
		{
			canvasHealthbar.transform.GetChild(1).gameObject.SetActive(false);
			fillLazyArmor.transform.SetParent(canvasHealthbar.transform.GetChild(2));
			fillArmor.transform.SetParent(canvasHealthbar.transform.GetChild(2));

			controller.SetBars(fillLazyArmor.GetComponent<Image>(), fillArmor.GetComponent<Image>());
		}
		else
			canvasHealthbar.transform.GetChild(1).gameObject.SetActive(true);

		if (type == 1)
		{
			fillLazyArmor.transform.SetParent(masksArmor1[0].transform);
			fillArmor.transform.SetParent(masksArmor1[1].transform);

			controller.SetBars(masksArmor1[0].GetComponent<Image>(), masksArmor1[1].GetComponent<Image>());
		}
		else if (type == 2)
		{
			fillLazyArmor.transform.SetParent(masksArmor2[0].transform);
			fillArmor.transform.SetParent(masksArmor2[1].transform);

			controller.SetBars(masksArmor2[0].GetComponent<Image>(), masksArmor2[1].GetComponent<Image>());
		}
		else if (type == 3)
		{
			fillLazyArmor.transform.SetParent(masksArmor3[0].transform);
			fillArmor.transform.SetParent(masksArmor3[1].transform);

			controller.SetBars(masksArmor3[0].GetComponent<Image>(), masksArmor3[1].GetComponent<Image>());
		}

		controller.UpdateBar(currentArmor, maxArmor);
	}

	public void ResetLerpFactorOnLazyBar(string bar)
	{
		if (bar == "health")
		{
			LazyBar healthController = canvasHealthbar.transform.GetChild(1).GetComponent<LazyBar>();
			healthController.ResetLerpFactor();
		}
		else if (bar =="armor")
		{
			LazyBar armorController = canvasHealthbar.transform.GetChild(2).GetComponent<LazyBar>();
			armorController.ResetLerpFactor();
		}
	}

	public void ShowHealthbar(bool show)
	{
		canvasHealthbar.gameObject.SetActive(show);
	}

	public void GameOver()
	{
		highscore.text = (waveNumber - 1).ToString();
		isGameActive = false;
		ResetGameStats();

		player.GetComponent<PlayerController>().playerCanvas.gameObject.SetActive(false);
		player.transform.GetChild(0).GetChild(0).GetComponent<Animator>().speed = 0;
		audioSource.mute = true;

		gameOverScreen.SetActive(true);
		canvasHealthbar.gameObject.SetActive(false);

		TransitionsHandler deathFadeHandler = GameObject.Find("Death Fade")
			.GetComponent<TransitionsHandler>();
		deathFadeHandler.StartDeathFade();
	}

	private void ResetGameStats()
	{
		wavesCleared = 0;
		waveNumber = 0;
		buyBreak = false;
		skip = true;
		bulletDamage = 5;
		critChance = 0.01f;
		armorPiercing = 0.5f;
	}

	public void RestartGame()
	{
		ResetGameStats();

		gameOverScreen.SetActive(false);

		TransitionsHandler loadingFadeHandler = GameObject.Find("Loading Crossfade")
			.GetComponent<TransitionsHandler>();
		loadingFadeHandler.TransitionTo("Without Props");

		if (isAudioOn)
			audioSource.mute = false;
	}

	public void StartGame()
	{
		isGameActive = true;

		player.GetComponent<PlayerController>().playerCanvas.gameObject.SetActive(true);
	}

	public void ApplyGravity(ref Vector3 gravityVector, float gravityForce, bool isGrounded, bool canJump, float jumpHeight = 0)
	{
		// Using free fall equation to calculate gravity
		if (canJump && Input.GetButtonDown("Jump") && isGrounded)
		{
			gravityVector.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
		}

		if (isGrounded && gravityVector.y < 0)
		{
			gravityVector.y = -2f;
		}

		gravityVector.y += gravityForce * Time.deltaTime;
	}
}
