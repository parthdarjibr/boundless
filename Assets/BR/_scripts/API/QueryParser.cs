//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BR.App;

namespace BR.BRUtilities {
	public class QueryParser
	{
		#region PUBLIC METHODS

		/// <summary>
		/// Gets the video list.
		/// </summary>
		/// <returns>The video list.</returns>
		/// <param name="jsonStr">Json string.</param>
		public List<VideoDetail> GetVideoList(string jsonStr) {
			// Get the videos edge
			string videosJSON = GetVideoEdge(jsonStr);

			JSONObject obj = new JSONObject (videosJSON);
			List<VideoDetail> videoList = new List<VideoDetail> ();
			VideoDetail currVideo = new VideoDetail ();

			for (int i = 0; i < obj.Count; i++) {
				switch ((string)obj.keys [i]) {
				case "edges":
					JSONObject videosList = obj.list [0];
					int videoIdInPlaylist = 0;
					foreach (JSONObject j in videosList.list) {
						currVideo = ParseVideoDetail (j);
						currVideo.SetIdInPlaylist (videoIdInPlaylist++);
						videoList.Add (currVideo);
					}
					break;
				}
			}
			return videoList;
		}

		/// <summary>
		/// Gets the influencer list.
		/// </summary>
		/// <returns>The influencer list.</returns>
		/// <param name="jsonStr">Json string.</param>
		 	public  List<InfluencerDetail> GetInfluencerList(string jsonStr, bool createDict, out Dictionary<string, InfluencerDetail> dict) {
			// Initialize the dictionary
			dict = new Dictionary<string, InfluencerDetail>();

			// Get the users edge from the string
			string userJSON = GetUserEdge(jsonStr);

			//Debug.Log (userJSON);

			JSONObject obj = new JSONObject (userJSON);
			List<InfluencerDetail> influencerList = new List<InfluencerDetail> ();
			InfluencerDetail currInfluncer = new InfluencerDetail ();

			for (int i = 0; i < obj.Count; i++) {
				switch((string)obj.keys[i]) {
				case "edges":
					JSONObject influencerListJSON = obj.list [0];
					foreach (JSONObject j in influencerListJSON.list) {
						currInfluncer = ParseInfluencerDetail (j);
						influencerList.Add (currInfluncer);

						// If dictionary asked, add to it
						if (createDict) {
							dict.Add (currInfluncer.gid, currInfluncer);
						}
					}
					break;
				}
			}
			return influencerList;
		}

		#endregion

		#region HELPER FUNCTIONS

		/// <summary>
		/// Gets the string with a videos edge
		/// </summary>
		/// <returns>String with "videos" subtree</returns>
		/// <param name="jsonstr">Original json file</param>
		private string GetVideoEdge(string jsonstr) {
			JSONObject obj = new JSONObject (jsonstr);
			for (int i = 0; i < obj.Count; i++) {
				switch ((string)obj.keys [i]) {
				case "data":
					// This object contains more json objects
					return GetVideoEdge (obj.list [i].ToString());
				case "featuredVideos":
					return obj.list [i].ToString();
				case "videos":
					return obj.list [i].ToString ();
				case "node":
					return GetVideoEdge (obj.list [i].ToString ());
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the string with a users edge
		/// </summary>
		/// <returns>String with "videos" subtree</returns>
		/// <param name="jsonstr">Original json file</param>
		private string GetUserEdge(string jsonstr) {
			JSONObject obj = new JSONObject (jsonstr);
			for (int i = 0; i < obj.Count; i++) {
				switch ((string)obj.keys [i]) {
				case "data":
					// This object contains more json objects
					return GetUserEdge (obj.list [i].ToString());
				case "featuredInfluencers":
					return obj.list [i].ToString();
				}
			}
			return null;
		}

		/// <summary>
		/// Parses the video detail.
		/// </summary>
		/// <returns>The video detail object</returns>
		/// <param name="obj">JSON Object for video array</param>
		private VideoDetail ParseVideoDetail(JSONObject obj) {
			VideoDetail vd = new VideoDetail ();
			for (int i = 0; i < obj.Count; i++) {
				switch ((string)obj.keys [i]) {
				case "node":
					return ParseVideoDetail (obj.list [i]);
				case "uniqueId":
					vd.SetUniqueId (obj.list [i].str);
					break;
				case "createdAt":
					vd.SetCreatedAt (obj.list [i].str);
					break;
				case "description":
					vd.SetDescription (obj.list [i].str);
					break;
				case "gid":
					vd.SetGid (obj.list [i].str);
					break;
				case "name":
					vd.SetName (obj.list [i].str);
					break;
				case "streamUrl":
					vd.SetStreamUrl (obj.list [i].str);
					break;
				case "thumbnailUrl":
					vd.SetThumbnailUrl (obj.list [i].str);
					break;
				case "userGid":
					vd.SetUserGid (obj.list [i].str);
					break;
				case "order":
					vd.SetOrder ((int)obj.list [i].i);
					break;
				case "displayType":
					vd.SetType (obj.list [i].str);
					break;
				case "customMetadata":
					vd.SetCategories (obj.list [i].str);
					break;
				default:
					break;
				}
			}
			return vd;
		}

		/// <summary>
		/// Parses the influencer detail.
		/// </summary>
		/// <returns>The influencer detail.</returns>
		/// <param name="obj">Json object with users array</param>
		private InfluencerDetail ParseInfluencerDetail(JSONObject obj) {
			InfluencerDetail id = new InfluencerDetail ();

			for (int i = 0; i < obj.Count; i++) {
				switch ((string)obj.keys [i]) {
				case "node":
					return ParseInfluencerDetail (obj.list [i]);
				case "gid":
					id.SetGid (obj.list [i].str);
					break;
				case "displayName":
					id.SetDisplayName (obj.list [i].str);
					break;
				case "handle":
					id.SetHandle (obj.list [i].str);
					break;
				case "picUrl":
					id.SetPicUrl (obj.list [i].str);
					break;
				case "bio":
					id.SetBio (obj.list [i].str);
					break;
				case "order":
					id.SetOrder ((int)obj.list [i].n);
					break;
				case "showInView":
					id.SetShowInView (obj.list [i].b);
					break;
				default:
					break;
				}
			}
			return id;
		}

		#endregion
	}
}