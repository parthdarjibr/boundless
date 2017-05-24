using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BR.BRUtilities;
using CielaSpike;
using BR.App;

public class DelegateTest : MonoBehaviour {
	
	string url = "https://gist.githubusercontent.com/parthdarjibr/60c4ada91283cb2bf3e11dce66f4cf86/raw/0ccc966b55178bd0283e686242711c2ef22dd1bf/videos.json";
	string influencerURL = "https://gist.githubusercontent.com/parthdarjibr/1c4ad4811575a07622cbe9e1f3ac53ee/raw/c80f2e5b757b788570ab371c94261c4e797071c7/InfluencerList.json";
	JSONParser parser = new JSONParser();

	// Use this for initialization
	void Start () {
		JSONParser.onVideoListDownloadComplete += VideosDownloaded;
		JSONParser.onInfluencerListDownloadComplete += InfluencerListDownloaded;

		this.StartCoroutineAsync (parser.GetInfluencerList (influencerURL));
		this.StartCoroutineAsync(parser.GetVideoList(url));
	}

	void VideosDownloaded(List<VideoDetail> videos) {
		JSONParser.onVideoListDownloadComplete -= VideosDownloaded;
		Debug.Log (videos.Count);
	}

	void InfluencerListDownloaded(List<InfluencerDetail> influencers) {
		JSONParser.onInfluencerListDownloadComplete -= InfluencerListDownloaded;
		Debug.Log (influencers.Count);
	}
}
