//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TextureDownloadObject 
{

	public TextureDownloadObject(string _url, Image _img, int _priority) {
		url = _url;
		img = _img;
		isDownloaded = false;
		downloadPriority = _priority;
	}

	public TextureDownloadObject(string _url, Image _img, Image _progressBar, int _priority, Action _action = null) {
	 	url = _url;
		img = _img;
		isDownloaded = false;
		downloadPriority = _priority;
		progressBar = _progressBar;
		action = _action;
	}

	private string m_url;
	private Image m_img;
	private Image m_progressBar;
	private bool m_isDownloaded;
	private int m_downloadPriority;
	private Action m_action;
	public int numberOfTries = 0;

	public string url {
		get { return m_url; }
		set { m_url = value; }
	}

	public Image img {
		get { return m_img; }
		set { m_img = value; }
	}

	public Image progressBar {
		get { return m_progressBar; }
		set { m_progressBar = value; }
	}

	public  bool isDownloaded {
		get { return m_isDownloaded; }
		set { m_isDownloaded = value; }
	}

	public int downloadPriority {
		get{ return m_downloadPriority; }
		set{ m_downloadPriority = value; }
	}

	public Action action {
		get { return m_action; }
		set { m_action = value; }
	}

	public void SetProgressBarValue(float val) {
		progressBar.fillAmount = val;
	}
}
