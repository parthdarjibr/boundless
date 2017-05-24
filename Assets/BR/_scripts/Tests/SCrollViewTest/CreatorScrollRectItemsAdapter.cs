//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using frame8.Logic.Misc.Other.Extensions;
using System.Collections.Generic;
using frame8.ScrollRectItemsAdapter.Util;
using BR.App;
using BR.BRUtilities;
using UnityEngine.EventSystems;

public class CreatorScrollRectItemsAdapter : GridAdapter<CreatorDetailParams, CreatorItemsViewHolder>
{
	List<InfluencerEdges> influencers = new List<InfluencerEdges>();
	public int CellCount { get { return influencers.Count; } }

	protected override void UpdateCellViewHolder(CreatorItemsViewHolder viewHolder) {
		viewHolder.infPicture.texture = _Params.defaultThumbnail;

		var model = influencers [viewHolder.itemIndex].node;

		viewHolder.infHandle.text = "@" + model.handle;

		int itemIndexAtRequest = viewHolder.itemIndex;

		string requestedPath = model.picUrl;

		SimpleImageDownloader.Instance.Download (
			requestedPath,
			texture => {
				if(IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, requestedPath)) {
					viewHolder.infPicture.texture = texture;
					viewHolder.infPicProgressBar.fillAmount = 0f;
				}
			},
			() => {
				if(IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, requestedPath)) {
					Debug.Log("Cannot download image");
				}
			},
			progress => {
				if(IsModelStillValid(viewHolder.itemIndex, itemIndexAtRequest, requestedPath)) {
					viewHolder.infPicProgressBar.fillAmount = 1f - progress;
				}
			}
		);

		// Setup the button click handlers
		// Remove all handlers before adding them again
		viewHolder.btnInfluencer.onClick.RemoveAllListeners();
		viewHolder.btnInfluencer.onClick.AddListener (delegate {
			ExecuteEvents.Execute (viewHolder.btnInfluencer.gameObject, new PointerEventData (EventSystem.current), ExecuteEvents.pointerExitHandler);

			InfluencerButtonClicked(model);
		});
	}

	// Utilites
	public void Add(params InfluencerEdges[] newModels) {
		influencers.AddRange (newModels);
		ChangeItemCountTo (influencers.Count);
	}

	public void Remove(InfluencerEdges newModel) {
		influencers.Add (newModel);
		ChangeItemCountTo (influencers.Count);
	}

	public void ChangeModels(InfluencerEdges[] newModels) {
		influencers.Clear ();
		Add (newModels);
	}

	public void Clear() {
		influencers.Clear ();
		ChangeItemCountTo (influencers.Count);
	}

	bool IsModelStillValid(int itemIndex, int itemIndexAtRequest, string imageURLAtRequest) {
		return
			influencers.Count > itemIndex &&
			itemIndexAtRequest == itemIndex &&
			imageURLAtRequest == influencers [itemIndex].node.picUrl;
	}

	#region EVENT HANDLERS

	void InfluencerButtonClicked(InfluencerDetail currentInfluencer) {
		AnalyticsManager.Instance ().SendButtonClickAnalytics ("creator", "creatorOnHome", currentInfluencer.displayName);

		// ApplicationController.Instance ().OpenInfluencerView (currentInfluencer);
		ApplicationController.Instance ().OpenInfluencerView (currentInfluencer.gid);
	}

	#endregion
}

