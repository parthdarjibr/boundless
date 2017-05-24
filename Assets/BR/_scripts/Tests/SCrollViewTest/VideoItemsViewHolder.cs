using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using frame8.Logic.Misc.Other.Extensions;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using frame8.ScrollRectItemsAdapter.Util;

public class VideoItemsViewHolder : CellViewHolder {
	public RawImage thumbnail, infPicture;
	public Text tvVideoName, tvInfluencerName;
	public Image thumbProgress, infPicProgress;
	public Button btnPlay, btnInfluencer, btnThumb;

	public override void CollectViews() {
		base.CollectViews ();

		thumbnail = views.Find ("Thumbnail").GetComponent<RawImage> ();
		thumbProgress = views.Find ("ThumbProgressBar").GetComponent<Image> ();
		tvVideoName = views.Find ("Overlay/VideoName").GetComponent<Text> ();
		btnPlay = views.Find ("Overlay/BtnPlay").GetComponent<Button> ();
		btnThumb = views.GetComponent<Button> ();
		tvInfluencerName = views.Find ("Overlay/InfluencerName").GetComponent<Text> ();
		btnInfluencer = views.Find ("Overlay/InfluencerPicture").GetComponent<Button> ();
		infPicture = views.Find ("Overlay/InfluencerPicture/RawImage").GetComponent<RawImage> ();
		infPicProgress = views.Find ("Overlay/InfluencerPicture/InfProgressBar").GetComponent<Image> ();

	}

	protected override RectTransform GetViews() {
		return root.Find ("Views") as RectTransform;
	}
}
