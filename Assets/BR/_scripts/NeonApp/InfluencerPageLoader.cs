//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BR.BRUtilities;
using UnityEngine.UI;

namespace BR.App {
	public class InfluencerPageLoader : MonoBehaviour
	{
		#region VARIABLES

		/*
		 private string influencerQuery = 
			"{ " +
					"node(id: \"UNIQUEUSERGID\") { " +
					"... on Influencer { " +
						"videos { " +
							"edges { " +
								"node { " +
									"streamUrl " +
								"} " +
							"} " +
						"} " +
					"} " +
				"} " +
			"}";
		*/
		private string influencerQuery = @"
		{
			node(id: ""UNIQUEUSERGID"") {
			  ... on Influencer {
				videos {
				  edges {
				    node {
				        streamUrl
						uniqueId
						gid
						name
						thumbnailUrl
						description
						createdAt
						userGid
						displayType
						customMetadata
				    }
				  }
				}
			  }
			}
		}";

		private QueryParser queryParser = new QueryParser();

		public VideoPanelLoader videoPanel;
		public InfluencerProfileLoader profilePanel;

		public ScrollManager creatorVideoScrollRect;

		#endregion

		#region UNITY MONO METHODS


		#endregion

		#region PUBLIC METHODS

		public void SetupInfluencerPage(InfluencerDetail influencer) {
			influencerQuery = influencerQuery.Replace ("\"UNIQUEUSERGID\"", "\"" + influencer.gid + "\"");

			// Get all the videos from the query
			string videoListQueryUrl = QueryBuilder.Instance().BuildQuery(influencerQuery);
		
			// get the json from the query
			string videoListQueryJson = QueryBuilder.Instance().GetQueryString(videoListQueryUrl);

			// Get the video list from the json
			// List<VideoDetail> influencerVideoList = queryParser.GetVideoList(videoListQueryJson);
			List<VideoEdges> influencerVideoList = JsonUtilityHelper.instance.GetInfluencerVideoList(videoListQueryJson);

			// Add influencer details to the videos
			// This is necessary for loading details in video player
			foreach (VideoEdges edge in influencerVideoList) {
				VideoDetail video = edge.node;
				video.SetUserHandle (influencer.handle);
				video.SetUserPicUrl (influencer.picUrl);
				video.SetUserGid (influencer.gid);
			}

			// Setup the panels
			videoPanel. SetupVideos(influencerVideoList);
			profilePanel.SetupInfluencerProfile(influencer);
		}

		#endregion

		#region OPITIMIZED SCROLL METHODS
		// public VideoDetailParams videoGridParams;
		VideoScrollRectItemsAdapter videoGridAdapter;

		public IEnumerator SetupInfluencerPageOptimized(InfluencerDetail influencer) {

			influencerQuery = influencerQuery.Replace ("\"UNIQUEUSERGID\"", "\"" + influencer.gid + "\"");

			// Get all the videos from the query
			string videoListQueryUrl = QueryBuilder.Instance().BuildQuery(influencerQuery);

			// get the json from the query
			//string videoListQueryJson = QueryBuilder.Instance().GetQueryString(videoListQueryUrl);
			WWW www = new WWW (videoListQueryUrl);
			yield return www;

			if (www.error == null) {
				string videoListQueryJson = www.text;

				// Get the video list from the json
				// List<VideoDetail> influencerVideoList = queryParser.GetVideoList(videoListQueryJson);
				List<VideoEdges> influencerVideoList = JsonUtilityHelper.instance.GetInfluencerVideoList (videoListQueryJson);

                // Setup video ids for playlist
                int id = 0;

				// Add influencer details to the videos
				// This is necessary for loading details in video player
				foreach (VideoEdges edge in influencerVideoList) {
                    edge.node.SetIdInPlaylist(id++);
					VideoDetail video = edge.node;
					video.SetUserHandle (influencer.handle);
					video.SetUserPicUrl (influencer.picUrl);
					video.SetUserGid (influencer.gid);
				}

				// Setup the panels
				profilePanel.SetupInfluencerProfile (influencer);

                creatorVideoScrollRect.InitializeVideoAdapter (influencerVideoList);

				VideoPlaylistManager.Instance ().AddPlaylistToStack (influencerVideoList);
				/*
			// Setup the video list
			videoGridAdapter = new VideoScrollRectItemsAdapter();
			videoGridAdapter.Init (videoGridParams);
			videoGridAdapter.ChangeModels (influencerVideoList.ToArray());*/
			}
			// StartCoroutine (DelayedInitVideos (influencerVideoList));
		}

		void OnDestroy() {
			if (videoGridAdapter != null)
				videoGridAdapter.Dispose ();
		}

		#endregion
	}
}
