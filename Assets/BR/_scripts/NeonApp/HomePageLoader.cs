//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This script loads the home screen and 
//			instantiates all the child views.
//			This view fetches all the data required for 
//			the view and then assigns them to the child views.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BR.BRUtilities;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using UnityEngine.UI;
using BR.BRUtilities.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

namespace BR.App {
	public class HomePageLoader : MonoBehaviour
	{
		#region VARIABLES
		public static Dictionary<string, InfluencerDetail> featuredInfluencerDict = new Dictionary<string, InfluencerDetail>();
		public VideoPanelLoader videoPanel;
		public CreatorPanelLoader creatorPanel;
		public List<VideoDetail.Category> allCategories;
		public ScrollManager videoScrollRect, influencerScrollRect;

		// private List<VideoDetail> homeVideoList;
		// private List<InfluencerDetail> influencerList;
		private List<VideoEdges> homeVideoList;
		private List<InfluencerEdges> influencerList;

		private string HomePageQuery = @"
		{
			featuredInfluencers {
				edges{
					node {
						gid
						displayName
						handle
						picUrl
						id
						bio
						showInView
					}
				}
			}
			featuredVideos {
				edges {
					node {
						gid
						name
						thumbnailUrl
						description
						createdAt
						streamUrl
						userGid
						displayType
						customMetadata
					}
				}
			}
		}
		";
		private QueryParser queryParser = new QueryParser();

		private void CreateAndSetCategoryPlaylist(VideoDetail.Category category) {
			// Create a new list to hold the sorted videos
			List<VideoEdges> catVideo = new List<VideoEdges>();
			int id = 0;

			foreach (VideoEdges edge in homeVideoList) {
				VideoDetail video = edge.node;
				if (video.categories.Contains (category)) {
					// Create a copy of the video detail to work from
					VideoDetail videoCopy = VideoDetail.copyFrom(video);

					// The video belongs to the category
					// Change the current ID and add the video to the list
					videoCopy.SetIdInPlaylist(id++);

					VideoEdges videoEdge = new VideoEdges ();
					videoEdge.node = videoCopy;
					catVideo.Add(videoEdge);
				}
			}

			// Add this list to the list
			VideoPlaylistManager.Instance().AddPlaylistToDictionary(category, catVideo);

            Debug.Log("Playlist added");
		}

		#endregion

		#region PUBLIC METHODS

		public void SortByCategories(VideoDetail.Category category) {
			videoPanel.SetupExistingVideos (category);
		}
			
		#endregion

		#region SCROLLVIEW OPTIMIZED METHODS

		public VideoDetailParams videoGridParams;
		public VideoScrollRectItemsAdapter videoGridAdapter;
		public GridParams creatorGridParams;
		CreatorScrollRectItemsAdapter creatorGridAdapter;

		public IEnumerator SetupHomeScreenOptimized() {
			if (!ConnectionManager.Instance ().isNetworkAvailable) {
				// Send data about wifi unreachability
				AnalyticsManager.Instance ().SendNoWifiAnalytics ();

				ErrorDetail ed = new ErrorDetail ();
				ed.SetErrorTitle ("Connect to Wifi");
				ed.SetErrorDescription ("A Wifi connection is required to use the app");

				// Associate this error detail with an action
				// ed.AddToDictionary (ErrorDetail.ResponseType.RETRY, SetupHomeScreen);
				ed.AddToDictionary (ErrorDetail.ResponseType.EXIT, new UnityEngine.Events.UnityAction (delegate {
                    // OVRManager.PlatformUIGlobalMenu ();
                    OVRManager.PlatformUIConfirmQuit();
				}));

                /*
                ed.AddToDictionary(ErrorDetail.ResponseType.RETRY, new UnityEngine.Events.UnityAction(delegate
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }));
                */

                ApplicationController.Instance ().OpenErrorView (ed);
			} else {
				// Execute home page query
				string homeQueryUrl = QueryBuilder.Instance().BuildQuery(HomePageQuery);

				// Get the json from the query
				// string homeQueryJson = QueryBuilder.Instance ().GetQueryString (homeQueryUrl);
				WWW www = new WWW(homeQueryUrl);
				yield return www;

                if (www.error == null) {
                    string homeQueryJson = www.text;
                    // Get the video and influencers list from the json
                    // homeVideoList = queryParser.GetVideoList (homeQueryJson);
                    // influencerList = queryParser.GetInfluencerList (homeQueryJson, true, out featuredInfluencerDict);

                    // Optimized for jsonutility
                    JsonUtilityHelper.instance.GetHomeLists(homeQueryJson, out homeVideoList, out influencerList, out featuredInfluencerDict);
                    // homeVideoList = JsonUtilityHelper.instance.GetVideoList(homeQueryJson, false);
                    // influencerList = JsonUtilityHelper.instance.GetInfluencerList (homeQueryJson, out featuredInfluencerDict, true, false);


                    // Add influencer details to videos
                    foreach (VideoEdges edge in homeVideoList) {
                        VideoDetail video = edge.node;
                        InfluencerDetail influencer = new InfluencerDetail();
                        featuredInfluencerDict.TryGetValue(video.userGid, out influencer);

                        if (influencer != null) {
                            // If influencer is found, add their details to the video object
                            video.SetUserHandle(influencer.handle);
                            video.SetUserPicUrl(influencer.picUrl);
                            video.SetUserGid(influencer.gid);
                        }
                    }

                    // Remove all videos from influencer list and remove ones with "show in view" as false
                    influencerList.RemoveAll(inf => inf.node.showInView == false);

                    // Setup the current playlist
                    // Add this list to the playlist
                    VideoPlaylistManager.Instance().AddPlaylistToStack(homeVideoList);

                    // Set up the category video lists
                    foreach (VideoDetail.Category cat in allCategories) {
                        CreateAndSetCategoryPlaylist(cat);
                    }

                    // Setup scroll rects
                    influencerScrollRect.InitializeCreatorAdapter(influencerList);
                    videoScrollRect.InitializeVideoAdapter(homeVideoList);

                    categoryButtons[0].onClick.RemoveAllListeners();
                    categoryButtons[1].onClick.RemoveAllListeners();
                    categoryButtons[2].onClick.RemoveAllListeners();
                    categoryButtons[3].onClick.RemoveAllListeners();

                    categoryButtons[0].onClick.AddListener(delegate{ CategoryClicked(categoryButtons[0]); });
                    categoryButtons[1].onClick.AddListener(delegate{ CategoryClicked(categoryButtons[1]); });
                    categoryButtons[2].onClick.AddListener(delegate{ CategoryClicked(categoryButtons[2]); });
                    categoryButtons[3].onClick.AddListener(delegate{ CategoryClicked(categoryButtons[3]); });

                    /*
					// Setup category buttons
					// Setup the category buttons
					foreach (Button b in categoryButtons) {
						b.onClick.RemoveAllListeners ();
						b.onClick.AddListener (delegate {
							CategoryClicked(b);
						});
					}
                    */

                    /*
					// Setup the video and creator lists
					videoGridAdapter = new VideoScrollRectItemsAdapter ();
					creatorGridAdapter = new CreatorScrollRectItemsAdapter ();*/

                    // StartCoroutine (DelayedInitCreators ());
                    // StartCoroutine (DelayedInitVideos ());
                }
			}
		}

