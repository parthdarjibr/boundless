// 
// Code by: Parth Darji
// Company: Boundless Reality
// (c) Boundless Reality, All rights reserved.
// 
// Details: This script loads videos in a panel it is called from
//
// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using BR.BRUtilities;
using BR.BRUtilities.UI;
using System;
using UnityEngine.EventSystems;

namespace BR.App {
	public class VideoPanelLoader : MonoBehaviour
	{
		#region UI Variables

		[Tooltip("Video Object to populate")]
		public GameObject videoObject; 

		[Tooltip("Parent panel for instantiation")]
		public Transform parentObject;
		public Button[] categoryButtons;

		#endregion

		#region Data Variables

		public bool isDebug = true;
		public TextAsset videoListJSON;
		private JSONParser parser = new JSONParser ();
		private List<VideoDetail> videos;
		public bool LoadInfluencerDetails = true;
		private List<GameObject> currentVideoList;
		private List<GameObject> totalVideoList;
		public List<VideoEdges> allVideoPlaylist;
		#endregion

		#region Unity Methods

		void Start ()
		{
			
		}

		#endregion

		#region Private Methods

		/*
		/// <summary>
		/// This method instantiates one prefab for each video detail in the list
		/// </summary>
		/// <param name="videos">Video list.</param>
		// TODO: This method is called on event callback
		public void SetupVideos(List<VideoDetail> videos) {

			// Setup the home video list
			currentVideoList = new List<GameObject> ();

			DownloadQueue ThumbnailQueue = new DownloadQueue ();
			DownloadQueue InfPicQueue = new DownloadQueue ();
			// Instantiate individual videos here
			foreach(VideoDetail video in videos) {
				// GameObject go = Instantiate (videoObject, Vector3.zero, Quaternion.identity, parentObject) as GameObject;
				GameObject go = Instantiate (videoObject, parentObject) as GameObject;
				RectTransform rt = go.transform as RectTransform;
				(go.transform as RectTransform).anchoredPosition3D.Set (rt.anchoredPosition3D.x, rt.anchoredPosition3D.y, 0);
				// go.transform.localPosition = new Vector3 (go.transform.localPosition.x, go.transform.localPosition.y, 0);
				go.transform.localPosition = new Vector3 (go.transform.localPosition.x, go.transform.localPosition.y, 0);
				VideoDetailsLoader videosLoader = go.GetComponent<VideoDetailsLoader> ();

				// Add instantiated object to the list
				currentVideoList.Add(go);

				videosLoader.SetupVideoDetails (video);

				// Download the thumbnail
				TextureDownloadObject thumbObject = new TextureDownloadObject(video.thumbnailUrl, videosLoader.imgThumb, videosLoader.thumbProgressBar, 0, videosLoader.VideoThumbnailLoaded);
				ThumbnailQueue.queue.Enqueue (thumbObject);

				// Download the influencer picture
				if (LoadInfluencerDetails) {
					TextureDownloadObject influencerPicObject = new TextureDownloadObject (video.userPicUrl, videosLoader.imgInfluencerPicture, videosLoader.infProgressBar, 1, videosLoader.InfluencerPictureLoaded);
					InfPicQueue.queue.Enqueue (influencerPicObject);
				}
			}

			// Start the download
			ThumbnailQueue.currentStatus = true;
			DownloadManager.Instance().listDownloadQueues.Add(ThumbnailQueue);
			if (LoadInfluencerDetails) {
				InfPicQueue.currentStatus = true;
				DownloadManager.Instance ().listDownloadQueues.Add (InfPicQueue);
			}

			// Add this list to the playlist
			VideoPlaylistManager.Instance().AddPlaylistToStack(videos);

			// Setup the category buttons
			foreach (Button b in categoryButtons) {
				b.onClick.RemoveAllListeners ();
				b.onClick.AddListener (delegate {
					CategoryClicked(b);
				});
			}
		}
		*/

