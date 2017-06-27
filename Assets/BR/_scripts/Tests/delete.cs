using UnityEngine;
using System.Collections;

public class delete : MonoBehaviour {

    float time = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        Debug.Log(time);
	}
}