		void OnDestroy() {
			if (videoGridAdapter != null)
				videoGridAdapter.Dispose ();

			if (creatorGridAdapter != null)
				creatorGridAdapter.Dispose (); 
		}

		IEnumerator DelayedInitVideos() {
			// Wait for 3 frames
			yield return new WaitForSeconds(1f);

			videoScrollRect.InitializeVideoAdapter(homeVideoList);

			// dStartCoroutine (DelayedInitCreators ());
		}

		IEnumerator DelayedInitCreators() {
			yield return new WaitForSeconds(0.5f);

			// Initiate influencer list
			influencerScrollRect.InitializeCreatorAdapter(influencerList);
		}

		IEnumerator WaitForSeconds(float time) {
			yield return new WaitForSeconds (time);

			videoScrollRect.InitializeVideoAdapter(homeVideoList);


			/*
			// Initialize video list
			videoGridAdapter.Init (videoGridParams);
			videoGridAdapter.ChangeModels (homeVideoList.ToArray ());
			// StartCoroutine (DelayedInitCreators ());
			// Initiate influencer list
			creatorGridAdapter.Init(creatorGridParams);
			creatorGridAdapter.ChangeModels (influencerList.ToArray ());
			*/
		}
			
		#endregion

		#region CATEGORY METHODS

		public Button[] categoryButtons;
		public List<VideoDetail> allVideoPlaylist;

		private void CategoryClicked(Button clickedButton) {
			// Enable button if it's name is not equal to the clicked button
			foreach(Button but in categoryButtons) {
				but.GetComponent<CategoryButtonAnimator>().SetInteractivity(true);
				// Send button up handler to everyone
				ExecuteEvents.Execute(but.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
			}

            // Disable the current category button and its child
            clickedButton.GetComponent<CategoryButtonAnimator>().SetInteractivity(false);

			VideoDetail.Category cat = (VideoDetail.Category) Enum.Parse(typeof(VideoDetail.Category), clickedButton.name, true);

			// Add a analytics event
			AnalyticsManager.Instance().SendCategoryAnalytics(cat.ToString());

			SetupExistingVideos(cat);
		}

        public  void SetupExistingVideos(VideoDetail.Category category) {
			List<VideoEdges> catList = new List<VideoEdges> ();
			if (category == VideoDetail.Category.all) {
				// If the category is all, remove second to last list from the playlist stack if it exists
				// If it doesn't exist, we are already on the 'all' list.
				if (VideoPlaylistManager.Instance ().videoListStack.Count > 1) {
					VideoPlaylistManager.Instance ().RemoveLastList ();
				}
				catList = homeVideoList;
				/*
				foreach (GameObject go in currentVideoList) {
					go.SetActive (true);
				}*/
			} else {
				// The category click means the last category playlist needs to be removed
				// from the list
				if (VideoPlaylistManager.Instance ().videoListStack.Count > 1) {
					VideoPlaylistManager.Instance ().RemoveLastList ();
				}
				// VideoPlaylistManager.Instance().RemoveLastList();

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
			// RemoveAllVideos();

			// Remove list from playlist
			// VideoPlaylistManager.Instance().RemoveLastList();
			videoScrollRect.UpdateVideoAdapter (catList);

			// Reset the scroll of the parent panel after loading
			// parentObject.GetComponentInParent<ScrollViewExtension>().verticalNormalizedPosition = 1;
		}

		#endregion
	}
}
