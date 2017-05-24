using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using frame8.Logic.Core.MonoBehaviours;
using frame8.Logic.Misc.Other.Extensions;

// NOTE: the vertical/horizontal LayoutGroup on content panel will be disabled
namespace frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter
{
    public abstract class ScrollRectItemsAdapter8<TParams, TItemViewsHolder> : OnScreenSizeChangedEventDispatcher.IOnScreenSizeChangedListener
    where TParams : BaseParams
    where TItemViewsHolder : BaseItemViewsHolder, new()
    {
        public TParams Parameters { get { return _Params; } }
        public List<TItemViewsHolder> VisibleItemsCopy { get { return new List<TItemViewsHolder>(_VisibleItems); } }

        protected TParams _Params;
        protected List<TItemViewsHolder> _VisibleItems;
        protected int _VisibleItemsCount;

        InternalParams _InternalParams;
        MonoBehaviourHelper8 _MonoBehaviourHelper;


        protected ScrollRectItemsAdapter8() { }


        public void Init(TParams parms)
        {
            if (_Params != null)
            {
                if (_Params.scrollRect)
                {
                    // Just to be sure we don't add the listener twice, if initializing with the same scrollview
                    _Params.scrollRect.onValueChanged.RemoveListener(OnScrollPositionChanged);
                }

                if (_MonoBehaviourHelper != null)
                    _MonoBehaviourHelper.Dispose();
            }


            _Params = parms;
            _Params.InitIfNeeded();

            _InternalParams = InternalParams.CreateFromSourceParamsOrThrow(_Params, this);
            // Commented: CreateFromSourceParamsOrThrow() will throw an exception if something's not right, so no need to handle it here
            //if (_InternalParams == null)
            //{
            //    // TODO (in case of error maybe do something else than only returning null)
            //    return;
            //}

            _VisibleItems = new List<TItemViewsHolder>();

            // Need to initialize before ChangeCountInternal, as _MonoBehaviourHelper is referenced there
            _MonoBehaviourHelper = MonoBehaviourHelper8.CreateInstance(MyUpdate, _Params.scrollRect.transform, "OptimizerHelperMonoBehaviour");
            _MonoBehaviourHelper.gameObject.AddComponent<OnScreenSizeChangedEventDispatcher>().RegisterListenerManually(this);

            ChangeItemCountInternal(0, false);
            _Params.ScrollToStart();
            _InternalParams.ResetLastProcessedNormalizedScrollPositionToStart();
            _Params.scrollRect.onValueChanged.AddListener(OnScrollPositionChanged);
        }

        public virtual void ChangeItemCountTo(int itemsCount) { ChangeItemCountTo(itemsCount, false); }

        public void ChangeItemCountTo(int itemsCount, bool contentPanelEndEdgeStationary) { ChangeItemCountInternal(itemsCount, contentPanelEndEdgeStationary); }

        public int GetItemCount() { return _InternalParams.itemsCount; }

        public TItemViewsHolder GetItemViewsHolderIfVisible(int withItemIndex)
        {
            int curVisibleIndex = 0;
            int curIndexInList;
            TItemViewsHolder curItemViewsHolder;
            for (curVisibleIndex = 0; curVisibleIndex < _VisibleItemsCount; ++curVisibleIndex)
            {
                curItemViewsHolder = _VisibleItems[curVisibleIndex];
                curIndexInList = curItemViewsHolder.itemIndex;
                // Commented: with introduction of itemIndexInView, this chek is no longer useful
                //if (curIndexInList > withItemIndex) // the requested item is before the visible ones, so no viewsHolder for it
                //    break;
                if (curIndexInList == withItemIndex)
                    return curItemViewsHolder;
            }

            return null;
        }

        public TItemViewsHolder GetItemViewsHolderIfVisible(RectTransform withRoot)
        {
            TItemViewsHolder curItemViewsHolder;
            for (int i = 0; i < _VisibleItemsCount; ++i)
            {
                curItemViewsHolder = _VisibleItems[i];
                if (curItemViewsHolder.root == withRoot)
                    return curItemViewsHolder;
            }

            return null;
        }

        #region ScrollTo helper methods
        /// <summary>
        /// Aligns the ScrollRect's content so that the item with <paramref name="itemIndex"/> will be at the top
        /// </summary>
        /// <param name="itemIndex"></param>
        public void ScrollTo(int itemIndex)
        {
            float minContentOffsetFromVPAllowed = _InternalParams.viewportSize - _InternalParams.contentPanelSize;
            if (minContentOffsetFromVPAllowed >= 0f)
                return; // can't, because content is not bigger than viewport
            SetContentStartOffsetFromViewportStart(Mathf.Max(minContentOffsetFromVPAllowed , - GetItemOffsetFromParentStart(itemIndex)));
        }

        /// <summary>
        /// Utility to smooth scroll.
        /// However, it's recommended that you use the SmoothScrollProgressCoroutine() call with your own MonoBehaviour.StartCoroutine
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <param name="onProgress">gets the progress and returns if the scrolling should continue</param>
        public void SmoothScrollTo(int itemIndex, float duration, Func<float, bool> onProgress=null)
        {
            _MonoBehaviourHelper.StartCoroutine(SmoothScrollProgressCoroutine(itemIndex, duration, onProgress));
        }

        public IEnumerator SmoothScrollProgressCoroutine(int itemIndex, float duration, Func<float, bool> onProgress=null)
        {
            float minContentOffsetFromVPAllowed = _InternalParams.viewportSize - _InternalParams.contentPanelSize;
            // Positive values indicate CT is smaller than VP, so no scrolling can be done
            if (minContentOffsetFromVPAllowed >= 0f)
            {
                // This is dependent on the case. sometimes is needed, sometimes not
                //if (duration > 0f)
                //    yield return new WaitForSeconds(duration);

                if (onProgress != null)
                    onProgress(1f);
                yield break;
            }

            Canvas.ForceUpdateCanvases();
            _Params.scrollRect.StopMovement();
            float initialInsetFromParent = _Params.content.GetInsetFromParentEdge(_Params.viewport as RectTransform, _InternalParams.startEdge);
            float targetInsetFromParent = Math.Max(minContentOffsetFromVPAllowed, -GetItemOffsetFromParentStart(itemIndex));
            float startTime = Time.time;
            float elapsedTime;
            float progress;
            float value;
            var endOfFrame = new WaitForEndOfFrame();
            bool notCanceled = true;
            do
            {
                yield return null;
                yield return endOfFrame;

                elapsedTime = Time.time - startTime;
                if (elapsedTime >= duration)
                    progress = 1f;
                else
                    // Normal in; sin slow out
                    progress = Mathf.Sin((elapsedTime / duration) * Mathf.PI / 2); ;

                value = Mathf.Lerp(initialInsetFromParent, targetInsetFromParent, progress);

                SetContentStartOffsetFromViewportStart(value);
            }
            while ((onProgress==null || (notCanceled = onProgress(progress))) && progress < 1f);

            // Assures the end result is the expected one
            if (notCanceled)
                ScrollTo(itemIndex);
        }

        /// <summary>
        /// The old way of doing it; the name says it all
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <param name="duration"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        public IEnumerator SmoothScrollProgressCoroutine_FasterButSometimesChoppy(int itemIndex, float duration, Func<float, bool> onProgress = null)
        {
            float normOffset = GetItemNormalizedOffsetFromParentStart_NotAccurate(itemIndex);
            float initialVal = _Params.scrollRect.vertical ? _Params.scrollRect.verticalNormalizedPosition : _Params.scrollRect.horizontalNormalizedPosition;
            float targetVal = normOffset;
            float startTime = Time.time;
            float elapsedTime;
            float progress;
            float value;
            var endOfFrame = new WaitForEndOfFrame();
            do
            {
                yield return null;
                yield return endOfFrame;

                elapsedTime = Time.time - startTime;
                if (elapsedTime >= duration)
                    progress = 1f;
                else
                    // Normal in; sin slow out
                    progress = Mathf.Sin((elapsedTime / duration) * Mathf.PI / 2); ;

                Canvas.ForceUpdateCanvases();
                value = Mathf.Lerp(initialVal, targetVal, progress);
                if (_Params.scrollRect.vertical)
                    _Params.scrollRect.verticalNormalizedPosition = value;
                else
                    _Params.scrollRect.horizontalNormalizedPosition = value;

                //ScrollToInsetFromParent(Mathf.Lerp(initialInsetFromParent, targetInsetFromParent, progress));
            }
            while ((onProgress == null || onProgress(progress)) && progress < 1f);

            // Temporary fix that assures the end result is the expected one
            ScrollTo(itemIndex);
        }

        /// <summary>
        /// <paramref name="offset"/> should be a valid value. See how it's clamped in  <seealso cref="ScrollTo(int)"/>
        /// </summary>
        /// <param name="offset"></param>
        public void SetContentStartOffsetFromViewportStart(float offset) { SetContentEdgeOffsetFromViewportEdge(_InternalParams.startEdge, offset); }

        /// <summary>
        /// <paramref name="offset"/> should be a valid value. See how it's clamped in <seealso cref="ScrollTo(int)"/>
        /// </summary>
        /// <param name="offset"></param>
        public void SetContentEndOffsetFromViewportEnd(float offset) { SetContentEdgeOffsetFromViewportEdge(_InternalParams.endEdge, offset); }
        #endregion

        /// <summary>
        /// <para>Will call GetItem[Height|Width](itemIndex) for each other item to have an updated sizes cache</para>
        /// <para>After, will change the item's size with <newSize> and will shift down/right the next ones, if any</para>
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <param name="requestedSize"></param>
        /// <param name="itemEndEdgeStationary">if to grow to the top/left (less common) instead of down/right (more common)</param>
        /// <returns>the resolved size. This can be slightly different than <requestedSize> slightly if the number of items is huge (>100k))</returns>
        public float RequestChangeItemSizeAndUpdateLayout(TItemViewsHolder withViewHolder, float requestedSize, bool itemEndEdgeStationary=false)
        {
            _Params.scrollRect.StopMovement(); // we don't want a ComputeVisibility() during changing an item's size, so we cut off any inertia 

            int v1_h0 = _Params.scrollRect.vertical ? 1 : 0;
            int vMinus1_h1 = -v1_h0 * 2 + 1;

            float oldSize = _InternalParams.itemsSizes[withViewHolder.itemIndexInView];
            float resolvedSize = _InternalParams.ChangeItemSizeAndUpdateContentSizeAccordingly(withViewHolder, requestedSize, itemEndEdgeStationary);
            float sizeChange = resolvedSize - oldSize;

            // Move all the next visible elements down / to the right
            // iterating from resized+1 to the last in _VisibleItems;
            TItemViewsHolder curVH;
            int i = 0;
            int indexOfResized = _VisibleItems.IndexOf(withViewHolder);
            Vector3 pos;
            for (i = indexOfResized + 1; i < _VisibleItemsCount; ++i)
            {
                curVH = _VisibleItems[i];
                //pos = curVH.root.InverseTransformPoint(curVH.root.position);
                pos = curVH.root.localPosition;
                pos[v1_h0] += vMinus1_h1 * sizeChange;
                //curVH.root.position = curVH.root.TransformPoint(pos);
                curVH.root.localPosition = pos;
            }

            return resolvedSize;
        }

        /// <summary>
        /// <para>returns the distance of the iten's left (if scroll view is Horizontal) or top (if scroll view is Vertical) edge </para>
        /// <para>from the parent's left (respectively, top) edge</para>
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public float GetItemOffsetFromParentStart(int itemIndex)
        {
            return _InternalParams.GetItemOffsetFromParentStartUsingItemIndexInView(_InternalParams.GetItemViewIndexFromRealIndex(itemIndex));
        }

        // Commented: not reliable
        public float GetItemNormalizedOffsetFromParentStart_NotAccurate(int itemIndex)
        {
            return 1 - _InternalParams.GetItemOffsetFromParentStartUsingItemIndexInView(_InternalParams.GetItemViewIndexFromRealIndex(itemIndex)) / _InternalParams.contentPanelSize;
        }


        /// <summary>
        /// <para>This is called automatically when screen size (or the orientation) changes</para>
        /// <para>But if you somehow resize the scrollview manually, you also must call this</para>
        /// </summary>
        public void NotifyScrollViewSizeChanged()
        {
            _InternalParams.CacheScrollViewInfo(); // update vp size etc.
            ChangeItemCountInternal(_InternalParams.itemsCount, false);
            _InternalParams.maxVisibleItemsSeenSinceLastScrollViewSizeChange = 0;
        }

        public virtual void Dispose()
        {
            if (_MonoBehaviourHelper)
                _MonoBehaviourHelper.Dispose();
        }


        /// <summary>
        /// Only called for vertical ScrollRects
        /// </summary>
        /// <param name="index">the element's index in your dataset</param>
        /// <returns>The height to be allocated for its visual representation in the view</returns>
        protected abstract float GetItemHeight(int index);

        /// <summary>
        /// Only called for horizontal ScrollRects
        /// </summary>
        /// <param name="index">the element's index in your dataset</param>
        /// <returns>The width to be allocated for its visual representation in the view</returns>
        protected abstract float GetItemWidth(int index);

        // newOrRecycled.root will be null for new holders
        protected abstract void InitOrUpdateItemViewHolder(TItemViewsHolder newOrRecycled);

        /// <summary>
        /// Self-explanatory. The default implementation returns true each time
        /// </summary>
        /// <param name="fromRecycleBin"></param>
        /// <param name="indexOfItemThatWillBecomeVisible"></param>
        /// <returns></returns>
        protected virtual bool IsRecyclable(TItemViewsHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, float heightOfItemThatWillBecomeVisible)
        {  
           //// Here's what we use <cachedSize> for; since those items are outside the view and the list may have changed, the <itemIndex> property may have become obsolete
           //if (_Params.onlyRecycleCandidatesWithCompatibleSizes && Mathf.Abs(potentiallyRecyclable.cachedSize - nlvSize) > .000001f )
           //    return;

            return true;
        }

        void ChangeItemCountInternal(int itemsCount, bool contentPanelEndEdgeStationary)
        {
            _Params.scrollRect.StopMovement();
            _InternalParams.OnItemsCountChanged(itemsCount, contentPanelEndEdgeStationary);

            // Re-build the content: mark all currentViews as recyclable
            // _RecyclableItems must be zero;
            if (GetNumExcessObjects() > 0)
            {
                throw new UnityException("ChangeItemCountInternal: GetNumExcessObjects() > 0 when calling ChangeItemCountInternal(); this may be due ComputeVisibility not being finished executing yet");
            }

            _InternalParams.recyclableItems.AddRange(_VisibleItems);
            _InternalParams.recyclableItemsCount += _VisibleItemsCount;

            _VisibleItems.Clear();
            _VisibleItemsCount = 0;

            _MonoBehaviourHelper.CallDelayedByFrames(
                    () =>
                    {
                        // Only computing the visibility if it didn't already
                        if (!_InternalParams.onScrollPositionChangedFiredAndVisibilityComputedForCurrentItems)
                        {
                            _Params.ClampScroll01();

                            // On some devices(i.e. some androids) this is not fired on initialization, so we're firing it here
                            //OnScrollPositionChanged(_Params.scrollRect.normalizedPosition);
                            _InternalParams.updateRequestPending = true;
                        }
                    },
                    3
                );
        }

        // Called by MonobehaviourHelper.Update
        void MyUpdate()
        {
            if (_InternalParams.updateRequestPending) 
            {
                // ON_SCROLL is the only case when we don't regularly update and are using only onScroll event for ComputeVisibility
                _InternalParams.updateRequestPending = _Params.updateMode != BaseParams.UpdateMode.ON_SCROLL;
                ComputeVisibilityForCurrentPosition();
            }
        }

        void OnScrollPositionChanged(Vector2 pos)
        {
            //Debug.Log(pos.x + "; " + pos.y);
            //float posXOrY;
            //if (_Params.scrollRect.horizontal)
            //{
            //    posXOrY = pos.x;
            //}
            //else
            //{
            //    posXOrY = pos.y;
            //}

            //if (posXOrY < -.01f || posXOrY > 1.01f) // ignoring the elastic effect of the scrollview
            //    return;

            if (_Params.updateMode != BaseParams.UpdateMode.MONOBEHAVIOUR_UPDATE)
                ComputeVisibilityForCurrentPosition();
            if (_Params.updateMode != BaseParams.UpdateMode.ON_SCROLL)
                _InternalParams.updateRequestPending = true;

            _InternalParams.onScrollPositionChangedFiredAndVisibilityComputedForCurrentItems = true;
        }

        void ComputeVisibilityForCurrentPosition()
        {
            float curPos = _Params.GetAbstractNormalizedScrollPosition();
            float delta = curPos - _InternalParams.lastProcessedAbstractNormalizedScrollPosition;

            if (_Params.loopItems)
                LoopIfNeeded(delta, curPos);

            ComputeVisibility(delta);
            // Important: do not remove this commented line: it's a remainder that the correct way of updating this var is to call 
            // again GetAbstractNormalizedScrollPosition(), since it can change due to LoopIfNeeded() being called.
            // Only using curPos if looping is disabled
            //_InternalParams.lastProcessedAbstractNormalizedScrollPosition = curPos;
            _InternalParams.lastProcessedAbstractNormalizedScrollPosition = _Params.loopItems ? _Params.GetAbstractNormalizedScrollPosition() : curPos;
        }

        void LoopIfNeeded(float delta, float curPos)
        {
            //if (curPos < 0f || curPos > 1f)
            //{
            //    //Debug.Log(curPos);
            //    return;
            //}

            if (_VisibleItems.Count > 0)
            {
                int resetDir = 0;
                if (delta > 0f && curPos > .95f) // scrolling to start (1f) and curPos is somewhat near start
                    // crossed half of the content in the positive dir (up/left) => reset it and re-map heights, cumulatedHeights; re-order items
                    resetDir = 2;
                else if (delta < 0f && curPos < .05f) // scrolling to end (0f) and curPos is somewhat near end
                    // crossed half of the content in the negative dir (down/right)  => reset it and re-map heights, cumulatedHeights; re-order items
                    resetDir = 1;
                else
                    return;

                var velocityBeforeLoop = _Params.scrollRect.velocity;
                int firstVisibleItem_IndexInView = _VisibleItems[0].itemIndexInView, 
                    lastVisibleItem_IndexInView = firstVisibleItem_IndexInView + _VisibleItemsCount -1;

                // Dig into reflection and get the original pointer data
                if (!(EventSystem.current.currentInputModule is PointerInputModule))
                    throw new InvalidOperationException("Cannot use looping with if the current input module does not inherit from PointerInputModule");

                var eventSystemAsPointerInputModule = (EventSystem.current.currentInputModule as PointerInputModule);
                var pointerEvents = eventSystemAsPointerInputModule
                    .GetType()
                    .GetField("m_PointerData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) 
                    .GetValue(eventSystemAsPointerInputModule)
                    as Dictionary<int, PointerEventData>;

                // Modify the original pointer to look like it was pressed at the current position and it didn't move
                PointerEventData originalPED = null;
                foreach (var pointer in pointerEvents.Values)
                {
                    if (pointer.pointerDrag == _Params.scrollRect.gameObject)
                    {
                        pointer.pointerPressRaycast = pointer.pointerCurrentRaycast;
                        pointer.pressPosition = pointer.position;
                        pointer.delta = Vector2.zero;
                        pointer.dragging = false;
                        pointer.scrollDelta = Vector2.zero;
                        // TODO test
                        //pointer.Use();
                        originalPED = pointer;
                        break;
                    }
                }

                RectTransform.Edge edgeToInsetContentFrom;
                float contentNewInsetFromParentEdge;
                if (resetDir == 1)
                {
                    // Already done (this can sometimes mean there are too few items in list)
                    if (firstVisibleItem_IndexInView == 0)
                        return;

                    float firstVisibleItemInsetFromStart = _InternalParams.GetItemOffsetFromParentStartUsingItemIndexInView(firstVisibleItem_IndexInView);
                    float contentInsetFromVPStart = _Params.content.GetInsetFromParentEdge(_Params.viewport, _InternalParams.startEdge);
                    float firstVisibleItemAmountOutside = -contentInsetFromVPStart - firstVisibleItemInsetFromStart;
                    float contentNewInsetFromParentStart = -(firstVisibleItemAmountOutside + _InternalParams.paddingContentStart);
                    contentNewInsetFromParentEdge = contentNewInsetFromParentStart;
                    edgeToInsetContentFrom = _InternalParams.startEdge;
                }
                else
                {
                    // Already done (this can sometimes mean there are too few items in list)
                    if (lastVisibleItem_IndexInView + 1 >= _InternalParams.itemsCount)
                        return;

                    float lastVisibleItemInsetFromStart = _InternalParams.GetItemOffsetFromParentStartUsingItemIndexInView(lastVisibleItem_IndexInView);
                    float lastVisibleItemSize = _InternalParams.itemsSizes[lastVisibleItem_IndexInView];
                    float lastVisibleItemInsetFromEnd = _InternalParams.contentPanelSize - (lastVisibleItemInsetFromStart + lastVisibleItemSize);
                    float contentInsetFromVPEnd = _Params.content.GetInsetFromParentEdge(_Params.viewport, _InternalParams.endEdge);
                    float lastVisibleItemAmountOutside = -contentInsetFromVPEnd - lastVisibleItemInsetFromEnd;
                    float contentNewInsetFromParentEnd = -(lastVisibleItemAmountOutside + _InternalParams.paddingContentEnd);
                    contentNewInsetFromParentEdge = contentNewInsetFromParentEnd;
                    edgeToInsetContentFrom = _InternalParams.endEdge;
                }
                _Params.content.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(_Params.viewport, edgeToInsetContentFrom, contentNewInsetFromParentEdge, _InternalParams.contentPanelSize);
                _Params.scrollRect.Rebuild(CanvasUpdate.PostLayout);
                _Params.scrollRect.velocity = velocityBeforeLoop;

                int newRealIndexOfFirstItemInView;
                if (resetDir == 1)
                {
                    newRealIndexOfFirstItemInView = _VisibleItems[0].itemIndex;

                    // Adjust the itemIndexInView for the visible items. they'll be the last ones, so the last one of them will have, for example, viewIndex = itemsCount-1
                    for (int i = 0; i < _VisibleItemsCount; ++i)
                        _VisibleItems[i].itemIndexInView = i;
                }
                else
                {
                    // The next item after this will become the first one in view
                    newRealIndexOfFirstItemInView = _InternalParams.GetItemRealIndexFromViewIndex(lastVisibleItem_IndexInView + 1);

                    // Adjust the itemIndexInView for the visible items. they'll be the last ones, so the last one of them will have, for example, viewIndex = itemsCount-1
                    for (int i = 0; i < _VisibleItemsCount; ++i)
                        _VisibleItems[i].itemIndexInView = _InternalParams.itemsCount - _VisibleItemsCount + i;
                }
                _InternalParams.OnScrollViewLooped(newRealIndexOfFirstItemInView);

                // Update the positions of the visible items so they'll retain their position relative to the viewport
                TItemViewsHolder vh;
                for (int i = 0; i < _VisibleItemsCount; ++i)
                {
                    vh = _VisibleItems[i];
                    float insetFromStart = _InternalParams.paddingContentStart + vh.itemIndexInView * _InternalParams.spacing;
                    if (vh.itemIndexInView > 0)
                        insetFromStart += _InternalParams.itemsSizesCumulative[vh.itemIndexInView - 1];

                    vh.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(_InternalParams.startEdge, insetFromStart, _InternalParams.itemsSizes[vh.itemIndexInView]);
                }

                _InternalParams.UpdateLastProcessedNormalizedScrollPosition();
            }
        }

        void ComputeVisibility(float abstractDelta)
        {
            /// ALIASES:
            // scroll down = the content goes down(the "view point" goes up); scroll up = analogue
            // the notation "x/y" means "x, if vertical scroll; y, if horizontal scroll"
            // positive scroll = down/right; negative scroll = up/left
            // [start] = usually refers to some point above/to-left-of [end], if negativeScroll; 
            //          else, [start] is below/to-right-of [end]; 
            //          for example: -in context of _VisibleItems, [start] is 0 for negativeScroll and <_VisibleItemsCount-1> for positiveScroll;
            //                       -in context of an item, [start] is its top for negativeScroll and bottom for positiveScroll;
            //                       - BUT for ct and vp, they have fixed meaning, regardless of the scroll sign. they only depend on scroll direction (if vert, start = top, end = bottom; if hor, start = left, end = right)
            // [end] = inferred from definition of [start]
            // LV = last visible (the last item that was closest to the negVPEnd_posVPStart in the prev call of this func - if applicable)
            // NLV = new last visible (the next one closer to the negVPEnd_posVPStart than LV)
            // neg = negative scroll (down or right)
            // pos =positive scroll (up or left)
            // ch = child (i.e. ctChS = content child start(first child) (= ct.top - ctPaddingTop, in case of vertical scroll))

            #region visualization
            // So, again, this is the items' start/end notions! Viewport's and Content's start/end are constant throughout the session
            // Assume the following scroll direction (hor) and sign (neg) (where the VIEWPORT+SCROLLBAR goes, opposed to where the CONTENT goes):
            // hor, negative:
            // O---------------->
            //      [vpStart]  [start]item[end] .. [start]item2[end] .. [start]LVItem[end] [vpEnd]
            // hor, positive:
            // <----------------O
            //      [vpStart]  [end]item[start] .. [end]item2[start] .. [end]LVItem[start] [vpEnd]
            #endregion

            bool negativeScroll = abstractDelta <= 0f;
            bool verticalScroll = _Params.scrollRect.vertical;

            // Viewport constant values
            float vpSize = _InternalParams.viewportSize;

            // Content panel constant values
            float ctSpacing = _InternalParams.spacing,
                  ctPadTransvStart = _InternalParams.transversalPaddingContentStart;

            // Items constant values
            float allItemsTransversalSizes = _InternalParams.itemsConstantTransversalSize;

            // Items variable values
            TItemViewsHolder nlvHolder = null;
            //int currentLVItemIndex;
            int currentLVItemIndexInView;

            float negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV;
            RectTransform.Edge negStartEdge_posEndEdge;
            RectTransform.Edge transvStartEdge = _InternalParams.transvStartEdge;

            //int negEndItemIndex_posStartItemIndex,
            //int endItemIndex, // TODO pending removal
            int endItemIndexInView,
                  neg1_posMinus1,
                  negMinus1_pos1,
                  neg1_pos0,
                  neg0_pos1;

            float neg0_posVPSize;

            if (negativeScroll)
            {
                neg1_posMinus1 = 1;
                negStartEdge_posEndEdge = _InternalParams.startEdge;
            }
            else
            {
                neg1_posMinus1 = -1;
                negStartEdge_posEndEdge = _InternalParams.endEdge;
            }
            negMinus1_pos1 = -neg1_posMinus1;
            neg1_pos0 = (neg1_posMinus1 + 1) / 2;
            neg0_pos1 = 1 - neg1_pos0;
            neg0_posVPSize = neg0_pos1 * vpSize;

            // -1, if negativeScroll
            // _InternalParams.itemsCount, else
            currentLVItemIndexInView = neg0_pos1 * (_InternalParams.itemsCount - 1) - neg1_posMinus1;

            // _InternalParams.itemsCount - 1, if negativeScroll
            // 0, else
            endItemIndexInView = neg1_pos0 * (_InternalParams.itemsCount - 1);

            float negCTInsetFromVPS_posCTInsetFromVPE = _Params.content.GetInsetFromParentEdge(_Params.viewport, negStartEdge_posEndEdge);

            // _VisibleItemsCount is always 0 in the first call of this func after the list is modified 
            if (_VisibleItemsCount > 0)
            {
                /// 
                /// 
                /// startingLV means the item in _VisibleItems that's the closest to the next one that'll spawn
                /// 
                /// 
                /// 



                int startingLVHolderIndex;
                // The item that was the last in the _VisibleItems; We're inferring the positions of the other ones after(below/to the right, depending on hor/vert scroll) it this way, since the heights(widths for hor scroll) are known
                TItemViewsHolder startingLVHolder;
                RectTransform startingLVRT;

                // startingLVHolderIndex will be:
                // _VisibleITemsCount - 1, if negativeScroll
                // 0, if upScroll
                startingLVHolderIndex = neg1_pos0 * (_VisibleItemsCount - 1);
                startingLVHolder = _VisibleItems[startingLVHolderIndex];
                startingLVRT = startingLVHolder.root;

                // Approach name(will be referenced below): (%%%)
                // currentStartToUseForNLV will be:
                // NLV top (= LV bottom - spacing), if negativeScroll
                // NLV bottom (= LV top + spacing), else
                //---
                // More in depth: <down0up1 - startingLVRT.pivot.y> will be
                // -startingLVRT.pivot.y, if negativeScroll
                // 1 - startingLVRT.pivot.y, else
                //---
                // And: 
                // ctSpacing will be subtracted from the value, if negativeScroll
                // added, if upScroll

                // Commented: using a more efficient way of doing this by using cumulative sizes
                //if (verticalScroll)
                //{
                //    float sizePlusSpacing = startingLVRT.rect.height + ctSpacing;
                //    if (negativeScroll)
                //        negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = startingLVRT.GetInsetFromParentTopEdge(_Params.content) + sizePlusSpacing;
                //    else
                //        negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = startingLVRT.GetInsetFromParentBottomEdge(_Params.content) + sizePlusSpacing;
                //}
                //else
                //{
                //    float sizePlusSpacing = startingLVRT.rect.width + ctSpacing;
                //    if (negativeScroll)
                //        negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = startingLVRT.GetInsetFromParentLeftEdge(_Params.content) + sizePlusSpacing;
                //    else
                //        negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = startingLVRT.GetInsetFromParentRightEdge(_Params.content) + sizePlusSpacing;
                //}
                //float sizePlusSpacing = _InternalParams.itemsSizes[startingLVHolder.itemIndex] + ctSpacing;
                //if (negativeScroll)
                //    negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = _InternalParams.GetItemOffsetFromParentStart(startingLVHolder.itemIndex) + sizePlusSpacing;
                //else
                //    negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = _InternalParams.contentPanelSize - _InternalParams.GetItemOffsetFromParentStart(startingLVHolder.itemIndex);

                // Items variable values; initializing them to the current LV
                currentLVItemIndexInView = startingLVHolder.itemIndexInView;

                // Get a list of items that are above(if neg)/below(if pos) viewport and move them from 
                // _VisibleItems to itemsOutsideViewport; they'll be candidates for recycling
                TItemViewsHolder curRecCandidateVH;
                bool currentIsOutside;
                RectTransform curRecCandidateRT;
                float curRecCandidateSizePlusSpacing;
                float insetFromParentEdge;
                while (true)
                {
                    // vItemHolder is:
                    // first in _VisibleItems, if negativeScroll
                    // last in _VisibleItems, else
                    int curRecCandidateVHIndex = neg0_pos1 * (_VisibleItemsCount - 1);
                    curRecCandidateVH = _VisibleItems[curRecCandidateVHIndex];
                    curRecCandidateRT = curRecCandidateVH.root;
                    //float lvSize = _InternalParams.itemsSizes[currentLVItemIndex];
                    curRecCandidateSizePlusSpacing = _InternalParams.itemsSizes[curRecCandidateVH.itemIndexInView] + ctSpacing; // major bugfix: 18.12.2016 1:20: must use vItemHolder.itemIndex INSTEAD of currentLVItemIndex
                    
                    // Commented: using a more efficient way of doing this by using cumulative sizes, even if we need to use an "if"
                    //currentIsOutside = negCTInsetFromVPS_posCTInsetFromVPE + (curRecCandidateRT.GetInsetFromParentEdge(_Params.content, negStartEdge_posEndEdge) + curRecCandidateSizePlusSpacing) <= 0f;
                    if (negativeScroll)
                        insetFromParentEdge = _InternalParams.GetItemOffsetFromParentStartUsingItemIndexInView(curRecCandidateVH.itemIndexInView);
                    else
                        insetFromParentEdge = _InternalParams.GetItemOffsetFromParentEndUsingItemIndexInView(curRecCandidateVH.itemIndexInView);
                    currentIsOutside = negCTInsetFromVPS_posCTInsetFromVPE + (insetFromParentEdge + curRecCandidateSizePlusSpacing) <= 0f;

                    if (currentIsOutside)
                    {
                        _InternalParams.recyclableItems.Add(curRecCandidateVH);
                        _VisibleItems.RemoveAt(curRecCandidateVHIndex);
                        --_VisibleItemsCount;
                        ++_InternalParams.recyclableItemsCount;

                        if (_VisibleItemsCount == 0) // all items that were considered visible are now outside viewport => will need to seek even more below 
                            break;
                    }
                    else
                        break; // the current item is outside(not necessarily completely) the viewport
                }
            }
            //else
            //    negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = neg1_pos0 * _InternalParams.paddingContentStart + neg0_pos1 * _InternalParams.paddingContentEnd;

            do
            {
                // negativeScroll vert/hor: there are no items below/to-the-right-of-the current LV that might need to be made visible
                // positive vert/hor: there are no items above/o-the-left-of-the current LV that might need to be made visible
                if (currentLVItemIndexInView == endItemIndexInView)
                    break;

                // Searching for next item that might get visible: downwards on negativeScroll OR upwards else
                //int nlvIndex = currentLVItemIndexInView; // TODO pending removal
                int nlvIndexInView = currentLVItemIndexInView;
                float nlvSize;
                bool breakBigLoop = false,
                     negNLVCandidateBeforeVP_posNLVCandidateAfterVP; // before vpStart, if negative scroll; after vpEnd, else
                do
                {
                    nlvIndexInView += neg1_posMinus1;
                    if (negativeScroll)
                        negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = _InternalParams.GetItemOffsetFromParentStartUsingItemIndexInView(nlvIndexInView);
                    else
                        negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = _InternalParams.GetItemOffsetFromParentEndUsingItemIndexInView(nlvIndexInView);
                    nlvSize = _InternalParams.itemsSizes[nlvIndexInView];
                    negNLVCandidateBeforeVP_posNLVCandidateAfterVP = negCTInsetFromVPS_posCTInsetFromVPE + (negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV + nlvSize) <= 0f;
                    if (negNLVCandidateBeforeVP_posNLVCandidateAfterVP)
                    {
                        if (nlvIndexInView == endItemIndexInView) // all items are outside viewport => abort
                        {
                            breakBigLoop = true;
                            break;
                        }
                    }
                    else
                    {
                        // Next item is after vp(if neg) or before vp (if pos) => no more items will become visible 
                        // (this happens usually in the first iteration of this loop inner loop, i.e. negNLVCandidateBeforeVP_posNLVCandidateAfterVP never being true)
                        if (negCTInsetFromVPS_posCTInsetFromVPE + negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV > vpSize)
                        {
                            breakBigLoop = true;
                            break;
                        }

                        // At this point, we've found the real nlv: nlvIndex, nlvH and currentTopToUseForNLV(if negativeScroll)/currentBottomToUseForNLV(if upScroll) were correctly assigned
                        break;
                    }
                }
                while (true);

                if (breakBigLoop)
                    break;

                int nlvRealIndex = _InternalParams.GetItemRealIndexFromViewIndex(nlvIndexInView);

                // Search for a recyclable holder for current NLV
                // This block remains the same regardless of <negativeScroll> variable, because the items in <itemsOutsideViewport> were already added in an order dependent on <negativeScroll>
                // (they are always from <closest to [start]> to <closest to [end]>)
                int i = 0;
                TItemViewsHolder potentiallyRecyclable;
                while (true)
                {
                    if (i < _InternalParams.recyclableItemsCount)
                    {
                        potentiallyRecyclable = _InternalParams.recyclableItems[i];
                        if (IsRecyclable(potentiallyRecyclable, nlvRealIndex, nlvSize))
                        {
                            _InternalParams.recyclableItems.RemoveAt(i);
                            --_InternalParams.recyclableItemsCount;
                            nlvHolder = potentiallyRecyclable;
                            break;
                        }
                        ++i;
                    }
                    else
                    {
                        // Found no recyclable view with the requested height
                        nlvHolder = new TItemViewsHolder();
                        break;
                    }
                }

                // Add it in list at [end]
                _VisibleItems.Insert(neg1_pos0 * _VisibleItemsCount, nlvHolder);
                ++_VisibleItemsCount;

                // Update its index
                nlvHolder.itemIndex = nlvRealIndex;
                nlvHolder.itemIndexInView = nlvIndexInView;

                // Cache its height
                nlvHolder.cachedSize = _InternalParams.itemsSizes[nlvIndexInView];

                // Update its views
                InitOrUpdateItemViewHolder(nlvHolder);
                RectTransform nlvRT = nlvHolder.root;

                // Make sure it's GO is activated
                nlvHolder.root.gameObject.SetActive(true);

                // Make sure it's left-top anchored (the need for this arose together with the feature for changind an item's size 
                // (an thus, the content's size) externally, using RequestChangeItemSizeAndUpdateLayout)
                nlvRT.anchorMin = nlvRT.anchorMax = _InternalParams.constantAnchorPosForAllItems;

                // Make sure it's parented to content panel
                nlvRT.SetParent(_Params.content, false);

                //// Though visually not relevant, maybe it helps the UI system 
                //if (negativeScroll) nlvRT.SetAsLastSibling();
                //else nlvRT.SetAsFirstSibling();

                nlvRT.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(negStartEdge_posEndEdge, negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV, nlvSize);

                // Commented: using cumulative sizes
                //negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV += nlvSizePlusSpacing;


                //float itemSizesUntilNowPlusSpacing = _InternalParams.itemsSizesCumulative[nlvIndex] + nlvIndex * ctSpacing;
                //if (negativeScroll)
                //    negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = _InternalParams.paddingContentStart + itemSizesUntilNowPlusSpacing + ctSpacing;
                //else
                //    negCurrentInsetFromCTSToUseForNLV_posCurrentInsetFromCTEToUseForNLV = _InternalParams.paddingContentStart + itemSizesUntilNowPlusSpacing + ctSpacing;

                // Assure transversal size and transversal position based on parent's padding and width settings
                nlvRT.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(transvStartEdge, ctPadTransvStart, allItemsTransversalSizes);

                currentLVItemIndexInView = nlvIndexInView;
            }
            while (true);

            // Keep track of the <maximum number of items that were visible since last scroll view size change>, so we can optimize the object pooling process
            // by destroying objects in recycle bin only if the aforementioned number is  less than <numVisibleItems + numItemsInRecycleBin>,
            // and of course, making sure at least 1 item is in the bin all the time
            if (_VisibleItemsCount > _InternalParams.maxVisibleItemsSeenSinceLastScrollViewSizeChange)
                _InternalParams.maxVisibleItemsSeenSinceLastScrollViewSizeChange = _VisibleItemsCount;

            // Disable all recyclable views
            // Destroy remaining unused views, BUT keep one, so there's always a reserve, instead of creating/destroying very frequently
            // + keep <numVisibleItems + numItemsInRecycleBin> abvove <_InternalParams.maxVisibleItemsSeenSinceLastScrollViewSizeChange>
            // See GetNumExcessObjects()
            GameObject go;
            for (int i = 0; i < _InternalParams.recyclableItemsCount; )
            {
                go = _InternalParams.recyclableItems[i].root.gameObject;
                go.SetActive(false);
                if (GetNumExcessObjects() > 0)
                {
                    GameObject.Destroy(go);
                    _InternalParams.recyclableItems.RemoveAt(i);
                    --_InternalParams.recyclableItemsCount;
                }
                else
                    ++i;
            }
        }

        int GetNumExcessObjects()
        {
            //return Mathf.Max(_InternalParams.recyclableItemsCount - 1, 0);
            if (_InternalParams.recyclableItemsCount > 1)
            {
                int excess = (_InternalParams.recyclableItemsCount + _VisibleItemsCount) - GetMinNumObjectsToKeepInMemory();
                if (excess > 0)
                    return excess;
            }

            return 0;
        }

        int GetMinNumObjectsToKeepInMemory()
        {
            return _Params.minNumberOfObjectsToKeepInMemory > 0 ? _Params.minNumberOfObjectsToKeepInMemory : _InternalParams.maxVisibleItemsSeenSinceLastScrollViewSizeChange+1;
        }

        void SetContentEdgeOffsetFromViewportEdge(RectTransform.Edge contentAndViewportEdge, float offset)
        {
            Canvas.ForceUpdateCanvases();
            _Params.scrollRect.StopMovement();
            _Params.content.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(contentAndViewportEdge, offset, _InternalParams.contentPanelSize);
            _InternalParams.CacheScrollViewInfo(); // the content size might slignly change due to eventual rounding errors, so we cache the info again
            // TODO see if a ChangeItemsCountTo is necessary for a refresh
        }

        void OnScreenSizeChangedEventDispatcher.IOnScreenSizeChangedListener.OnScreenSizeChanged(float lastWidth, float lastHeight, float width, float height)
        {
            NotifyScrollViewSizeChanged();
        }



        // A LayoutGroup will be added to source params's content game object if not existent, depending on the type of the scroll rect (horizontal/vertical)
        // the LayoutGroup component will be disabled
        class InternalParams
        {
            /// Fields are in format: value if vertical scrolling/value if horizontal scrolling
            // Constant params 
            //internal HorizontalOrVerticalLayoutGroup layoutGroup;
            internal readonly Vector2 constantAnchorPosForAllItems = new Vector2(0f, 1f); // top-left
            internal float viewportSize;
            internal float paddingContentStart; // top/left
            internal float transversalPaddingContentStart; // left/top
            internal float paddingContentEnd; // bottom/right
            internal float paddingStartPlusEnd;
            internal float spacing;
            internal RectTransform.Edge startEdge; // RectTransform.Edge.Top/RectTransform.Edge.Left
            internal RectTransform.Edge endEdge; // RectTransform.Edge.Bottom/RectTransform.Edge.Right
            internal RectTransform.Edge transvStartEdge; // RectTransform.Edge.Left/RectTransform.Edge.Top
            internal float itemsConstantTransversalSize; // widths/heights

            // Cache params
            internal int itemsCount;
            internal float lastProcessedAbstractNormalizedScrollPosition; // normY / 1-normX
            internal int realIndexOfFirstItemInView;

            internal List<TItemViewsHolder> recyclableItems = new List<TItemViewsHolder>();
            internal int recyclableItemsCount = 0;
            internal float[] itemsSizes; // heights/widths
            internal float[] itemsSizesCumulative; // heights/widths
            internal float cumulatedSizesOfAllItemsPlusSpacing;
            internal float contentPanelSize; // height/width
            internal bool onScrollPositionChangedFiredAndVisibilityComputedForCurrentItems;
            internal bool updateRequestPending;
            internal int maxVisibleItemsSeenSinceLastScrollViewSizeChange = 0; // heuristic used to prevent destroying too much objects; reset back to 0 when the NotifyScrollViewSizeChanged is called

            TParams _SourceParams;
            Func<int, float> _GetItemSizeFunc;

            InternalParams(TParams sourceParams, ScrollRectItemsAdapter8<TParams, TItemViewsHolder> adapter)
            {
                _SourceParams = sourceParams;

                var lg = sourceParams.content.GetComponent<HorizontalOrVerticalLayoutGroup>();
                if (lg && lg.enabled)
                {
                    lg.enabled = false;
                    Debug.Log("LayoutGroup on GameObject " + lg.name + " has beed disabled in order to use ScrollRectItemsAdapter8");
                }

                var contentSizeFitter = sourceParams.content.GetComponent<ContentSizeFitter>();
                if (contentSizeFitter && contentSizeFitter.enabled)
                {
                    contentSizeFitter.enabled = false;
                    Debug.Log("ContentSizeFitter on GameObject " + contentSizeFitter.name + " has beed disabled in order to use ScrollRectItemsAdapter8");
                }

                var layoutElement = sourceParams.content.GetComponent<LayoutElement>();
                if (layoutElement)
                {
                    GameObject.Destroy(layoutElement);
                    Debug.Log("LayoutElement on GameObject " + contentSizeFitter.name + " has beed DESTROYED in order to use ScrollRectItemsAdapter8");
                }

                if (sourceParams.scrollRect.horizontal)
                {
                    startEdge = RectTransform.Edge.Left;
                    endEdge = RectTransform.Edge.Right;
                    transvStartEdge = RectTransform.Edge.Top;

                    // Need to create a lambda, not store the method directly (which will call the base's abstract method)
                    _GetItemSizeFunc = i => adapter.GetItemWidth(i);
                }
                else
                {
                    startEdge = RectTransform.Edge.Top;
                    endEdge = RectTransform.Edge.Bottom;
                    transvStartEdge = RectTransform.Edge.Left;

                    // Need to create a lambda, not store the method directly (which will call the base's abstract method)
                    _GetItemSizeFunc = i => adapter.GetItemHeight(i);
                }
                CacheScrollViewInfo();
            }

            internal static InternalParams CreateFromSourceParamsOrThrow(TParams sourceParams, ScrollRectItemsAdapter8<TParams, TItemViewsHolder> adapter)
            {
                if (sourceParams.scrollRect.horizontal && sourceParams.scrollRect.vertical)
                {
                    throw new UnityException("Can't optimize a ScrollRect with both horizontal and vertical scrolling modes. Disable one of them");
                }

                return new InternalParams(sourceParams, adapter);
            }


            internal void CacheScrollViewInfo()
            {
                RectTransform vpRT = _SourceParams.viewport;
                Rect vpRect = vpRT.rect;
                
                if (_SourceParams.scrollRect.horizontal)
                {
                    viewportSize = vpRect.width;
                    paddingContentStart = _SourceParams.contentPadding.left;
                    paddingContentEnd = _SourceParams.contentPadding.right;
                    transversalPaddingContentStart = _SourceParams.contentPadding.top;
                    itemsConstantTransversalSize = _SourceParams.content.rect.height - (transversalPaddingContentStart + _SourceParams.contentPadding.bottom);
                }
                else
                {
                    viewportSize = vpRect.height;
                    paddingContentStart = _SourceParams.contentPadding.top;
                    paddingContentEnd = _SourceParams.contentPadding.bottom;
                    transversalPaddingContentStart = _SourceParams.contentPadding.left;
                    itemsConstantTransversalSize = _SourceParams.content.rect.width - (transversalPaddingContentStart + _SourceParams.contentPadding.right);
                }
                paddingStartPlusEnd = paddingContentStart + paddingContentEnd;
                spacing = _SourceParams.contentSpacing;
            }


            void AssureItemsSizesArrayCapacity()
            {
                if (itemsSizes == null || itemsSizes.Length != itemsCount)
                    itemsSizes = new float[itemsCount];

                if (itemsSizesCumulative == null || itemsSizesCumulative.Length != itemsCount)
                    itemsSizesCumulative = new float[itemsCount];
            }

            float CollectSizesOfAllItems()
            {
                AssureItemsSizesArrayCapacity();
                float size, cumulatedSizesOfAllItems = 0f;
                int realIndex;
                for (int viewIndex = 0; viewIndex < itemsCount; ++viewIndex)
                {
                    realIndex = GetItemRealIndexFromViewIndex(viewIndex);
                    size = _GetItemSizeFunc(realIndex);
                    itemsSizes[viewIndex] = size;
                    cumulatedSizesOfAllItems += size;
                    itemsSizesCumulative[viewIndex] = cumulatedSizesOfAllItems;
                }

                return cumulatedSizesOfAllItems;
            }

            internal void OnItemsCountChanged(int itemsNewCount, bool contentPanelEndEdgeStationary)
            {
                realIndexOfFirstItemInView = itemsNewCount > 0 ? 0 : -1;

                itemsCount = itemsNewCount;
                OnTotalSizeOfAllItemsChanged(CollectSizesOfAllItems(), contentPanelEndEdgeStationary);

                // Schedule a new ComputeVisibility iteration
                onScrollPositionChangedFiredAndVisibilityComputedForCurrentItems = false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="viewHolder"></param>
            /// <returns>the resolved size, as this may be a bit different than the passed <newSize> for huge data sets (>100k items)</returns>
            internal float ChangeItemSizeAndUpdateContentSizeAccordingly(TItemViewsHolder viewHolder, float requestedSize, bool itemEndEdgeStationary)
            {
                //LiveDebugger8.logR("ChangeItemCountInternal");
                if (itemsSizes == null)
                    throw new UnityException("Wait for initialization first");

                if (viewHolder.root == null)
                    throw new UnityException("God bless"); // shouldn't happen if implemented according to documentation/examples
                
                viewHolder.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(startEdge, GetItemOffsetFromParentStartUsingItemIndexInView(viewHolder.itemIndexInView),  requestedSize);
                float resolvedSize;
                // Even though we know the desired size, the one actually set by the UI system may be different, so we cache that one
                if (_SourceParams.scrollRect.horizontal)
                    resolvedSize = viewHolder.root.rect.width;
                else
                    resolvedSize = viewHolder.root.rect.height;
                viewHolder.cachedSize = resolvedSize;

                float size, cumulatedSizesOfAllItems = 0f;
                int realIndex;
                for (int viewIndex = 0; viewIndex < itemsCount; ++viewIndex)
                {
                    realIndex = GetItemRealIndexFromViewIndex(viewIndex);
                    if (viewIndex == viewHolder.itemIndexInView) // don't request the size of the modified view; use the resolved one instead
                        size = resolvedSize;
                    else
                        size = _GetItemSizeFunc(realIndex);

                    itemsSizes[viewIndex] = size;
                    cumulatedSizesOfAllItems += size;
                    itemsSizesCumulative[viewIndex] = cumulatedSizesOfAllItems;
                }
                OnTotalSizeOfAllItemsChanged(cumulatedSizesOfAllItems, itemEndEdgeStationary);

                return resolvedSize;
            }

            internal void UpdateLastProcessedNormalizedScrollPosition()
            {
                lastProcessedAbstractNormalizedScrollPosition = _SourceParams.GetAbstractNormalizedScrollPosition();
            }

            internal void ResetLastProcessedNormalizedScrollPositionToStart()
            {
                // Commented: not needed anymore, as now start=1, end=0, regardless of the scroll type
                //if (_SourceParams.scrollRect.horizontal)
                //    lastProcessedAbstractNormalizedScrollPosition = 0f;
                //else
                //lastProcessedAbstractNormalizedScrollPosition = 1f;

                lastProcessedAbstractNormalizedScrollPosition = 1f;
            }
            internal void ResetLastProcessedNormalizedScrollPositionToEnd()
            {
                // Commented: not needed anymore, as now start=1, end=0, regardless of the scroll type
                //if (_SourceParams.scrollRect.horizontal)
                //    lastProcessedAbstractNormalizedScrollPosition = 0f;
                //else
                //lastProcessedAbstractNormalizedScrollPosition = 1f;

                lastProcessedAbstractNormalizedScrollPosition = 0f;
            }

            internal float GetItemOffsetFromParentStartUsingItemIndexInView(int itemIndexInView)
            {
                // Commented: using a more efficient way of doing this by using cumulative sizes
                //float distanceFromParentStart = paddingContentStart;
                ////float cumulatedSizesOfAllItems = 0f;
                //for (int i = 0; i < itemIndex; ++i)
                //    distanceFromParentStart += itemsSizes[i];
                //distanceFromParentStart += itemIndex * spacing;

                //return distanceFromParentStart;

                float cumulativeSizeOfAllItemsBeforePlusSpacing = 0f;
                if (itemIndexInView > 0)
                    cumulativeSizeOfAllItemsBeforePlusSpacing = itemsSizesCumulative[itemIndexInView - 1] + itemIndexInView * spacing;

                return paddingContentStart + cumulativeSizeOfAllItemsBeforePlusSpacing;
            }
            internal float GetItemOffsetFromParentEndUsingItemIndexInView(int itemIndexInView)
            {
                return contentPanelSize - (GetItemOffsetFromParentStartUsingItemIndexInView(itemIndexInView) + itemsSizes[itemIndexInView]);

                //float cumulativeSizeOfAllItemsInclusiveThisOnePlusSpacing = itemsSizesCumulative[itemIndex] + itemIndex * spacing;

                //return paddingContentEnd + (cumulatedSizesOfAllItemsPlusSpacing - cumulativeSizeOfAllItemsInclusiveThisOnePlusSpacing);
            }

            internal int GetItemRealIndexFromViewIndex(int indexInView) { return (realIndexOfFirstItemInView + indexInView) % itemsCount; }
            internal int GetItemViewIndexFromRealIndex(int realIndex) { return (realIndex - realIndexOfFirstItemInView + itemsCount) % itemsCount; }

            internal void OnScrollViewLooped(int newValueOf_RealIndexOfFirstItemInView)
            {
                int oldValueOf_realIndexOfFirstItemInView = realIndexOfFirstItemInView;
                realIndexOfFirstItemInView = newValueOf_RealIndexOfFirstItemInView;

                int arrayRotateAmount = oldValueOf_realIndexOfFirstItemInView - realIndexOfFirstItemInView;
                if (arrayRotateAmount != 0)
                {
                    itemsSizes = itemsSizes.GetRotatedArray(arrayRotateAmount);

                    float cumulatedSizesOfAllItems = 0f;
                    for (int i = 0; i < itemsCount; ++i)
                    {
                        cumulatedSizesOfAllItems += itemsSizes[i];
                        itemsSizesCumulative[i] = cumulatedSizesOfAllItems;
                    }
                }
            }

            void OnTotalSizeOfAllItemsChanged(float cumulatedSizeOfAllItems, bool contentPanelEndEdgeStationary)
            {
                cumulatedSizesOfAllItemsPlusSpacing = cumulatedSizeOfAllItems + Mathf.Max(0, itemsCount - 1) * spacing;
                OnCumulatedSizesOfAllItemsPlusSpacingChanged(cumulatedSizesOfAllItemsPlusSpacing, contentPanelEndEdgeStationary);
            }

            void OnCumulatedSizesOfAllItemsPlusSpacingChanged(float newValue, bool contentPanelEndEdgeStationary)
            {
                contentPanelSize = cumulatedSizesOfAllItemsPlusSpacing + paddingStartPlusEnd;

                var edgeToUse = contentPanelEndEdgeStationary ? endEdge : startEdge;
                float insetToUse = _SourceParams.content.GetInsetFromParentEdge(_SourceParams.viewport, edgeToUse);

                // This way, the content doesn't move, it only grows down
                _SourceParams.content.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(_SourceParams.viewport, edgeToUse, insetToUse, contentPanelSize);
                _SourceParams.scrollRect.Rebuild(CanvasUpdate.PostLayout);
                Canvas.ForceUpdateCanvases();
            }
        }
    }
}
