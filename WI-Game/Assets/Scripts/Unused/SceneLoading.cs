using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
	public Image loadingBar;

	private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        // StartCoroutine(LoadAsyncOperation(gameManager.sceneToGo));
    }

    // Update is called once per frame
    void Update()
    {

    }

	IEnumerator LoadAsyncOperation(string sceneToGo)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToGo);

		while (!operation.isDone)
		{
			float progress = operation.progress/0.9f;

			loadingBar.fillAmount = progress;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForEndOfFrame();
	}
}
