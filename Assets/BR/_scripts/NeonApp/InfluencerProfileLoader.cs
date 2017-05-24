//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This class loads the influencer details on the profile panel
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BR.BRUtilities.UI;
using BR.BRUtilities;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace BR.App {
	public class InfluencerProfileLoader : MonoBehaviour
	{

		public Image infPicture;
		// public TextMeshPro tvInfluencerName, tvInfluencerHandle, tvBio;
		public Text tvInfluencerName, tvInfluencerHandle, tvBio;

		[HideInInspector] public InfluencerDetail currentInfluencer;
		public Button btnFollow;

		void Start ()
		{
			if (GetComponentInParent<CanvasObject> ().setupOnLoad) {
				SetupInfluencerProfile ();
			}
		}

		void SetupInfluencerProfile() {
			SetupInfluencerProfile (currentInfluencer);
		}

		public void SetupInfluencerProfile(InfluencerDetail influencer) {
			// Setup the profile picture
			// Create a new download queue and add this object to that queue
			DownloadQueue infPicqueue = new DownloadQueue ();

			TextureDownloadObject downloadObject = new TextureDownloadObject(influencer.picUrl, infPicture, 0);
			infPicqueue.queue.Enqueue (downloadObject);

			// Start download and add the queue to the download list
			infPicqueue.currentStatus = true;
			DownloadManager.Instance().listDownloadQueues.Add(infPicqueue);

			// Setup rest of the profile
			tvInfluencerName.text = influencer.displayName;
			tvInfluencerHandle.text = "@" + influencer.handle;
			tvBio.text = influencer.bio;

			// TODO implement follow button handler
			btnFollow.onClick.AddListener( FollowButtonPressed );
		}

		private void FollowButtonPressed() {
			ExecuteEvents.Execute (btnFollow.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);

			ErrorDetail ed = new ErrorDetail ();
			ed.SetErrorTitle ("Coming Soon...");
			ed.SetErrorDescription ("");

			/*
			List<ErrorDetail.ResponseType> r = new List<ErrorDetail.ResponseType> ();
			r.Add (ErrorDetail.ResponseType.ACCEPT);

			ed.SetResponseTypes (r);
			*/

			// Add responses to dictionary
			ed.AddToDictionary (ErrorDetail.ResponseType.ACCEPT, new UnityEngine.Events.UnityAction (delegate {
				ViewManagerUtility.Instance ().RemoveCanvasFromCurrentView ();
			}));
			ApplicationController.Instance ().OpenErrorView (ed);
		}
	}
}

