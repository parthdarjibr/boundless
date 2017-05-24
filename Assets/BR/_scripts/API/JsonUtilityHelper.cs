//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace BR.App {
	public class JsonUtilityHelper : MonoBehaviour
	{

		#region INITIALIZATION

		private static JsonUtilityHelper _instance;

		public static JsonUtilityHelper instance {
			get {
				if (_instance == null)
					throw new Exception ("JsonUtilityhelper object not found");
				return _instance;
			}
		}

		void Awake() {
			if (_instance == null)
				_instance = this;

			DontDestroyOnLoad (this.gameObject);
		}

        private void OnDestroy()
        {
            _instance = null;
        }
        #endregion

        #region PUBLIC METHODS

        public List<VideoEdges> GetInfluencerVideoList(string jsonStr) {
			if (string.IsNullOrEmpty (jsonStr)) {
				Debug.LogError ("Empty json");
				return null;
			}

			InfluencerVideoDataRoot data = JsonUtility.FromJson<InfluencerVideoDataRoot> (jsonStr);
			return data.data.node.videos.edges;
		}

		public void GetHomeLists(string jsonStr, out List<VideoEdges> videoList, out List<InfluencerEdges> influencerList, out Dictionary<string, InfluencerDetail> dict) {
			videoList = new List<VideoEdges> ();
			int id = 0;
			influencerList = new List<InfluencerEdges> ();
			dict = new Dictionary<string, InfluencerDetail> ();

			DataRoot data = JsonUtility.FromJson<DataRoot> (jsonStr);

			// Setup the videos
			videoList = data.data.featuredVideos.edges;
			foreach (VideoEdges edge in data.data.featuredVideos.edges) {
				edge.node.SetIdInPlaylist (id++);
				edge.node.SetCategories (edge.node.customMetadata);
			}

			// Setup influencers
			influencerList = data.data.featuredInfluencers.edges;
			foreach (InfluencerEdges edge in data.data.featuredInfluencers.edges) {
				dict.Add (edge.node.gid, edge.node);
			}
		}

		/*
		public List<VideoEdges> GetVideoList(string jsonStr, bool overwrite = false) {
			if (string.IsNullOrEmpty (jsonStr)) {
				Debug.LogError ("Empty json");
				return null;
			}

			// Get the data root
			GetJsonData(jsonStr, overwrite);

			// Data should be populated by the time execution reaches here
			// Setup categories for the videos
			foreach (VideoEdges edge in data.data.featuredVideos.edges) {
				edge.node.SetCategories (edge.node.customMetadata);
			}

			return data.data.featuredVideos.edges;
		}

		public List<InfluencerEdges> GetInfluencerList(string jsonStr, out Dictionary<string, InfluencerDetail> dict, bool createDict, bool overwrite = false) {
			dict = new Dictionary<string, InfluencerDetail> ();

			if (string.IsNullOrEmpty (jsonStr)) {
				Debug.LogError ("Empty json");
				return null;
			}

			// GEt the data root
			GetJsonData(jsonStr, overwrite);

			// Data should be populated by the time execution reaches here
			// Setup the influencer dictionary
			if (createDict) {
				foreach (InfluencerEdges edge in data.data.featuredInfluencers.edges) {
					dict.Add (edge.node.gid, edge.node);
				}
			}
				
			return data.data.featuredInfluencers.edges;
		}*/
		#endregion
	}
}

