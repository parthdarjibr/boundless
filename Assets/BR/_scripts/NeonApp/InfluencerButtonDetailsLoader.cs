//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using BR.BRUtilities;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using BR.BRUtilities.UI;
using UnityEngine.Events;

namespace BR.App {
	public class InfluencerButtonDetailsLoader : MonoBehaviour
	{
		#region EVENTS


		public delegate void InfluencerButtonLoaded();
		public event InfluencerButtonLoaded onInfluencerButtonLoaded;

		#endregion

		#region UI VARIABLES

		public TextExtension tvInfluencerName;
		public Image imgInfluencerPicture, infProgressBar;
		public Button btnFollow, btnInfluencer;

		#endregion

		#region DATA VARIABLES

		[HideInInspector] public InfluencerDetail currentInfluencer;

		#endregion

		#region PRIVATE METHODS

		public void SetupInfluencerButon(InfluencerDetail influencer) {
			// TODO check for internet connection
			if (influencer == null) {
				return;
			} else {
				currentInfluencer = influencer;

				tvInfluencerName.text = "@" + influencer.handle;

				// Add the listeners
				btnInfluencer.onClick.AddListener (this.InfluencerButtonClicked);
				btnFollow.onClick.AddListener(FollowButtonClicked);


				if (onInfluencerButtonLoaded != null) {
					onInfluencerButtonLoaded ();
				}
			}
		}

		#endregion

		#region PUBLIC METHODS

		#endregion

		#region EVENT HANDLERS

		public void InfluencerPictureLoaded() {
			infProgressBar.gameObject.SetActive (false);
			btnInfluencer.interactable = true;
			btnFollow.interactable = true;

		}

		void InfluencerButtonClicked() {
			AnalyticsManager.Instance ().SendButtonClickAnalytics ("creator", "creatorOnHome", currentInfluencer.displayName);

			ExecuteEvents.Execute (btnInfluencer.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
			// ApplicationController.Instance ().OpenInfluencerView (currentInfluencer);
			ApplicationController.Instance ().OpenInfluencerView (currentInfluencer.gid);
		}

		void FollowButtonClicked() {
			ExecuteEvents.Execute (btnFollow.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);

			ErrorDetail ed = new ErrorDetail ();
			ed.SetErrorTitle ("Coming Soon...");
			ed.SetErrorDescription ("");

			/*
			List<ErrorDetail.ResponseType> r = new List<ErrorDetail.ResponseType> ();
			r.Add (ErrorDetail.ResponseType.ACCEPT);

			ed.SetResponseTypes (r);
			

			// Add responses to dictionary
			ed.AddToDictionary (ErrorDetail.ResponseType.ACCEPT, new UnityEngine.Events.UnityAction (delegate {
				ViewManagerUtility.Instance ().RemoveCanvasFromCurrentView ();
			}));
			ApplicationController.Instance ().OpenErrorView (ed);
            */
		}

		#endregion
	}
}

