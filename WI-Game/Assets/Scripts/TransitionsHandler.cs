using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionsHandler : MonoBehaviour
{
	public Animator transition;
	public float transitionTime = 1;

	private GameManager gameManager;

	public void TransitionTo(string sceneName)
	{
		StartCoroutine(LoadTransitionedScene(sceneName));
	}

	private IEnumerator LoadTransitionedScene(string sceneName)
	{
		transition.SetTrigger("Start");

		yield return new WaitForSeconds(transitionTime);

		SceneManager.LoadScene(sceneName);
	}

	public void StartDeathFade()
	{
		StartCoroutine(Fade());
	}

	private IEnumerator Fade()
	{
		transition.SetTrigger("Start");

		yield return new WaitForSeconds(transitionTime);
	}

	public void StartCrossfade()
	{
		StartCoroutine(Crossfade());
	}

	private IEnumerator Crossfade()
	{
		transition.SetTrigger("Start");

		yield return new WaitForSeconds(transitionTime);
	}

}
