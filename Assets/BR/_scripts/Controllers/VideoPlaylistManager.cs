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
using System;

namespace BR.App {
	public class VideoPlaylistManager : MonoBehaviour
	{
		#region INSTANTIATION

		private static VideoPlaylistManager _instance;

		private static bool Exists() {
			return _instance != null;
		}

		public static VideoPlaylistManager Instance() {
			if (!Exists ()) {
				throw new Exception ("VideoPlaylistManager object not found!");
			}
			return _instance;
		}

		void Awake() {
			if (_instance == null)
				_instance = this;
			// DontDestroyOnLoad (this.gameObject);

			if (videoListStack == null)
				videoListStack = new Stack ();
		}

        private void OnDestroy()
        {
            _instance = null;
        }
        #endregion

        #region VARIABLES

        public Stack videoListStack;
		private VideoDetail currentVideo;
		private List<VideoEdges> currentVideoList;
		private Dictionary<VideoDetail.Category, List<VideoEdges>> playlistDict;

		#endregion

		#region UNITY MONO METHODS

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Adds the playlist to stack and sets it as the current playlist
		/// </summary>
		/// <param name="videoList">Video list.</param>
		public void AddPlaylistToStack(List<VideoEdges> videoList) {
			if (videoListStack == null)
				videoListStack = new Stack ();

			// Add the new list to the stack
			videoListStack.Push(videoList);

			// Set this list as the current list
			currentVideoList = videoList;
		}

		/// <summary>
		/// Removes the last video list from playlist stack
		/// </summary>
		public void RemoveLastList() {
			// Remove the last object in the stack
			if (videoListStack.Count > 0)
				videoListStack.Pop ();

			if (videoListStack.Count > 0)
				currentVideoList = videoListStack.Peek () as List<VideoEdges>;
			else
				currentVideoList = null;
		}

		/// <summary>
		/// Gets the current video list.
		/// </summary>
		/// <returns>The current video list.</returns>
		public List<VideoEdges> GetCurrentVideoList() {
			return currentVideoList;
		}

		/// <summary>
		/// Gets the next video in current playlist.
		/// </summary>
		/// <returns>The next video in playlist.</returns>
		/// <param name="currentID">Current ID.</param>
		public VideoEdges GetNextVideoInPlaylist(int currentID) {
			if (currentVideoList.Count > currentID + 1)
				return currentVideoList [currentID + 1];

			return null;
		}

		/// <summary>
		/// Adds the playlist to category dictionary.
		/// </summary>
		/// <param name="category">Category.</param>
		/// <param name="playlist">Playlist.</param>
		public void AddPlaylistToDictionary(VideoDetail.Category category, List<VideoEdges> playlist) {
			// Initialize the dictionary if it is null
			if (playlistDict == null)
				playlistDict = new Dictionary<VideoDetail.Category, List<VideoEdges>> ();

			// Add the playlist to the dictionary
			playlistDict.Add(category, playlist);
		}

		/// <summary>
		/// Gets the category playlist from dictionary.
		/// </summary>
		/// <returns>The category playlist from dictionary if it exists. Null otherwise</returns>
		/// <param name="cat">Cat.</param>
		public List<VideoEdges> GetCategoryPlaylistFromDictionary(VideoDetail.Category cat) {
			List<VideoEdges> catList;
			playlistDict.TryGetValue (cat, out catList);
			return catList;
		}

		#endregion
	}
}