		/// <summary>
		/// Overriding method for optimized data type
		/// </summary>
		/// <param name="videos">Videos.</param>
		public void SetupVideos(List<VideoEdges> videos) {
			// Setup the home video list
			currentVideoList = new List<GameObject> ();

			DownloadQueue ThumbnailQueue = new DownloadQueue ();
			DownloadQueue InfPicQueue = new DownloadQueue ();
			// Instantiate individual videos here
			foreach(VideoEdges edge in videos) {
				VideoDetail video = edge.node;

				// GameObject go = Instantiate (videoObject, Vector3.zero, Quaternion.identity, parentObject) as GameObject;
				GameObject go = Instantiate (videoObject, parentObject) as GameObject;
				RectTransform rt = go.transform as RectTransform;
				(go.transform as RectTransform).anchoredPosition3D.Set (rt.anchoredPosition3D.x, rt.anchoredPosition3D.y, 0);
				// go.transform.localPosition = new Vector3 (go.transform.localPosition.x, go.transform.localPosition.y, 0);
				go.transform.localPosition = new Vector3 (go.transform.localPosition.x, go.transform.localPosition.y, 0);
				VideoDetailsLoader videosLoader = go.GetComponent<VideoDetailsLoader> ();

				// Add instantiated object to the list
				currentVideoList.Add(go);

				videosLoader.SetupVideoDetails (video);

				// Download the thumbnail
				TextureDownloadObject thumbObject = new TextureDownloadObject(video.thumbnailUrl, videosLoader.imgThumb, videosLoader.thumbProgressBar, 0, videosLoader.VideoThumbnailLoaded);
				ThumbnailQueue.queue.Enqueue (thumbObject);

				// Download the influencer picture
				if (LoadInfluencerDetails) {
					TextureDownloadObject influencerPicObject = new TextureDownloadObject (video.userPicUrl, videosLoader.imgInfluencerPicture, videosLoader.infProgressBar, 1, videosLoader.InfluencerPictureLoaded);
					InfPicQueue.queue.Enqueue (influencerPicObject);
				}
			}

			// Start the download
			ThumbnailQueue.currentStatus = true;
			DownloadManager.Instance().listDownloadQueues.Add(ThumbnailQueue);
			if (LoadInfluencerDetails) {
				InfPicQueue.currentStatus = true;
				DownloadManager.Instance ().listDownloadQueues.Add (InfPicQueue);
			}

			// Add this list to the playlist
			VideoPlaylistManager.Instance().AddPlaylistToStack(videos);

			// Setup the category buttons
			foreach (Button b in categoryButtons) {
				b.onClick.RemoveAllListeners ();
				b.onClick.AddListener (delegate {
					CategoryClicked(b);
				});

				/*
				if (categoryAnimator.imageCategory != null) {
					categoryAnimator.imageCategory.onClick.RemoveAllListeners ();
					categoryAnimator.imageCategory.onClick.AddListener (delegate {
						CategoryClicked (b);
					});
				}*/
			}
		}

		/// <summary>
		/// Destroys all videos from the video panel
		/// </summary>
		public void RemoveAllVideos() {
			// If you only want to destroy the children, it gets a bit more complicated
			foreach(Transform child in parentObject){
				Destroy(child.gameObject);
			}
		}

		/// <summary>
		/// Sorts existing video panel with selected category
		/// </summary>
		/// <param name="category">Category to sort with.</param>
		public  void SetupExistingVideos(VideoDetail.Category category) {
			List<VideoEdges> catList = new List<VideoEdges> ();
			if (category == VideoDetail.Category.all) {
				// If the category is all, remove second to last list from the playlist stack if it exists
				// If it doesn't exist, we are already on the 'all' list.
				if (VideoPlaylistManager.Instance ().videoListStack.Count > 1) {
					VideoPlaylistManager.Instance ().RemoveLastList ();
				}
				catList = allVideoPlaylist;
				/*
				foreach (GameObject go in currentVideoList) {
					go.SetActive (true);
				}*/
			} else {
				// Get the current video list from playlist
				catList = VideoPlaylistManager.Instance().GetCategoryPlaylistFromDictionary(category);

				if (catList != null) {
					// A playlist for current category exists
					// Add it to the stack
					VideoPlaylistManager.Instance ().AddPlaylistToStack (catList);
				}
			}

			// Once we have the category video list, remove all videos from current view 
			// and populate it with new list
			RemoveAllVideos();

			// Remove list from playlist
			VideoPlaylistManager.Instance().RemoveLastList();
			SetupVideos (catList);

			// Reset the scroll of the parent panel after loading
			// parentObject.GetComponentInParent<ScrollViewExtension>().verticalNormalizedPosition = 1;
		}
			
		#endregion

		#region PRIVATE METHODS

		private void CategoryClicked(Button b) {
			// Enable button if it's name is not equal to the clicked button
			foreach(Button but in categoryButtons) {
				but.GetComponent<CategoryButtonAnimator>().SetInteractivity(true);
				// Send button up handler to everyone
				ExecuteEvents.Execute(but.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
			}

			// Disable the current category button and its child
			b.GetComponent<CategoryButtonAnimator>().SetInteractivity(false);

			VideoDetail.Category cat = (VideoDetail.Category) Enum.Parse(typeof(VideoDetail.Category), b.name, true);

			// Add a analytics event
			AnalyticsManager.Instance().SendCategoryAnalytics(cat.ToString());

			SetupExistingVideos(cat);
		}

		#endregion
	}
}