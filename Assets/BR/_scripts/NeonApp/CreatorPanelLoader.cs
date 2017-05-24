//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This script loads the list of creators from a json and populates a supplied panel with details
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BR.BRUtilities;
using BR.BRUtilities.UI;

namespace BR.App {
	public class CreatorPanelLoader : MonoBehaviour
	{
		#region UI VARIABLES
		[Tooltip("Object to be instantiated.")]
		public GameObject influencerButton;

		[Tooltip("Parent panel for instantiation")]
		public Transform parentPanel;

		#endregion

		#region DATA VARIABLES

		[Tooltip("API endpoint for connection")]
		public string API_ENDPOINT = "";

		public bool isDebug = true;
		public TextAsset influencerListJSON;
		private List<InfluencerDetail> influencers;
		private JSONParser parser = new JSONParser ();

		#endregion

		#region UNITY MONOBEHAVIOUR METHODS

		void Start() {
			if (GetComponentInParent<CanvasObject>().setupOnLoad) {
				if (isDebug) {
					// Currently in debug mode
					// Do local processing
					string jsonStr = influencerListJSON.text;
					JSONObject obj = new JSONObject (jsonStr);

					// Get the list of influencers
					influencers = parser.GetInfluencerList (obj);

					SetupInfluencers (influencers);
				}
			}
		}

		#endregion

		#region PRIVATE METHODS

		/// <summary>
		/// A method to set up the creator panel from other views
		/// </summary>
		/// <param name="_influencers">Influencers.</param>
		public void SetupInfluencers(List<InfluencerDetail> _influencers) {
			DownloadQueue InfluencerQueue = new DownloadQueue ();

			foreach (InfluencerDetail influencer in _influencers) {
				// Show the influencer in view only if 
				// the showInView flag is set
				if (influencer.showInView) {
					GameObject go = Instantiate (influencerButton, Vector3.zero, Quaternion.identity, parentPanel) as GameObject;
					(go.transform as RectTransform).localPosition = Vector3.zero;
					(go.transform as RectTransform).localRotation = Quaternion.identity;

					InfluencerButtonDetailsLoader btnDetails = go.GetComponent<InfluencerButtonDetailsLoader> ();
					btnDetails.SetupInfluencerButon (influencer);

					TextureDownloadObject influencerPicObject = new TextureDownloadObject (influencer.picUrl, btnDetails.imgInfluencerPicture, btnDetails.infProgressBar, 1, btnDetails.InfluencerPictureLoaded);
					InfluencerQueue.queue.Enqueue (influencerPicObject);
				}
			}

			InfluencerQueue.currentStatus = true;
			DownloadManager.Instance ().listDownloadQueues.Add(InfluencerQueue);
		}

		#endregion

	}
}

