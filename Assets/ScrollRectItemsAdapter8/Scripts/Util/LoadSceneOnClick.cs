using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadSceneOnClick : MonoBehaviour {
    public string sceneName;


	void Start () {
        GetComponent<Button>().onClick.AddListener(LoadScene);
	}

    void LoadScene()
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
#else
        Application.LoadLevel(sceneName);
#endif
    }
}
