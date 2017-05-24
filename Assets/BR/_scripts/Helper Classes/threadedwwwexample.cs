using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CielaSpike;
using UnityEngine.UI;

public class threadedwwwexample : MonoBehaviour {
	public Image image;

	// Use this for initialization
	void Start () {
		Caching.CleanCache ();
		image = GetComponent<Image> ();
		//WWW www = new WWW ("https://s3-us-west-2.amazonaws.com/public.goboundless.io/Influencer+Profile+Assets/Dave+Days/StudioTour.png");
		StartCoroutine (DownloadImage ());
		//this.StartCoroutineAsync(DownloadImage(www));
	}

	IEnumerator DownloadImage() {//WWW www) {
		//WWW www = new WWW ("http://www.best.cornell.edu/images/etc/CSC_0576.JPG");
		WWW www = new WWW ("https://dwknz3zfy9iu1.cloudfront.net/uscenes_h-264_uhd_test.mp4");

		yield return www;

		Debug.Log ("Downloaded");
		/*
		Texture2D tex = new Texture2D (2, 2);
		tex = www.texture;
		Sprite spr = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
		image.sprite = spr;*/
	}

}
