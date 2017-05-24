using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BR.BRUtilities;
using System.Net;
using System.Threading;
using BR.BRUtilities.UI;

public class CoroutineExample : MonoBehaviour {
	VideoObjectImageExtension texImg;
	public string url;

	byte[] bytes;
	public GameObject go;
	public Transform parentTransform;

	void Start() {
		texImg = GetComponent<VideoObjectImageExtension> ();

		/*
		for (int i = 0; i < 20; i++) {
			Thread t = new Thread (() => InstantiateObject(url));
		}*/
		DownloadFileWebClient (url);
		//StartCoroutine(DownloadTextureWWW(str));
	}

	/*
	void Update() {
		if (currentState == State.DOWNLOADED && !imgLoaded) {
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage (bytes);
			Sprite spr = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
			texImg.sprite = spr;

			imgLoaded = true;
		}

	}*/

	void InstantiateObject(string url) {
		Instantiate (go, parentTransform);
		DownloadFileWebClient (url);
	}

	/// <summary>
	/// Downloads the file.
	/// </summary>
	/// <param name="url">URL.</param>
	public void DownloadFileWebClient(string url) {
		bytes = null;
		WebClient wc = new WebClient ();
		wc.DownloadDataCompleted += new System.Net.DownloadDataCompletedEventHandler (DownloadDataComplete);
		wc.DownloadDataAsync(new System.Uri(url), this);

	}

	void DownloadDataComplete(object sender, System.Net.DownloadDataCompletedEventArgs args) {
		if (args.Error != null) {
		} else {
			bytes = args.Result;
			UnityMainThreadDispatcher.Instance ().Enqueue (UpdateTexture (bytes));
		}
	}

	public IEnumerator UpdateTexture(byte[] bytes) {
		Texture2D tex = new Texture2D(2, 2);
		tex.LoadImage (bytes);
		Sprite spr = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
		texImg.GetComponent<Image>().sprite = spr;
		yield return null;
	}

	private IEnumerator DownloadTextureWWW(string url) {
		Texture2D tex = new Texture2D(2, 2) ;
		WWW www = new WWW (url);
		yield return www;
		www.LoadImageIntoTexture (tex);
		Sprite spr = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
		texImg.sprite = spr;
	}
}
