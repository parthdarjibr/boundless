using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (OpenNextScene ());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			SceneManager.LoadSceneAsync (1);
	}

	IEnumerator OpenNextScene() {
		yield return new WaitForSeconds (2);
		SceneManager.LoadSceneAsync (1);
	}
}
