//
//Code by: Parth Darji
//Company: Boundless Reality
//(c) Boundless Reality, All rights reserved.
//
//Details:
//
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BR.BRUtilities {
	public class QueryBuilder : MonoBehaviour
	{
		#region INSTANTIATION

		private static QueryBuilder instance;

		private static bool Exists() {
			return instance != null;
		}

		public static QueryBuilder Instance() {
			if (!Exists ())
				throw new Exception ("QueryBuilder object not found");
			return instance;
		}

		void Awake() {
			if (instance == null)
				instance = this;
			// DontDestroyOnLoad (this.gameObject);
		}

		void Start() {
			/*
			string str = nodejson.text;
			QueryParser.Instance ().ExecuteNode (str);
			*/
			/*

			string endpoint = "{users { edges { node{ id uniqueId name handle thumbUrl createdAt bio } } }videos { edges { node { id name thumbUrl description createdAt streamUrl duration userId } } } }";

			string queryString = BuildQuery (endpoint);
			Debug.Log (queryString);

			string outJson = "";

			StartCoroutine(ExecuteQuery(queryString));*/
		}

        private void OnDestroy()
        {
            instance = null;
        }
        #endregion

        #region VARIABLES

        public string API_ENDPOINT = "https://api.neonvr.tv/graphql?query=query";
		public string HomeEndPoint = "{users { edges { node{ id uniqueId name handle thumbUrl createdAt bio } } }videos { edges { node { id name thumbUrl description createdAt streamUrl duration userId } } } }";
		public TextAsset nodejson;
		private QueryParser queryParser;

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Returns final query to be made to the server.
		/// </summary>
		/// <returns>The query.</returns>
		/// <param name="url">Query url</param>
		public string BuildQuery(string url) {
			// Get the escape url
			url = WWW.EscapeURL(url);

			string queryURL = API_ENDPOINT + url;
			return queryURL;
		}

		/*
		/// <summary>
		/// Executes the query.
		/// </summary>
		/// <returns>The query.</returns>
		/// <param name="url">URL.</param>
		public IEnumerator ExecuteQuery(string url) {
			WWW www = new WWW (url);

			yield return www;
			if (www.error == null) {
				//QueryParser.Instance ().GetVideoList (www.text);
				queryParser.ExecuteQuery(www.text);
			} else {
				Debug.LogError (www.error);
			}
		}
		*/

		public string GetQueryString(string url) {

			WWW www = new WWW (url);
			while (!www.isDone)
				;

			if (www.error != null) {
				// TODO: Error handler
				return "ERROR";
			} else {
				return www.text;
			}
		}

		#endregion
	}
}