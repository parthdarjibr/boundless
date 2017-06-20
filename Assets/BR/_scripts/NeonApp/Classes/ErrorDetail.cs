//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//

using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BR.App {
	public class ErrorDetail
	{
        public enum ErrorType
        {
            STARTUP,
            INFLUENCERCLICK,
            VIDEOCLICK,
            INVIDEO
        }

		public enum ResponseType
		{
			ACCEPT,
			DECLINE,
			CANCEL,
			RETRY,
			EXIT,
            SETTINGS,
            RESTART,
            IGNORE
		}

		public Dictionary<ResponseType, UnityAction> ResponseDict {
			get;
			private set;
		}

		public string errorTitle {
			get;
			private set;
		}

		public string errorDescription {
			get;
			private set;
		}

		/*
		public List<ResponseType> responseType {
			get;
			private set;
		}*/

		public void SetErrorTitle(string title) {
			errorTitle = title;
		}

		public void SetErrorDescription(string desc) {
			errorDescription = desc;
		}

		/*
		public void SetResponseTypes(List<ResponseType> responses) {
			responseType = responses;
		}*/

		public void AddToDictionary(ResponseType response, UnityAction act) {
			if (ResponseDict == null)
				ResponseDict = new Dictionary<ResponseType, UnityAction> ();

			ResponseDict.Add (response, act);
		}

		public void ClearDictionary() {
			ResponseDict.Clear ();
		}
	}
}

