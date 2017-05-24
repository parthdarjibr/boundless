using System;
using System.Collections.Generic;

namespace BR.App {
[Serializable]
public class VideoDetail
	{
		public enum Type {
			MONO,
			STEREO,
			VOLUMETRIC
		}

		public enum Category
		{
			personality,
			funny,
			music,
			all
		}

		#region PROPERTIES

		public int idInPlaylist;
		public string uniqueId;
		public string thumbnailUrl;
		public string name;
		public DateTime createdAt;
		public string gid;
		public string streamUrl;
		public string description;
		public string userGid;
		public int order;
		public Type type;
		public string customMetadata;
		public string userHandle {
			get;
			private set;
		}
		public string userPicUrl {
			get;
			private set;
		}
		public string userDisplayName {
			get;
			private set;
		}
		public List<Category> categories {
			get;
			private set;
		}

		#endregion

		#region SETTER METHODS

		public void SetIdInPlaylist(int _idInPlaylist) {
			idInPlaylist = _idInPlaylist;
		}

		public void SetUniqueId(string _id) {
			uniqueId = _id;
		}

		public void SetThumbnailUrl(string _thumbUrl) {
			thumbnailUrl = _thumbUrl;
		}

		public void SetName(string _name) {
			name = _name;
		}

		public void SetCreatedAt(string _createdAt) {
			createdAt = DateTime.Parse (_createdAt);
		}

		public void SetGid(string _gid) {
			gid = _gid;
		}

		public void SetStreamUrl(string _streamUrl) {
			streamUrl = _streamUrl;
		}

		public void SetDescription(string _description) {
			description = _description;
		}

		public void SetUserGid(string _userGid) {
			userGid = _userGid;
		}

		public void SetOrder(int _order) {
			order = _order;
		}

		public void SetType(string _type) {
			type = (Type)Enum.Parse (typeof(Type), _type.ToUpper ());
			/*
			switch (_type) {
			case "mono":
				type = Type.MONO;
				break;
			case "stereo":
				type = Type.STEREO;
				break;
			case 2:
				type = Type.VOLUMETRIC;
				break;
			default: 
				break;
			}
			*/
		}

		public void SetUserHandle(string _handle) {
			userHandle = _handle;
		}

		public void SetUserPicUrl(string _url) {
			userPicUrl = _url;
		}

		public void SetUserDisplayName(string _name) {
			userDisplayName = _name;
		}

		public void SetCategories(string _categories) {
			List<Category> catList = new List<Category> ();
			if (_categories != null) {
				string[] cats = _categories.Split (',');
				foreach (string cat in cats) {
					switch (cat) {
					case "funny":
						catList.Add (Category.funny);
						break;
					case "personality":
						catList.Add (Category.personality);
						break;
					case "music":
						catList.Add (Category.music);
						break;
					}
				}
			}
			categories = catList;
		}

		#endregion

		#region HELPER FUNCTIONS

		public static VideoDetail copyFrom(VideoDetail vid) {
			VideoDetail newVid = new VideoDetail ();
			newVid.SetIdInPlaylist (vid.idInPlaylist);
			newVid.SetUniqueId (vid.uniqueId);
			newVid.SetThumbnailUrl (vid.thumbnailUrl);
			newVid.SetName (vid.name);
			newVid.SetCreatedAt (vid.createdAt.ToString ());
			newVid.SetGid (vid.gid);
			newVid.SetStreamUrl (vid.streamUrl);
			newVid.SetDescription (vid.description);
			newVid.SetUserGid (vid.userGid);
			newVid.SetOrder (vid.order);
			newVid.SetType (vid.type.ToString());
			newVid.SetUserHandle (vid.userHandle);
			newVid.SetUserPicUrl (vid.userPicUrl);
			newVid.SetUserDisplayName (vid.userDisplayName);

			// Generate the categories string
			string _categories = "";
			foreach (Category c in vid.categories)
				_categories += c.ToString () + ",";

			// Remove the last comma
			_categories = _categories.Substring (0, _categories.Length - 1);
			// Set the categories
			newVid.SetCategories(_categories);

			return newVid;
		}

		#endregion
	}
}