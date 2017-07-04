using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using frame8.Logic.Misc.Other.Extensions;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using frame8.ScrollRectItemsAdapter.Util;
using BR.App;
using UnityEngine.EventSystems;
using BR.BRUtilities;

public class VideoScrollRectItemsAdapter : GridAdapter<VideoDetailParams, VideoItemsViewHolder>  {
	
	List<VideoEdges> videos = new List<VideoEdges>();
	public int CellCount { get { return videos.Count; } }

	protected override void UpdateCellViewsHolder(VideoItemsViewHolder viewHolder) {
		viewHolder.thumbnail.texture = Parameters.defaultThumbnail;
		var model = videos [viewHolder.itemIndex].node;

		viewHolder.tvVideoName.transform.parent.gameObject.SetActive (true);
		viewHolder.tvVideoName.text = model.name.Replace(" ", "\n");

		int itemIndexAtRequest = viewHolder.itemIndex;
		string requestedPath = model.thumbnailUrl;

        // Check if the url exists in cache dictionary
        Texture2D tex;
        if (CacheManager.instance.GetCachedTexture(requestedPath, out tex))
        {
            viewHolder.thumbProgress.fillAmount = 0f;
            viewHolder.thumbnail.texture = tex;
        }
        else
        {

            var request = new SimpleImageDownloader.Request()
            {
                url = requestedPath,
                onDone = texture =>
                {
                    if (IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, requestedPath))
                    {
                        viewHolder.thumbProgress.fillAmount = 0f;
                        viewHolder.thumbnail.texture = texture.CreateTextureFromReceivedData();
                        CacheManager.instance.AddToDictionary(requestedPath, texture.CreateTextureFromReceivedData());
                    }
                },
                onProgress = progress =>
                {
                    if (IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, requestedPath))
                        viewHolder.thumbProgress.fillAmount = progress;
                },
                onError = () =>
                {
                    if (IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, requestedPath))
                    {
                        Debug.Log("Error downloading thumbnail image");
                    }
                }
            };
            SimpleImageDownloader.Instance.Enqueue(request);
        }

        /*
        SimpleImageDownloader.Instance.Download (
			requestedPath,
			texture => {
				if (IsModelStillValid (viewHolder.itemIndex, itemIndexAtRequest, requestedPath)) {
					viewHolder.thumbnail.texture = texture;
					viewHolder.thumbProgress.fillAmount = 0f;
				}
			},
			() => {
				if (IsModelStillValid (viewHolder.itemIndex, itemIndexAtRequest, requestedPath)) {
					Debug.Log ("Error downloading thumbnail image");
				}
			},
			progress => {
				if (IsModelStillValid (viewHolder.itemIndex, itemIndexAtRequest, requestedPath))
					viewHolder.thumbProgress.fillAmount = 1f - progress;
			}
		);
        */
        // Setup buttons
        // Remove all event handlers to begin with
        viewHolder.btnThumb.onClick.RemoveAllListeners();
		viewHolder.btnPlay.onClick.RemoveAllListeners();

		viewHolder.btnThumb.onClick.AddListener (delegate {
			// Execute the on exit handler
			ExecuteEvents.Execute(viewHolder.btnThumb.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
			VideoButtonClicked(model);
		});

		viewHolder.btnPlay.onClick.AddListener (delegate {
			// Execute the on exit handler
			ExecuteEvents.Execute(viewHolder.btnPlay.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
			VideoButtonClicked(model);
		});

		// Load influencer details only if asked for
		if (Parameters.LoadInfluencerDetails) {

			viewHolder.tvInfluencerName.gameObject.SetActive (true);
			viewHolder.btnInfluencer.gameObject.SetActive (true);
			viewHolder.infPicture.gameObject.SetActive (true);
			viewHolder.infPicProgress.gameObject.SetActive (true);

			viewHolder.tvInfluencerName.text = "@" + model.userHandle;
			string userPicPath = model.userPicUrl;

            Texture2D userTex;
            if (CacheManager.instance.GetCachedTexture(userPicPath, out userTex))
            {
                viewHolder.infPicProgress.fillAmount = 0f;
                viewHolder.infPicture.texture = userTex;
            }
            else
            {
                var creatorRequest = new SimpleImageDownloader.Request()
                {
                    url = userPicPath,
                    onDone = texture =>
                    {
                        if (IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, userPicPath))
                        {
                            viewHolder.infPicProgress.fillAmount = 0f;
                            viewHolder.infPicture.texture = texture.CreateTextureFromReceivedData();
                            CacheManager.instance.AddToDictionary(userPicPath, texture.CreateTextureFromReceivedData());
                        }
                    },
                    onProgress = progress =>
                    {
                        if (IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, userPicPath))
                            viewHolder.infPicProgress.fillAmount = progress;
                    },
                    onError = () =>
                    {
                        if (IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, userPicPath))
                        {
                            Debug.Log("Error downloading influencer image");
                        }
                    }
                };

                SimpleImageDownloader.Instance.Enqueue(creatorRequest);
            }
            /*
            SimpleImageDownloader.Instance.Download (
				userPicPath,
				texture => {
					if (IsModelStillValid (viewHolder.itemIndex, itemIndexAtRequest, userPicPath)) {
						viewHolder.infPicture.texture = texture;
						viewHolder.infPicProgress.fillAmount = 0f;
					}
				},
				() => {
					if (IsModelStillValid (viewHolder.itemIndex, itemIndexAtRequest, userPicPath)) {
						Debug.Log ("Error downloading influencer image");
					}
				},
				progress => {
					if (IsModelStillValid (viewHolder.itemIndex, itemIndexAtRequest, userPicPath))
						viewHolder.infPicProgress.fillAmount = 1f - progress;
				}
			);
            */
            viewHolder.btnInfluencer.onClick.RemoveAllListeners ();
			viewHolder.btnInfluencer.onClick.AddListener (delegate {
				// Execute the on exit handler
				ExecuteEvents.Execute (viewHolder.btnInfluencer.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
				ExecuteEvents.Execute(viewHolder.btnThumb.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
				viewHolder.btnThumb.GetComponent<Animator>().SetTrigger("Normal");
				InfluencerButtonClicked (model);
			});
		}
	}

	// Utilites
	public void Add(params VideoEdges[] newModels) {
		videos.AddRange (newModels);
		ChangeItemCountTo (videos.Count);
	}

	public void Remove(VideoEdges newModel) {
		videos.Add (newModel);
		ChangeItemCountTo (videos.Count);
	}

	public void ChangeModels(VideoEdges[] newModels) {
		videos.Clear ();
		Add (newModels);
	}

	public void Clear() {
		videos.Clear ();
		// ChangeItemCountTo (videos.Count);
	}

	bool IsModelStillValid(int itemIndex, int itemIndexAtRequest, string imageURLAtRequest) {
		return
			videos.Count > itemIndex &&
			itemIndexAtRequest == itemIndex &&
			(imageURLAtRequest == videos [itemIndex].node.thumbnailUrl || imageURLAtRequest == videos[itemIndex].node.userPicUrl);
 
	}


	#region EVENT HANDLERS

	/// <summary>
	/// Click event handler for video buttons
	/// </summary>
	void VideoButtonClicked(VideoDetail currentVideo) {
        // AUDIO IS PLAYED FROM AUDIOPLAYBACKMANAGER
        // Play the audio
        // AudioController.Instance().PlayOneShot(AudioController.Instance().videoClickAudioClip);

        // Handle analytics
        AnalyticsManager.Instance().SendButtonClickAnalytics("video", Parameters.LoadInfluencerDetails ? "videoOnHome" : "videoOnInfluencer", currentVideo.name);

        // Also send the event to the play button
        ApplicationController.Instance ().OpenVideoPlayer (currentVideo);

		// Send pointer exit message to the video button
		// ExecuteEvents.Execute (this.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
	}

	/// <summary>
	/// Click event handler for influencer button
	/// </summary>
	void InfluencerButtonClicked(VideoDetail currentVideo) {
		// Send pointer exit messages to both the video object and the influencer object
		// ExecuteEvents.Execute (this.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);
		// ExecuteEvents.Execute (btnInfluencer.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);

		// Handle analytics
		AnalyticsManager.Instance ().SendButtonClickAnalytics ("creator", "creatorOnVideoThumbnail", currentVideo.userHandle);

		ApplicationController.Instance().OpenInfluencerView(currentVideo.userGid);

		// AUDIO IS PLAYED FROM AUDIOPLAYBACKMANAGER
		// Play the audio
		// AudioController.Instance().PlayOneShot(AudioController.Instance().influencerClickAudioClip);

		/*
			// Open the influencer View
		// Define an InfluencerDetail object and send it to ApplicationManager for instantiation
		InfluencerDetail influencer = new InfluencerDetail();
		influencer.SetGid (currentVideo.userGid);
		influencer.SetDisplayName (currentVideo.userDisplayName);
		influencer.SetPicUrl (currentVideo.userPicUrl);
		//influencer.SetInfluencerBio (currentVideo.influencerBio);

		ApplicationController.Instance().OpenInfluencerView(influencer);
		*/
	}


	#endregion
}
