//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using System;
using System.Collections.Generic;

namespace BR.App {
    [Serializable]
	public class DataRoot {
		public Data data;
	}

	[Serializable]
	public class Data {
		public FeaturedVideos featuredVideos;
		public FeaturedInfluencers featuredInfluencers;
	}

	[Serializable]
	public class FeaturedVideos {
		public List<VideoEdges> edges;
	}

	[Serializable]
	public class FeaturedInfluencers {
		public List<InfluencerEdges> edges;
	}

	[Serializable]
	public class VideoEdges {
		public VideoDetail node;
	}

	[Serializable] 
	public class InfluencerEdges {
		public InfluencerDetail node;
	}

	[Serializable]
	public class InfluencerVideoDataRoot {
		public InfluencerVideoNode data;
	}

	[Serializable]
	public class InfluencerVideoNode {
		public InfluencerVideoData node;
	}

	[Serializable]
	public class InfluencerVideoData {
		public FeaturedVideos videos;
	}
}

