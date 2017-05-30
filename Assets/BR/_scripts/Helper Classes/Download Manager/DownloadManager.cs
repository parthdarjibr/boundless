//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: The DownloadManager class handles all the downloads for the application
//			A new DownloadQueue object is instantiated whenever a list is to be downloaded
//			and is passed on to this class for processing.
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Net.NetworkInformation;

namespace BR.BRUtilities {
	public class DownloadManager : MonoBehaviour
	{
		#region EVENTS

		public enum ConnectionState {
			CONNECTED,
			DISCONNECTED,
			CONNECTING
		}

		public delegate void OnConnectionChanged(ConnectionState state);
		public static event OnConnectionChanged onConnectionChanged;

		#endregion

		#region INSTANTIATION

		private static DownloadManager _instance = null;

		public static DownloadManager Instance() {
			if (!Exists ()) {
				throw new Exception ("No DownloadManager object found.");
			}
			return _instance;
		}

		private static bool Exists() {
			return _instance != null;
		}

		#endregion

		#region VARIABLES

		// Stack of download queues
		private bool currentStatus;
		[SerializeField] private int maxSimultaneousDownloads = 3;
		public List<DownloadQueue> listDownloadQueues = new List<DownloadQueue> ();
		private Vector2 sprCenter = new Vector2(0.5f, 0.5f);
		public static bool isInternetAvailable = true;

		#endregion

		#region UNITY MONO METHODS

		void Awake() {
			if (_instance == null) {
				_instance = this;
				// DontDestroyOnLoad (this.gameObject);
			}

			// Listen to events
			NetworkChange.NetworkAvailabilityChanged += NetworkChangedCallback;
		}

		void OnDestroy() {
			_instance = null;
			NetworkChange.NetworkAvailabilityChanged -= NetworkChangedCallback;

		}

		void Update() {
			foreach (DownloadQueue dq in listDownloadQueues) {
				if (dq.currentStatus != false)
					PerformDownload (dq);
			}
		}

		#endregion

		#region PUBLIC METHODS

		public bool GetCurrentStatus() {
			return currentStatus;
		}

		public void SetCurrentStatus(bool status) {
			currentStatus = status;
		}

		public void SetMaxSimultaneousDownloads(int _max) {
			maxSimultaneousDownloads = _max;
		}

		public int GetMaxSimultaneousDownloads() {
			return maxSimultaneousDownloads;
		}

		#endregion

		#region PRIVATE METHODS

		private void StopDownloadManager() {
			currentStatus = false;
		}

		private void StartDownloadManager() {
			currentStatus = true;
		}

		void PerformDownload(DownloadQueue dq) {
			if (dq.queue.Count > maxSimultaneousDownloads - 1) {
				dq.currentStatus = false;
				// Download the chunk
				for (int i = 0; i < maxSimultaneousDownloads; i++) {
					StartCoroutine (DownloadAndSetTextureChunk (dq));
				}
			} else {
				while (dq.queue.Count > 0) {
					// Download individual thumbnails
					StartCoroutine (DownloadAndSetTexture (dq.queue.Dequeue () as TextureDownloadObject));
				}
			}
			// Debug.Log (dq.queue.Count);
		}

		IEnumerator DownloadAndSetTextureChunk(DownloadQueue q) {
			TextureDownloadObject obj = q.queue.Dequeue () as TextureDownloadObject;
			WWW www = new WWW (obj.url);

			// TODO: Set the download priority

			// Progressbar
			StartCoroutine(ShowProgress(www, obj));

			yield return www;

			if (www.error == null) {
				Texture2D tex = www.texture;
				if (tex != null) {
					obj.img.sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), sprCenter);

					// Download complete
					// Call the action on the invoker
					if (obj.action != null) {
						obj.action ();
					}

                    // Set progress bar value
                    obj.progressBar.fillAmount = 0f;
				}
			} else {
				// TODO Error - handle appropriately
			}

			q.currentDownloadID++;
			if (q.currentDownloadID == maxSimultaneousDownloads) {
				q.currentDownloadID = 0;
				q.currentStatus = true;
			}
		}

		IEnumerator DownloadAndSetTexture(TextureDownloadObject obj) {
			WWW www = new WWW (obj.url);
			yield return www;
			obj.numberOfTries++;
			if (www.error == null && obj.img != null) {
				Texture2D tex = www.texture;
				if (tex != null) {
					obj.img.sprite = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), sprCenter);

					// Download complete
					// Call the action on the invoker
					if (obj.action != null) {
						obj.action ();
					}

                    // Set progressbar value to 0
                    obj.progressBar.fillAmount = 0f;
				}
			} else {
				// TODO Error - handle appropriately
				if (obj.numberOfTries < 3) {
					StartCoroutine (DownloadAndSetTexture (obj));
				}
			}
		}

		private IEnumerator ShowProgress(WWW www, TextureDownloadObject obj) {
			while (!www.isDone) {
				// Start the progressbar
				obj.SetProgressBarValue (www.progress);
				yield return null;
			}
		}

		private void NetworkChangedCallback(object sender, NetworkAvailabilityEventArgs e) {
			if (e.IsAvailable) {
				isInternetAvailable = true;
			} else {
				isInternetAvailable = false;
			}

			// Debug.Log (isInternetAvailable.ToString ());
		}

		#endregion

	}
}

