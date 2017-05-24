//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using BR.BRUtilities;
using System;
using BR.App;

public class JsonUtilityTest : MonoBehaviour {
	QueryParser queryParser = new QueryParser();
	private string HomePageQuery = @"
		{
  			featuredInfluencers {
				edges{
					node {
						gid
						displayName
						handle
						picUrl
						id
						bio
						showInView
					}
				}
			}
			featuredVideos {
				edges {
					node {
						uniqueId
						gid
						name
						thumbnailUrl
						description
						createdAt
						streamUrl
						userGid
						displayType
						customMetadata
            			order
					}
				}
			}
		}
		";

	public TextAsset videosJson;

	// Use this for initialization
	void Start () {
		string homeQueryUrl = QueryBuilder.Instance ().BuildQuery (HomePageQuery);
		
		string homeQueryJson = QueryBuilder.Instance ().GetQueryString (homeQueryUrl);

		var data = JsonUtility.FromJson<DataRoot> (homeQueryJson);
		Debug.Log (data.data.featuredInfluencers.edges [0].node.displayName);

		// Debug.Log (data.data.featuredVideos.edges[0].node.customMetadata);
		// Debug.Log (data.data.featuredVideos.edges[0].node.customMetadata);
	}


}