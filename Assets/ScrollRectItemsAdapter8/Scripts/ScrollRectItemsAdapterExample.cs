using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

/*
BRIEF INSTRUCTIONS (It's recommended to manually go through example code in order to fully understand the mechanism)
1. create your own implementation of BaseItemViewsHolder, let's name it MyItemViewsHolder
2. create your own implementation of BaseParams (if needed), let's name it MyParams
3. create your own implementation of ScrollRectItemsAdapter8<MyParams, MyItemViewsHolder>, let's name it MyScrollRectItemsAdapter
4. instantiate MyScrollRectItemsAdapter
5. call MyScrollRectItemsAdapter.ChangeItemCountTo(int count) once (and any time your dataset is changed) and these two things will happen:
    1. 
        if the scroll rect has vertical scrolling (only top-to-bottom is currently supported), MyScrollRectItemsAdapter.GetItemHeight(int index) will be called <count> times (with index going from 0 to <count-1>)
        else if the scroll rect has horizontal scrolling (only left-to-right is currently supported), MyScrollRectItemsAdapter.GetItemWidth(int index) will ...[idem above]...
    2. MyScrollRectItemsAdapter.InitOrUpdateItemViewHolder(MyItemViewsHolder newOrRecycledViewsHolder) will be called ONLY for the items currently visible and each time a new one will become visible:
        - use newOrRecycledViewsHolder.itemIndex to get the item index, so you can retrieve its associated data model from your data set
        - newOrRecycledViewsHolder.root will be null if the item is not recycled. So you need to instantiate your prefab (or whatever), assign it and call newOrRecycledViewsHolder.CollectViews()
        - newOrRecycledViewsHolder.root won't be null if the item is recycled. This means that it's assigned a valid object whose UI elements only need their values changed
        - update newOrRecycledViewsHolder's views from it's associated data model
*/


public class ScrollRectItemsAdapterExample : MonoBehaviour
{
    [SerializeField]
    MyParams _ScrollRectAdapterParams;

    MyScrollRectItemsAdapter _ScrollRectItemsAdapter;
    List<SampleObjectModel> _Data;

    public BaseParams Params { get { return _ScrollRectAdapterParams; } }
    public MyScrollRectItemsAdapter Adapter { get { return _ScrollRectItemsAdapter; } }
    public List<SampleObjectModel> Data { get { return _Data; } }


    IEnumerator Start()
    {
        // Wait for Unity's layout (UI scaling etc.)
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        _Data = new List<SampleObjectModel>();

        int currentItemNum = 0;

        _ScrollRectItemsAdapter = new MyScrollRectItemsAdapter(_Data, _ScrollRectAdapterParams);

        _ScrollRectAdapterParams.updateItemsButton.onClick.AddListener(() =>
        {
            _Data.Clear();
            int capacity = 0;
            int.TryParse(_ScrollRectAdapterParams.numItemsInputField.text, out capacity);

            _Data.Capacity = capacity;

            for (int i = 0; i < capacity; ++i)
            {
                _Data.Add(new SampleObjectModel("Item " + i/* + currentItemNum++*/));
            }

            _ScrollRectItemsAdapter.ChangeItemCountTo(capacity);
        });
    }

    void OnDestroy()
    {
        if (_ScrollRectItemsAdapter != null)
            _ScrollRectItemsAdapter.Dispose();
    }

    // This is your data model
    // this one will generate 5 random colors
    public class SampleObjectModel
    {
        public string objectName;
        public Color aColor, bColor, cColor, dColor, eColor;
        public bool expanded;

        public SampleObjectModel(string name)
        {
            objectName = name;
            aColor = GetRandomColor();
            bColor = GetRandomColor();
            cColor = GetRandomColor();
            dColor = GetRandomColor();
            eColor = GetRandomColor();
        }

        Color GetRandomColor()
        {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }
    }

    public sealed class MyItemsViewHolder : BaseItemViewsHolder
    {
        public Text objectTitle;
        public Image a, b, c, d, e;
        public ExpandCollapseOnClick expandOnCollapseComponent;

        public override void CollectViews()
        {
            base.CollectViews();

            a = root.GetChild(0).GetChild(0).GetComponent<Image>();
            b = root.GetChild(0).GetChild(1).GetComponent<Image>();
            c = root.GetChild(0).GetChild(2).GetComponent<Image>();
            d = root.GetChild(0).GetChild(3).GetComponent<Image>();
            e = root.GetChild(0).GetChild(4).GetComponent<Image>();

            objectTitle = root.GetChild(0).GetChild(5).GetComponentInChildren<Text>();

            expandOnCollapseComponent = root.GetComponent<ExpandCollapseOnClick>();
        }
    }


    [Serializable]
    public class MyParams : BaseParams
    {
        public RectTransform itemPrefab;
        public Toggle randomizeSizesToggle;
        public InputField numItemsInputField;
        public Button updateItemsButton;
    }

