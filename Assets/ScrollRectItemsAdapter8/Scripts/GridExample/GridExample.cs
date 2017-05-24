using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using frame8.Logic.Misc.Other.Extensions;
using frame8.ScrollRectItemsAdapter.Util.GridView;
using frame8.ScrollRectItemsAdapter.Util;

namespace frame8.ScrollRectItemsAdapter.GridExample
{
    public class GridExample : MonoBehaviour
    {
        public GridParams gridParams; // configuration visible in the inspector
        public Text countText; // holds the number of items which will be fetched from server

        readonly string[] IMAGE_URLS = new string[]
        {
            "https://68.media.tumblr.com/avatar_a6abe54fa29b_128.png",
            "https://a.wattpad.com/useravatar/YasYeas.128.870434.jpg",
            "https://www.lebanoninapicture.com/Prv/Images/Pages/Page_92120/meditation-nature-peace-lebanesemountains-livel-2-22-2017-12-19-17-am-t.jpg",
            "https://www.lebanoninapicture.com/Prv/Images/Pages/Page_96905/purple-flower-green-plants-nature-naturecolors--2-28-2017-4-06-40-pm-t.jpg",
            "https://68.media.tumblr.com/avatar_4b20b991f1fa_128.png",
            "https://s-media-cache-ak0.pinimg.com/236x/cd/de/a2/cddea289ff95c409ce414983f02847b6.jpg",
            "https://images-na.ssl-images-amazon.com/images/I/71L8cVOFuAL._CR204,0,1224,1224_UX128.jpg",
            "https://d2ujflorbtfzji.cloudfront.net/key-image/691c98df-8e89-491f-b7ac-c7ff5a0c441e.png",
            "https://images-na.ssl-images-amazon.com/images/I/71QdMdCSz5L._CR204,0,1224,1224_UX128.jpg",
            "https://a.wattpad.com/useravatar/CorpseDragneel0.128.192903.jpg",
            "http://wiki.teamliquid.net/commons/images/1/18/Crystal_maiden_freezing_field.png",
            "http://icons.iconarchive.com/icons/emoopo/darktheme-folder/128/Folder-Nature-Leave-icon.png",
            "http://icons.iconarchive.com/icons/majid-ksa/nature-folder/128/flower-folder-icon.png"
        };

        // Instance of your ScrollRectItemsAdapter8 implementation
        MyGridAdapter _GridAdapter;


        void Start()
        {
            _GridAdapter = new MyGridAdapter();

            // Need to initialize adapter after a few frames, so Unity will finish initializing its layout
            StartCoroutine(DelayedInit());
        }

        void OnDestroy()
        {
            // The adapter has some resources that need to be disposed after you destroy the scroll view
            if (_GridAdapter != null)
                _GridAdapter.Dispose();
        }

        // Initialize the adapter after 3 frames
        // You can also try calling Canvas.ForceUpdateCanvases() instead if you for some reason can't wait 3 frames, although it wasn't tested
        IEnumerator DelayedInit()
        {
            // Wait 3 frames
            yield return null;
            yield return null;
            yield return null;

            _GridAdapter.Init(gridParams);
        }

        // Callback from UI Button
        // Mocking a basic server communication where you request x items and you receive them
        public void UpdateItems()
        {
            int newCount;
            int.TryParse(countText.text, out newCount);

            // Generating some random models
            var models = new BasicModel[newCount];
            for (int i = 0; i < newCount; ++i)
            {
                models[i] = new BasicModel();
                models[i].title = "Item " + i;
                models[i].imageURL = IMAGE_URLS[UnityEngine.Random.Range(0, IMAGE_URLS.Length)];
            }
            _GridAdapter.ChangeModels(models);
        }

        // Testing the SmoothScrollTo functionality & showing how to transform from item space (i.e. "group" space, i.e. row or column) to cell space 
        public void ScrollToItemWithIndex10()
        {
            if (_GridAdapter != null && _GridAdapter.CellCount > 10)
                _GridAdapter.SmoothScrollTo(gridParams.GetGroupIndex(10), 1f, null);
        }

        // This is your model
        public class BasicModel
        {
            public string title;
            public string imageURL;
        }


        public class MyCellViewHolder : CellViewHolder
        {
            public RawImage icon; // using a raw image because it works with less code when we already have a Texture2D (downloaded from www with SimpleImageDownloader)
            public Image loadingProgress; 
            public Text title;


            public override void CollectViews()
            {
                base.CollectViews();

                icon = views.Find("IconRawImage").GetComponent<RawImage>();
                loadingProgress = views.Find("LoadingProgressImage").GetComponent<Image>();
                title = views.Find("TitleText").GetComponent<Text>();
            }

            protected override RectTransform GetViews() { return root.Find("Views") as RectTransform; }
        }


        #region ScrollRectItemsAdapter8 code
        public class MyGridAdapter : GridAdapter<GridParams, MyCellViewHolder>
        {
            public int CellCount { get { return _Data.Count; } }

            List<BasicModel> _Data = new List<BasicModel>();


            /// <summary> Called when a cell becomes visible </summary>
            /// <param name="viewHolder"> use viewHolder.itemIndex to find your corresponding model and feed data into views and </param>
            protected override void UpdateCellViewHolder(MyCellViewHolder viewHolder)
            {
                var model = _Data[viewHolder.itemIndex];

                viewHolder.icon.enabled = false;
                viewHolder.title.text = "Loading";
                int itemIdexAtRequest = viewHolder.itemIndex;

                string requestedPath = model.imageURL;
                SimpleImageDownloader.Instance.Download(
                    requestedPath,
                    //progress =>
                    //{
                    //    if (IsModelStillValid(viewHolder.itemIndex, itemIdexAtRequest, requestedPath))
                    //        viewHolder.loadingProgress.fillAmount = 1f - progress;
                    //},
                    texture =>
                    {
                        if (IsModelStillValid(viewHolder.itemIndex, itemIdexAtRequest, requestedPath))
                        {
                            viewHolder.title.text = model.title;
                            viewHolder.icon.enabled = true;
                            viewHolder.icon.texture = texture;
                            //viewHolder.loadingProgress.fillAmount = 0f;
                        }
                    },
                    () =>
                    {
                        if (IsModelStillValid(viewHolder.itemIndex, itemIdexAtRequest, requestedPath))
                            viewHolder.title.text = "No connection";
                    }
                );
            }

            // Common utility methods to manipulate the data list
            public void Add(params BasicModel[] newModels)
            {
                _Data.AddRange(newModels);
                ChangeItemCountTo(_Data.Count);
            }
            public void Remove(BasicModel newModel)
            {
                _Data.Add(newModel);
                ChangeItemCountTo(_Data.Count);
            }
            public void ChangeModels(BasicModel[] newModels)
            {
                _Data.Clear();
                Add(newModels);
            }
            public void Clear()
            {
                _Data.Clear();
                ChangeItemCountTo(_Data.Count);
            }

            bool IsModelStillValid(int itemIndex, int itemIdexAtRequest, string imageURLAtRequest)
            {
                return
                    _Data.Count > itemIndex // be sure the index still points to a valid model
                    && itemIdexAtRequest == itemIndex // be sure the view's associated model index is the same (i.e. the viewHolder wasn't re-used)
                    && imageURLAtRequest == _Data[itemIndex].imageURL; // be sure the model at that index is the same (could have changed if ChangeItemCountTo would've been called meanwhile)
            }
        }
        #endregion

    }
}
