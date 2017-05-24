//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using frame8.Logic.Misc.Other.Extensions;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using UnityEngine.UI;

public class CreatorItemsViewHolder : CellViewHolder 
{
	public RawImage infPicture;
	public Image infPicProgressBar;
	public Text infHandle;
	public Button btnInfluencer;

	public override void CollectViews() {
		base.CollectViews ();

		infPicture = views.Find ("InfPicture/InfPicture").GetComponent<RawImage> ();
		infPicProgressBar = views.Find ("InfPicture/InfProgressBar").GetComponent<Image> ();
		infHandle = views.Find ("InfPicture/InfHandle").GetComponent<Text> ();
		btnInfluencer = views.Find ("InfPicture").GetComponent<Button> ();

	}

	protected override RectTransform GetViews() {
		return root.Find ("Views") as RectTransform;
	}
}

