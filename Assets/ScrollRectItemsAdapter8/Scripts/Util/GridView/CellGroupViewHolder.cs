using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Util.GridView
{
    public class CellGroupViewHolder<TCellVH> : BaseItemViewsHolder where TCellVH : CellViewHolder, new()
    {
        // TODO uppercase this
        public override int itemIndex
        {
            get { return base.itemIndex; }
            set
            {
                base.itemIndex = value;
                if (_Capacity > 0)
                    OnGroupIndexChanged();
            }
        }

        public int NumActiveCells
        {
            get { return _NumActiveCells; }
            set
            {
                if (_NumActiveCells != value)
                {
                    _NumActiveCells = value;
                    for (int i = 0; i < _Capacity; ++i)
                        ContainingCellViewHolders[i].views.gameObject.SetActive(i < _NumActiveCells);
                }
            }
        }


        public TCellVH[] ContainingCellViewHolders { get; private set; }

        protected HorizontalOrVerticalLayoutGroup _LayoutGroup;
        protected int _Capacity = -1;
        protected int _NumActiveCells = 0;


        public override void CollectViews()
        {
            base.CollectViews();

            //if (capacity == -1) // not initialized
            //    throw new InvalidOperationException("ItemAsLayoutGroupViewHolder.CollectViews(): call InitGroup(...) before!");

            _LayoutGroup = root.GetComponent<HorizontalOrVerticalLayoutGroup>();

            _Capacity = root.childCount;
            ContainingCellViewHolders = new TCellVH[_Capacity];
            for (int i = 0; i < _Capacity; ++i)
            {
                ContainingCellViewHolders[i] = new TCellVH();
                ContainingCellViewHolders[i].Init(root.GetChild(i) as RectTransform);
                ContainingCellViewHolders[i].views.gameObject.SetActive(false); // not visible, initially
            }

            if (itemIndex != -1 && _Capacity > 0)
                UpdateIndicesOfContainingCells();
        }

        protected virtual void OnGroupIndexChanged()
        {
            if (_Capacity > 0)
                UpdateIndicesOfContainingCells();
        }

        protected virtual void UpdateIndicesOfContainingCells()
        {
            for (int i = 0; i < _Capacity; ++i)
            {
                ContainingCellViewHolders[i].itemIndex = itemIndex * _Capacity + i; 
            }
        }
    }

}
