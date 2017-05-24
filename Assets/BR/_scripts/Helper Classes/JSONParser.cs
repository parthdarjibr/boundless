using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using CielaSpike;
using BR.App;

namespace BR.BRUtilities {
	public class JSONParser {

		#region Events and Delegates
		public delegate void VideoListDownloadComplete(List<VideoDetail> videos);
		public delegate void InfluencerListDownloadComplete (List<InfluencerDetail> videos);

		public static event VideoListDownloadComplete onVideoListDownloadComplete;
		public static event InfluencerListDownloadComplete onInfluencerListDownloadComplete;
		#endregion

		#region Variables

		DownloadUtility downloadUtility = new DownloadUtility ();
		VideoDetail currentVideo;

		#endregion

		#region Public Methods

		/// <summary>
		/// Public method to get a list of videos from a json file
		/// </summary>
		/// <returns>Raises an event when download is complete.</returns>
		/// <param name="url">JSON URL</param>
		public IEnumerator GetVideoList(string url) {
			// Get the video data from URL
			string JSONString = downloadUtility.GetStringSync(url);

			if (string.IsNullOrEmpty (JSONString)) {
				if (onVideoListDownloadComplete != null) {
					onVideoListDownloadComplete (null);
				}
			} else {
				JSONObject obj = new JSONObject (JSONString);
				List<VideoDetail> videos = GetVideoList (obj);
				if (onVideoListDownloadComplete != null) {
					onVideoListDownloadComplete (videos);
				}
			}
			yield return null;
		}

		public IEnumerator GetInfluencerList(string url) {
			// Get the influencer data from URL
			string JSONString = downloadUtility.GetStringSync(url);

			// Process the json
			if (string.IsNullOrEmpty (JSONString)) {
				if (onInfluencerListDownloadComplete != null) {
					onInfluencerListDownloadComplete (null);
				}
			} else {
				JSONObject obj = new JSONObject (JSONString);
				List<InfluencerDetail> influencers = GetInfluencerList (obj);
				if (onInfluencerListDownloadComplete != null) {
					onInfluencerListDownloadComplete (influencers);
				}
			}
			yield return null;
		}

		#endregion

		#region Helper methods

		/// <summary>
		/// Gets the video list from json object.
		/// </summary>
		/// <returns>The video list</returns>
		/// <param name="obj">JSON object with videos list</param>
		public List<VideoDetail> GetVideoList(JSONObject obj) {

			// This is the json for videos list
			// Parse the string in background
			List<VideoDetail> videos = new List<VideoDetail>();
			VideoDetail currVideo = new VideoDetail ();

			for (int i = 0; i < obj.Count; i++) {
				if (obj.keys [i] != null) {
					switch ((string)obj.keys [i]) {
					case "videos":
						videos = GetVideoList (obj.list [i]);
						break;
					case "video":
						JSONObject videosList = obj.list [0];
						int videoIdInPlaylist = 0;
						foreach (JSONObject j in videosList.list) {
							currVideo = ParseVideoDetail (j);
							currVideo.SetIdInPlaylist (videoIdInPlaylist++);
							videos.Add (currVideo);
						}
						break;		
					}
				}
			}
			return videos;
		}

		/// <summary>
		/// Parses the video detail.
		/// </summary>
		/// <returns>The video detail.</returns>
		/// <param name="obj">JSON object for individual video.</param>
		// TODO: Change to private after backend integration
		public VideoDetail ParseVideoDetail(JSONObject obj) {
			VideoDetail video = new VideoDetail ();
			for (int i = 0; i < obj.Count; i++) {
				if (obj.keys [i] != null) {
					switch ((string)obj.keys [i]) {
					case "node":
						// root of the object
						// Parse objects in children
						return ParseVideoDetail(obj.list[i]);
					case "videoID":
						video.SetGid (obj.list [i].str);
						break;
					case "videoName":
						video.SetName (obj.list [i].str);
						break;
					case "videoURL":
						video.SetStreamUrl (obj.list [i].str);
						break;
					case "videoThumbURL":
						video.SetThumbnailUrl (obj.list [i].str);
						break;
					case "videoDescription":
						video.SetDescription (obj.list [i].str);
						break;
					case "timeUploaded":
						video.SetCreatedAt (obj.list [i].str);
						break;
					
					case "userGid":
						video.SetUserGid (obj.list [i].str);
						break;
					case "displayType":
						video.SetType (obj.list [i].str);
						break;
					default:
						break;
					}
				}
			}
			return video;
		}

		/// <summary>
		/// Gets the influencer list from json object.
		/// </summary>
		/// <returns>The influencer list.</returns>
		/// <param name="obj">JSON object with influencers list.</param>
		// TODO: Change to private after backend integration
		public List<InfluencerDetail> GetInfluencerList(JSONObject obj) {
			List<InfluencerDetail> influencerList = new List<InfluencerDetail> ();
			InfluencerDetail currInfluencer = new InfluencerDetail ();

			for (int i = 0; i < obj.Count; i++) {
				if (obj.keys != null) {
					switch ((string)obj.keys [i]) {
					case "influencers":
						influencerList = GetInfluencerList (obj.list [i]);
						break;
					case "influencer":
						JSONObject influencerArray = obj.list [0];
						foreach (JSONObject j in influencerArray.list) {
							currInfluencer = ParseInfluencerDetail (j);
							influencerList.Add (currInfluencer);
						}
						break;
					}
				}
			}
			return influencerList;
		}

		/// <summary>
		/// Parses the influencer detail.
		/// </summary>
		/// <returns>The influencer detail.</returns>
		/// <param name="obj">JSON object for individual influencer.</param>
		public InfluencerDetail ParseInfluencerDetail(JSONObject obj) {
			InfluencerDetail influencer = new InfluencerDetail ();
			for (int i = 0; i < obj.Count; i++) {
				if (obj.keys [i] != null) {
					string key = (string)obj.keys [i];
					switch (key) {
					case "influencer":
						return ParseInfluencerDetail (obj.list [0]);
					case "influencerID":
						influencer.SetGid (obj.list [i].str);
						break;
					case "influencerName":
						influencer.SetDisplayName (obj.list [i].str);
						break;
					case "influencerPicURL":
						influencer.SetPicUrl (obj.list [i].str);
						break;
					case "influencerBio":
						influencer.SetBio (obj.list [i].str);
						break;
					}
				}
			}
			return influencer;
		}

		/// <summary>
		/// Parses the user detail from json object
		/// </summary>
		/// <returns>The user detail.</returns>
		/// <param name="obj">JSON object with influencers list</param>
		public UserDetail ParseUserDetail(JSONObject obj) {
			UserDetail user = new UserDetail ();
			for (int i = 0; i < obj.Count; i++) {
				if (obj.keys [i] != null) {
					string key = (string)obj.keys [i];
					switch (key) {
					case "user":
						return ParseUserDetail (obj.list [0]);
					case "userID":
						user.SetUserID ((int)obj.list [i].i);
						break;
					case "userName":
						user.SetUserName (obj.list [i].str);
						break;
					case "firstName":
						user.SetFirstName (obj.list [i].str);
						break;
					case "lastName":
						user.SetLastName (obj.list [i].str);
						break;
					case "email":
						user.SetEmail (obj.list [i].str);
						break;
					case "numFollowing":
						user.SetFollowingCount ((int)obj.list [i].i);
						break;
					}
				}
			}
			return user;
		}
			
		#endregion
	}
}

