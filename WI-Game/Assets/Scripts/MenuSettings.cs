using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuSettings : MonoBehaviour
{
	public Slider sensitivitySlider;
	public GameObject audioOnButton;
	public GameObject audioOffButton;
	public Button crosshairType1;
	public Image crosshairType1Image;
	public Button crosshairType2;
	public Image crosshairType2Image;
	public TMP_Dropdown colorDropdown;
	public AudioSource audioSource;

	private GameManager gameManager;
	private Color blueColor = new Color(0.161f, 0.867f, 0.886f, 1f);
	private Color orangeColor = new Color(0.831f, 0.608f, 0.255f, 1f);

	private int sense;
    // Start is called before the first frame update
    void Start()
    {
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		RefreshSettingsInfo();
    }

	public void RefreshSettingsInfo()
	{
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        sense = (int)Mathf.Lerp(0, 36, Mathf.InverseLerp(1, 100, gameManager.mouseSensitivity));
		sensitivitySlider.value = sense;

		if (gameManager.crosshairType == 1)
		{
			ColorBlock colorBlock = crosshairType1.colors;
			colorBlock.normalColor = orangeColor;
			crosshairType1.colors = colorBlock;
		}
		else if (gameManager.crosshairType == 2)
		{
			ColorBlock colorBlock = crosshairType2.colors;
			colorBlock.normalColor = orangeColor;
			crosshairType2.colors = colorBlock;
		}

		crosshairType1Image.color = gameManager.selectedColor;
		crosshairType2Image.color = gameManager.selectedColor;
	}

    // Update is called once per frame
    void Update()
    {

    }

	public void ChangeSensitivity()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		float currentSense = sensitivitySlider.value;
		gameManager.mouseSensitivity = Mathf.Lerp(1, 100, Mathf.InverseLerp(0, 36, currentSense));
	}

	public void TurnAudioOn()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		gameManager.isAudioOn = true;
		audioOnButton.SetActive(true);
		audioOffButton.SetActive(false);
	}

	public void TurnAudioOff()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		gameManager.isAudioOn = false;
		audioOnButton.SetActive(false);
		audioOffButton.SetActive(true);
	}

	public void SelectType1()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		ColorBlock colorBlock1 = crosshairType1.colors;
		colorBlock1.normalColor = orangeColor;
		crosshairType1.colors = colorBlock1;

		ColorBlock colorBlock2 = crosshairType2.colors;
		colorBlock2.normalColor = blueColor;
		crosshairType2.colors = colorBlock2;

		gameManager.crosshairType = 1;
	}

	public void SelectType2()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		ColorBlock colorBlock1 = crosshairType1.colors;
		colorBlock1.normalColor = blueColor;
		crosshairType1.colors = colorBlock1;

		ColorBlock colorBlock2 = crosshairType2.colors;
		colorBlock2.normalColor = orangeColor;
		crosshairType2.colors = colorBlock2;

		gameManager.crosshairType = 2;
	}

	public void ChangeCrosshairColor()
	{
		if (gameManager.isAudioOn)
			audioSource.PlayOneShot(audioSource.clip);

		switch(colorDropdown.value)
		{
			case 0:
				gameManager.selectedColor = gameManager.whiteColor;
				break;
			case 1:
				gameManager.selectedColor = gameManager.redColor;
				break;
			case 2:
				gameManager.selectedColor = gameManager.yellowColor;
				break;
			case 3:
				gameManager.selectedColor = gameManager.greenColor;
				break;
		}

		crosshairType1Image.color = gameManager.selectedColor;
		crosshairType2Image.color = gameManager.selectedColor;
	}
}
