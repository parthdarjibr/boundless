using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Util.GridView
{
    [Serializable] // serializable, so it can be shown in inspector
    public class GridParams : BaseParams, ISerializationCallbackReceiver
    {
        [Header("Grid")]
        public RectTransform cellPrefab;
        [Tooltip("The max. number of cells in a row (for vertical ScrollView) or column (for horizontal ScrollView)")]
        public int numCellsPerGroup;
        public TextAnchor alignmentOfCellsInGroup = TextAnchor.UpperLeft;
        public RectOffset groupPadding;
        public bool cellWidthForceExpandInGroup;
        public bool cellHeightForceExpandInGroup;

        // Cached prefab, auto-generated at runtime, first time GetItemAsLayoutGroupPrefab() is called
        HorizontalOrVerticalLayoutGroup _TheOnlyGroupPrefab;
        int _NumCellsPerGroupHorizontally = 1, _NumCellsPerGroupVertically = 1;


        public virtual HorizontalOrVerticalLayoutGroup GetGroupPrefab(int forItemAtThisIndex)
        {
            if (_TheOnlyGroupPrefab == null)
            {
                var go = new GameObject(scrollRect.name + "_GridAdapter_GroupPrefab");
                go.SetActive(false);
                go.transform.SetParent(scrollRect.transform, false);
                if (scrollRect.horizontal)
                {
                    _TheOnlyGroupPrefab = go.AddComponent<VerticalLayoutGroup>(); // groups are columns in a horizontal scrollview
                }
                else
                {
                    _TheOnlyGroupPrefab = go.AddComponent<HorizontalLayoutGroup>(); // groups are rows in a vertical scrollview
                }

                _TheOnlyGroupPrefab.spacing = contentSpacing;
                _TheOnlyGroupPrefab.childForceExpandWidth = cellWidthForceExpandInGroup;
                _TheOnlyGroupPrefab.childForceExpandHeight = cellHeightForceExpandInGroup;
                _TheOnlyGroupPrefab.childAlignment = alignmentOfCellsInGroup;
                _TheOnlyGroupPrefab.padding = groupPadding;


            }

            return _TheOnlyGroupPrefab;
        }

        public virtual float GetGroupWidth()
        { return groupPadding.left + /*cellPrefab.rect.width*/ cellPrefab.sizeDelta.x * _NumCellsPerGroupHorizontally + contentSpacing + groupPadding.right; }

        public virtual float GetGroupHeight()
        { 
            return groupPadding.top + /*cellPrefab.rect.height*/cellPrefab.sizeDelta.y * _NumCellsPerGroupVertically + contentSpacing + groupPadding.bottom;
        }

        //public virtual int GetCellIndex(int groupIndex) { }
        public virtual int GetGroupIndex(int cellIndex) { return cellIndex / numCellsPerGroup; }

        public virtual int GetNumberOfRequiredGroups(int numberOfCells) { return numberOfCells == 0 ? 0 : GetGroupIndex(numberOfCells - 1) + 1; }

        protected virtual void InitNonSerializedData()
        {
			if (scrollRect != null) {
				if (scrollRect.horizontal)
					_NumCellsPerGroupVertically = numCellsPerGroup;
				else
					_NumCellsPerGroupHorizontally = numCellsPerGroup;
			}
        }

        #region ISerializationCallbackReceiver
        public void OnBeforeSerialize() {}
        public void OnAfterDeserialize() { InitNonSerializedData(); }
        #endregion
    }
}