    public sealed class MyScrollRectItemsAdapter : ScrollRectItemsAdapter8<MyParams, MyItemsViewHolder>, ExpandCollapseOnClick.ISizeChangesHandler
    {
        bool _RandomizeSizes;
        float _PrefabSize;
        float[] _ItemsSizessToUse;
        List<SampleObjectModel> _Data;

        public MyScrollRectItemsAdapter(List<SampleObjectModel> data, MyParams parms)
        {
            _Data = data;

            if (parms.scrollRect.horizontal)
                _PrefabSize = parms.itemPrefab.rect.width;
            else
                _PrefabSize = parms.itemPrefab.rect.height;

            InitSizes(false);

            parms.randomizeSizesToggle.onValueChanged.AddListener((value) => _RandomizeSizes = value);

            // Need to call Init(Params) AFTER we init our stuff, because both GetItem[Height|Width]() and InitOrUpdateItemViewHolder() will be called in this method
            Init(parms);
        }

        void InitSizes(bool random)
        {
            int newCount = _Data.Count;
            if (_ItemsSizessToUse == null || newCount != _ItemsSizessToUse.Length)
                _ItemsSizessToUse = new float[newCount];

            if (random)
                for (int i = 0; i < newCount; ++i)
                    _ItemsSizessToUse[i] = UnityEngine.Random.Range(30, 400);
            else
                for (int i = 0; i < newCount; ++i)
                    _ItemsSizessToUse[i] = _PrefabSize;
        }

        public override void ChangeItemCountTo(int itemsCount)
        {
            // Keep the mocked heights array's size up to date, so the GetItemHeight(int) callback won't throw an index out of bounds exception
            InitSizes(_RandomizeSizes);

            base.ChangeItemCountTo(itemsCount);
        }

        // Remember, only GetItemHeight (for vertical scroll) or GetItemWidth (for horizontal scroll) will be called
        protected override float GetItemHeight(int index)
        {
            return _ItemsSizessToUse[index];
        }

        // Remember, only GetItemHeight (for vertical scroll) or GetItemWidth (for horizontal scroll) will be called
        protected override float GetItemWidth(int index)
        {
            return _ItemsSizessToUse[index];
        }

        protected override void InitOrUpdateItemViewHolder(MyItemsViewHolder newOrRecycled)
        {
            // Recycling process handling
            if (newOrRecycled.root == null) // new
            {
                // Note: you can use whatever prefab you want... you can have multiple prefabs, you can create/change them dynamically etc. it's up to your imagination and needs :)
                newOrRecycled.root = (GameObject.Instantiate(_Params.itemPrefab.gameObject) as GameObject).GetComponent<RectTransform>();
                newOrRecycled.root.gameObject.SetActive(true); // just in case the prefab started inactive
                newOrRecycled.CollectViews();
                newOrRecycled.root.name = "ListItem " + newOrRecycled.itemIndex;
            }

            // Update v2.4: commented, because renaming game objects in a scroll view each frame messes up with some versions of unity and slows performance; 
            //// optionally rename the object
            //newOrRecycled.root.name = "ListItem " + newOrRecycled.itemIndex;

            // Populating with data from associated model
            SampleObjectModel dataModel = _Data[newOrRecycled.itemIndex];
            newOrRecycled.objectTitle.text = dataModel.objectName + " [Size:" + _ItemsSizessToUse[newOrRecycled.itemIndex] + "]";
            newOrRecycled.a.color = dataModel.aColor;
            newOrRecycled.b.color = dataModel.bColor;
            newOrRecycled.c.color = dataModel.cColor;
            newOrRecycled.d.color = dataModel.dColor;
            newOrRecycled.e.color = dataModel.eColor;

            if (newOrRecycled.expandOnCollapseComponent)
            {
                newOrRecycled.expandOnCollapseComponent.sizeChangesHandler = this;
                newOrRecycled.expandOnCollapseComponent.expanded = dataModel.expanded;
                if (!dataModel.expanded)
                    newOrRecycled.expandOnCollapseComponent.nonExpandedSize = _ItemsSizessToUse[newOrRecycled.itemIndex];
            }
        }

        #region ExpandCollapseOnClick.ISizeChangesHandler implementation
        bool ExpandCollapseOnClick.ISizeChangesHandler.HandleSizeChangeRequest(RectTransform rt, float newSize)
        {
            var vh = GetItemViewsHolderIfVisible(rt);

            // If the vh is visible and the request is accepted, we update our list of sizes
            if (vh != null)
            {
                float resolvedSize = RequestChangeItemSizeAndUpdateLayout(vh, newSize);
                if (resolvedSize != -1f)
                {
                    _ItemsSizessToUse[vh.itemIndex] = newSize;

                    return true;
                }
            }

            return false;
        }

        public void OnExpandedStateChanged(RectTransform rt, bool expanded)
        {
            var vh = GetItemViewsHolderIfVisible(rt);

            // If the vh is visible and the request is accepted, we update the model's "expanded" field
            if (vh != null)
                _Data[vh.itemIndex].expanded = expanded;
        }
        #endregion
    }
}
