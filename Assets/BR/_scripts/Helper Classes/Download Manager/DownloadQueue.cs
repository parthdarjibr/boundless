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

namespace BR.BRUtilities {
	public class DownloadQueue
	{
		private Queue m_queue;
		private int m_currentDownloadID;
		private bool m_currentStatus;

		public Queue queue {
			get {
				return m_queue;
			}
			set {
				m_queue = value;
			}
		}

		public int currentDownloadID {
			get {
				return m_currentDownloadID;
			}
			set {
				m_currentDownloadID = value;
			}
		}

		public bool currentStatus {
			get {
				return m_currentStatus;
			}
			set {
				m_currentStatus = value;
			}
		}

		public DownloadQueue() {
			queue = new Queue ();
			currentStatus = false;
			currentDownloadID = 0;
		}
	}
}

