//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details: This class stores details about the user if they're logged in
//

using System;

namespace BR.App {
	public class UserDetail
	{

		#region VARIABLES
		public bool isLoggedIn {
			get;
			private set;
		}

		public int userID {
			get;
			private set;
		}

		public string userName {
			get;
			private set;
		}

		public string firstName {
			get;
			private set;
		}

		public string lastName {
			get;
			private set;
		}

		public string email {
			get;
			private set;
		}

		public string privateKey {
			get;
			private set;
		}

		public int numFollowing {
			get;
			private set;
		}
		#endregion

		#region SETTER METHODS

		public void SetIsLoggedIn(bool state) {
			isLoggedIn = state;
		}

		public void SetUserID(int id) {
			userID = id;
		}

		public void SetUserName(string username) {
			userName = username;
		}

		public void SetFirstName(string fname) {
			firstName = fname;
		}

		public void SetLastName(string lname) {
			lastName = lname;
		}

		public void SetEmail(string _email) {
			email = _email;
		}

		public void SetPrivateKey(string key) {
			privateKey = key;
		}

		public void SetFollowingCount(int count) {
			numFollowing = count;
		}

		#endregion
	}
}


