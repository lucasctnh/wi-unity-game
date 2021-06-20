using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
	public float defaultSpeed = 6f;
	public float runningSpeed = 14f;
	public float gravity = -9.81f;
	public float jumpHeight = 3f;
	public bool isGrounded;

	[Header("UI Components")]
	public Canvas playerCanvas;
	public TextMeshProUGUI wavesText;

	public Transform groundCheck;
	public float groundDistance = 0.4f;
	public LayerMask groundMask;

	private float currentSpeed;
	private Vector3 gravityVector = new Vector3();
	private Vector3 moveXZ;
	private float moveReducer = 2.5f;

	private CharacterController controller;
	private GameManager gameManager;
    private AudioSource[] audioSources;

	[Header("Audio Clips")]
    [Tooltip("The audio clip that is played while walking."), SerializeField]
    public AudioClip walkingSound;

    [Tooltip("The audio clip that is played while running."), SerializeField]
    public AudioClip runningSound;

	public void PlayDeathSound()
	{
		audioSources[1].PlayOneShot(audioSources[1].clip);
	}

	private void Awake()
	{
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		gameManager.SearchNewPlayer();
	}

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSources = GetComponents<AudioSource>();

		currentSpeed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		// Movement
        float xInput = Input.GetAxis("Horizontal");
		float zInput = Input.GetAxis("Vertical");
		bool isGettingInput = !((xInput == 0) && (zInput == 0));

		if (gameManager.isGameActive &&
			Input.GetKey(KeyCode.LeftShift) &&
			isGrounded &&
			zInput > 0)
		{
			currentSpeed = runningSpeed;
			audioSources[0].clip = runningSound;
		}
		else if (isGrounded)
		{
			currentSpeed = defaultSpeed;
			audioSources[0].clip = walkingSound;
			if (!isGettingInput)
				currentSpeed = 1;
		}

		if (isGrounded)
			moveXZ = transform.right * xInput + transform.forward * zInput;
		else if (zInput > 0)
			moveXZ = transform.right * (xInput/moveReducer) + transform.forward;
		else if (zInput < 0)
		{
			currentSpeed = defaultSpeed;
			moveXZ = transform.right * (xInput/moveReducer) + transform.forward * (zInput/moveReducer);
		}

		gameManager.ApplyGravity(ref gravityVector, gravity, isGrounded, true, jumpHeight);

		Vector3 moveXYZ = (moveXZ * currentSpeed) + gravityVector;

		if (gameManager.isGameActive)
		{
			PlayFootstepSounds(isGettingInput);
			controller.Move(moveXYZ * Time.deltaTime);
		}
		else
			audioSources[0].Stop();

		if (!gameManager.isAudioOn || !gameManager.isGameActive)
		{
			audioSources[0].mute = true;
			audioSources[1].mute = true;
		}
		else
		{
			audioSources[0].mute = false;
			audioSources[1].mute = false;
		}
    }

    private void PlayFootstepSounds(bool isGettingInput)
    {
        if (isGrounded && isGettingInput)
        {
            if (!audioSources[0].isPlaying)
            {
                audioSources[0].Play();
            }
        }
        else
        {
            if (audioSources[0].isPlaying)
            {
                audioSources[0].Pause();
            }
        }
    }

	public void UpdateScore()
	{
		wavesText.text = "WAVES: " + gameManager.wavesCleared;
	}
}
