//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//

using System;

namespace BR.App {
	[Serializable]
	public class InfluencerDetail
	{
		#region Member Variables

		public string gid;
		public string displayName;
		public string handle;
		public string picUrl;
		public string id;
		public string bio;
		public bool showInView;

		public int order;

		#endregion

		#region Setter Methods

		public void SetDisplayName(string _displayName) {
			displayName = _displayName;
		}

		public void SetPicUrl(string _picUrl) {
			picUrl = _picUrl;
		}

		public void SetBio(string _bio) {
			bio = _bio;
		}

		public void SetShowInView(bool _show) {
			showInView = _show;
		}

		public void SetOrder(int _order) {
			order = _order;
		}

		public void SetHandle(string _handle) {
			handle = _handle;
		}

		public void SetGid(string _gid) {
			gid = _gid;
		}
		#endregion
	}
}
