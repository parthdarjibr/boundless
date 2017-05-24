using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading;
using UnityEngine.UI;
using System;
using CielaSpike;

namespace BR.BRUtilities {
	public class DownloadUtility {

		#region PUBLIC METHODS

		/// <summary>
		/// Synchronously downloads string from a url.
		/// </summary>
		/// <returns>String.</returns>
		/// <param name="url">URL.</param>
		public string GetStringSync(string url) {
			WebClient wc = new WebClient ();
			return wc.DownloadString (url);
		}


		public void DownloadAndSetTexture(string url, Image img, Image progressBar, Action act) {
			WebClient wc = new WebClient ();
			wc.DownloadProgressChanged += (sender, args) => DownloadProgressHandler(progressBar, args);
			wc.DownloadDataCompleted += (sender, args) => TextureDownloaded (img, act, args);
			//ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
			try {
				wc.DownloadDataAsync (new System.Uri (url));
			} catch (WebException e) {
				Debug.Log ("Download Error: " + e.Message);
			}
		}

		#endregion

		#region EVENT HANDLERS

		void TextureDownloaded(Image img, Action act, System.Net.DownloadDataCompletedEventArgs args) {
			if (args.Error != null) {
				// Error downloading texture
				// TODO Handle appropriately
				UnityMainThreadDispatcher.Instance().Enqueue(LogError(args.Error.HelpLink));
			} else {
				byte[] bytes = args.Result;
				UnityMainThreadDispatcher.Instance ().Enqueue (AssignSpriteToImage (bytes, img, act));
			} 

		}

		void DownloadProgressHandler(Image progressBar, DownloadProgressChangedEventArgs e) {
			int progress = e.ProgressPercentage;
			UnityMainThreadDispatcher.Instance ().Enqueue (UpdatePercentage (progressBar, progress));
		}

		IEnumerator AssignSpriteToImage(byte[] bytes, Image img, Action act) {
			Debug.Log ("Downloaded");
			Texture2D tex = new Texture2D (1, 1);
			tex.LoadImage (bytes);
			Sprite spr = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
			img.sprite = spr;
			act ();
			yield return null;
		}

		IEnumerator AssignSpriteToImage(Texture2D tex, Image img, Action act) {
			Sprite spr = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
			img.sprite = spr;
			act ();
			yield return null;
		}

		IEnumerator UpdatePercentage(Image progressBar, int progress) {
			//Debug.Log ((float)progress/100);
			progressBar.fillAmount = (float)progress/100;
			yield return null;
		}

		IEnumerator LogError(Exception e) {
			Debug.LogError ("Error: " + e.Message);
			yield return null;
		}

		IEnumerator LogError(string error) {
			Debug.LogError ("Error: " + error);
			yield return null;
		}
		#endregion
	}
}
