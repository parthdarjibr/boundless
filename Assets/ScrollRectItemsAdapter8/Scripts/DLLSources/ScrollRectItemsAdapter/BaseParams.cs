using frame8.Logic.Misc.Other.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter
{
    /// <summary>
    /// <para>This can easily be used as a Monobehaviour's field and populate it with predefined data</para>
    /// <para>Or can be manually constructed, depending on your needs</para>
    /// </summary>
    [System.Serializable]
    public class BaseParams
    {
        [Header("Optimizing Process")]
        [Tooltip("How much objects to always keep in memory, no matter what. "+
            "This includes visible items + items in the recycle bin. "+
            " The recycle bin will always have at least one item in it, "+
            "regardless of this setting. Set to -1 or 0 to Detect automatically (Recommended!). "+
            "Change it only if you know what you're doing (usually, it's the estimated number of visible items + 1)"+
            ". Last note: this field will only be considered after the number <visible+in the bin> grows past it")]
        public int minNumberOfObjectsToKeepInMemory = -1;
        //[Tooltip("If true, you won't be asked if an object holder can be "+
        //    "recycled or not (via a callback to IsRecyclable()). Instead, this is decided internally. Better " +
        //    "to leave it off, especially if you don't have too much views with "+
        //    "different sizes, as the default implementation calls IsRecyclable() (which you can override and provide your own logic), which returns TRUE for everything")]
        //public bool onlyRecycleCandidatesWithCompatibleSizes;
        [Tooltip("See BaseParams.UpdateMode enum for full description. The default is ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE and it's the most stable")]
        public UpdateMode updateMode = UpdateMode.ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE;

        [Tooltip("If true: When the last item is reached, the first one appears after it, basically allowing you to scroll infinitely.\n"+
            " Initially intended for things like spinners, but it can be used for anything alike.\n" +
            " It may interfere with other functionalities in some very obscure/complex contexts/setups, so be sure to test the hell out of it.\n"+
            " Also please note that sometimes during dragging the content, the actual looping changes the Unity's internal PointerEventData for the current click/touch pointer id, so if you're also externally tracking the current click/touch, in this case only 'PointerEventData.pointerCurrentRaycast' and 'PointerEventData.position'(current position) are preserved, the other ones are reset to defaults to assure a smooth loop transition. Sorry for the long decription. Here's an ASCII potato: (@)")]
        public bool loopItems;

        [Space(10, order = 0)]
        public ScrollRect scrollRect;
        [Tooltip("If null, the scrollRect is considered to be the viewport")]
        public RectTransform viewport;
        [Tooltip("If null, scrollRect.content is considered to be the content")]
        public RectTransform content;

        [Tooltip("This is used instead of the old way of putting a disabled LayoutGroup component on the content")]
        public RectOffset contentPadding = new RectOffset();
        [Tooltip("This is used instead of the old way of putting a disabled LayoutGroup component on the content")]
        public float contentSpacing;

        //float[] cachedWorldCornersArray1, cachedWorldCornersArray2;

        // ..so it can be serializable
        public BaseParams() { }

        public BaseParams(BaseParams other)
        {
            this.minNumberOfObjectsToKeepInMemory = other.minNumberOfObjectsToKeepInMemory;
            this.updateMode = other.updateMode;
            this.scrollRect = other.scrollRect;
            this.viewport = other.viewport;
            this.content = other.content;
            this.contentPadding = other.contentPadding == null ? new RectOffset() : new RectOffset(contentPadding.left, contentPadding.right, contentPadding.top, contentPadding.bottom);
            this.contentSpacing = other.contentSpacing;
        }

        public BaseParams(ScrollRect scrollRect)
            :this(scrollRect, scrollRect.transform as RectTransform, scrollRect.content)
        {}

        public BaseParams(ScrollRect scrollRect, RectTransform viewport, RectTransform content)
        {
            this.scrollRect = scrollRect;
            this.viewport = viewport ?? scrollRect.transform as RectTransform;
            this.content = content ?? scrollRect.content;
        }

        /// <summary>
        /// Make sure to replace null-values with default ones
        /// </summary>
        internal void InitIfNeeded()
        {
            viewport = viewport ?? scrollRect.transform as RectTransform;
            content = content ?? scrollRect.content;

            //cachedWorldCornersArray1 = new float[4];
            //cachedWorldCornersArray2 = new float[4];
        }

        /// <summary>
        /// Uses a different approach when content size is smaller than viewport's size, but yields the same result for ComputeVisibility
        /// You won't need this, as it's for internal use. For general purposes, just use the built-in ScrollRect.normalizedPosition
        /// </summary>
        /// <returns>1 Meaning Start And 0 Meaning End</returns> 
        internal float GetAbstractNormalizedScrollPosition()
        {
            if (scrollRect.horizontal)
            {
                float vpw = viewport.rect.width;
                float cw = content.rect.width;
                if (vpw > cw)
                {
                    //viewport.GetWorldCorners(cachedWorldCornersArray1);
                    //content.GetWorldCorners(cachedWorldCornersArray2);

                    return content.GetInsetFromParentLeftEdge(viewport) / vpw;
                }

                return 1f - scrollRect.horizontalNormalizedPosition;
            }

            float vph = viewport.rect.height;
            float ch = content.rect.height;
            if (vph > ch)
            {
                //viewport.GetWorldCorners(cachedWorldCornersArray1);
                //content.GetWorldCorners(cachedWorldCornersArray2);

                return content.GetInsetFromParentTopEdge(viewport) / vph;
            }

            return scrollRect.verticalNormalizedPosition;
        }

        internal float TransformVelocityToAbstract(Vector2 rawVelocity)
        {
            if (scrollRect.horizontal)
            {
                rawVelocity.x = -rawVelocity.x;

                return rawVelocity.x;
            }

            return rawVelocity.y;
        }

        internal void ScrollToStart()
        {
            if (scrollRect.horizontal)
                scrollRect.horizontalNormalizedPosition = 0f;
            else
                scrollRect.verticalNormalizedPosition = 1f;
        }

        internal void ScrollToEnd()
        {
            if (scrollRect.horizontal)
                scrollRect.horizontalNormalizedPosition = 1f;
            else
                scrollRect.verticalNormalizedPosition = 0f;
        }

        internal void ClampScroll01()
        {
            if (scrollRect.horizontal)
                scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition);
            else
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
        }

        public enum UpdateMode
        {
            // Updates are triggered by a MonoBehaviour.Update() (i.e. each frame the ScrollView is active) and at each OnScroll event
            // Moderate performance when scrolling, but looks best
            ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE,

            // Updates ar triggered by each OnScroll event
            // Experimental. However, if you use it and see no issues, it's recommended over ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE.
            // This is also useful if you don't want the optimizer to use CPU when idle.
            // A bit better performance when scrolling
            ON_SCROLL,

            // Update is triggered by a MonoBehaviour.Update() (i.e. each frame the ScrollView is active)
            // In this mode, some temporary gaps appear when fast-scrolling. If this is not acceptable, use other mode.
            // Best performance when scrolling, items appear a bit delayed when fast-scrolling
            MONOBEHAVIOUR_UPDATE
        }
    }
}
