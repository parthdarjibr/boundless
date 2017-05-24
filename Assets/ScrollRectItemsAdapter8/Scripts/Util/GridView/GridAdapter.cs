using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Util.GridView
{
    public abstract class GridAdapter<TParams, TCellVH> : ScrollRectItemsAdapter8<TParams, CellGroupViewHolder<TCellVH>> 
        where TParams : GridParams
        where TCellVH : CellViewHolder, new()
    {
        protected int _CellsCount;


        // Optionally handle the change item count command before/after calling the base implementation
        public override void ChangeItemCountTo(int cellsCount)
        {
            _CellsCount = cellsCount;

            // The number of groups is passed to the base's implementation
            int groupsCount = _Params.GetNumberOfRequiredGroups(_CellsCount);

            base.ChangeItemCountTo(groupsCount);
        }

        // Will be called for vertical scroll views
        protected override float GetItemHeight(int index)
        { return _Params.GetGroupHeight(); }

        // Will be called for horizontal scroll views
        protected override float GetItemWidth(int index)
        { return _Params.GetGroupWidth(); }

        // Here's the meat of the whole recycling process
        protected override void InitOrUpdateItemViewHolder(CellGroupViewHolder<TCellVH> newOrRecycled)
        {
            if (newOrRecycled.root == null) // container empty => instantiate the prefab in it
            {
                // Instantiate the layout group representing the a group of cells (row or column)
                newOrRecycled.root = (GameObject.Instantiate(_Params.GetGroupPrefab(newOrRecycled.itemIndex).gameObject) as GameObject).transform as RectTransform;
                newOrRecycled.root.gameObject.SetActive(true); // just in case the prefab was disabled

                // Instantiate all the cells in the group
                for (int i = 0; i < _Params.numCellsPerGroup; ++i)
                {
                    var cellInstance = (GameObject.Instantiate(_Params.cellPrefab.gameObject) as GameObject).transform as RectTransform;
                    cellInstance.gameObject.SetActive(true); // just in case the prefab was disabled
                    cellInstance.SetParent(newOrRecycled.root);
                }
                newOrRecycled.CollectViews();
            }

            // At this point there is for sure enough groups, but there may not be enough enabled cells, or there are too much enabled cells

            int activeCellsForThisGroup;
            // If it's the last one
            if (newOrRecycled.itemIndex + 1 == GetItemCount())
            {
                int totalCellsBeforeThisGroup = 0;
                if (newOrRecycled.itemIndex > 0)
                {
                    totalCellsBeforeThisGroup = newOrRecycled.itemIndex * _Params.numCellsPerGroup;
                }
                activeCellsForThisGroup = _CellsCount - totalCellsBeforeThisGroup;
            }
            else
            {
                activeCellsForThisGroup = _Params.numCellsPerGroup;
            }
            newOrRecycled.NumActiveCells = activeCellsForThisGroup;

            for (int i = 0; i < activeCellsForThisGroup; ++i)
                UpdateCellViewHolder(newOrRecycled.ContainingCellViewHolders[i]);

            // Update v2.4: commented, because renaming game objects in a scroll view each frame messes up with some versions of unity and slows performance; 
            //// optionally rename the object
            //newOrRecycled.root.name = "ListItem " + newOrRecycled.itemIndex;
        }

        protected abstract void UpdateCellViewHolder(TCellVH viewHolder);
    }
}
