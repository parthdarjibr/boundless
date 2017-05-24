using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using frame8.Logic.Misc.Other.Extensions;


public class SimpleLoopingSpinnerExample : MonoBehaviour
{
    public MyParams adapterParams; // configuration visible in the inspector
 
	// Instance of your ScrollRectItemsAdapter8 implementation
    MyScrollRectAdapter _Adapter;
 
    void Start()
    {
        _Adapter = new MyScrollRectAdapter();
 
        // Need to initialize adapter after a few frames, so Unity will finish initializing its layout
        StartCoroutine(DelayedInit());
    }
 
    void OnDestroy()
    {
		// The adapter has some resources that need to be disposed after you destroy the scroll view
        if (_Adapter != null)
            _Adapter.Dispose();
    }
 
	// Initialize the adapter after 3 frames
	// You can also try calling Canvas.ForceUpdateCanvases() instead if you for some reason can't wait 3 frames, although it wasn't tested
    IEnumerator DelayedInit()
    {
        // Wait 3 frames
        yield return null;
        yield return null;
        yield return null;
 
        _Adapter.Init(adapterParams);
        _Adapter.ChangeItemCountTo(adapterParams.numberOfItems);
    }

    void Update()
    {
        // Parameters are null until Init() is called, so this is an indicator that the adapter was not yet initialized. See DelayedInit() above
        if (_Adapter.Parameters == null)
            return;

        var viewHoldersVisibleItems = _Adapter.VisibleItemsCopy;

        adapterParams.currentSelectedIndicator.text = "Selected: ";
        if (viewHoldersVisibleItems.Count == 0)
            return;

        var middleVH = viewHoldersVisibleItems[viewHoldersVisibleItems.Count / 2];

        adapterParams.currentSelectedIndicator.text += "#" + middleVH.itemIndex + ", value=" + adapterParams.GetItemValueAtIndex(middleVH.itemIndex);
        middleVH.background.color = adapterParams.selectedColor;

        foreach (var other in viewHoldersVisibleItems)
            if (other != middleVH)
                other.background.color = adapterParams.nonSelectedColor;
    }

    #region ScrollRectItemsAdapter8 code
    [Serializable] // serializable, so it can be shown in inspector
    public class MyParams : BaseParams
    {
        public RectTransform prefab;

        public int startItemNumber = 0;
        public int increment = 1;
        public int numberOfItems = 10;

        public Color selectedColor, nonSelectedColor;

        public Text currentSelectedIndicator;

        public int GetItemValueAtIndex(int index) { return startItemNumber + increment * index; }
    }
 
	// Self-explanatory
    public class MyItemViewsHolder : BaseItemViewsHolder
    {
        public Image background;
        public Text titleText;

        public override void CollectViews()
        {
            base.CollectViews();

            background = root.GetComponent<Image>();
            titleText = root.GetComponentInChildren<Text>();
        }
    }
 
    public class MyScrollRectAdapter : ScrollRectItemsAdapter8<MyParams, MyItemViewsHolder>
    {
		// Will be called for vertical scroll views
        protected override float GetItemHeight(int index) { return _Params.prefab.rect.height; }
		// Will be called for horizontal scroll views
        protected override float GetItemWidth(int index) { return _Params.prefab.rect.width; }
 
		// Here's the meat of the whole recycling process
        protected override void InitOrUpdateItemViewHolder(MyItemViewsHolder newOrRecycled)
        {
            if (newOrRecycled.root == null) // container empty => instantiate the prefab in it
            {
                newOrRecycled.root = (GameObject.Instantiate(_Params.prefab.gameObject) as GameObject).transform as RectTransform;
                newOrRecycled.root.gameObject.SetActive(true); // just in case the prefab was disabled
                newOrRecycled.CollectViews();
            }
 
            // Update v2.4: commented, because renaming game objects in a scroll view each frame messes up with some versions of unity and slows performance; 
            //// optionally rename the object
            //newOrRecycled.root.name = "ListItem " + newOrRecycled.itemIndex;

            newOrRecycled.titleText.text = _Params.GetItemValueAtIndex(newOrRecycled.itemIndex) + "";
            newOrRecycled.background.color = Color.white;
        }
    }
    #endregion
 
}
