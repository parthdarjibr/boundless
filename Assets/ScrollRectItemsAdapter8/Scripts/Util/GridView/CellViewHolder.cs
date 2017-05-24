using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Util.GridView
{
    public abstract class CellViewHolder : AbstractViewHolder
    {
        public RectTransform views;


        public virtual void Init(RectTransform root)
        {
            this.root = root;
            itemIndex = -1; // initially, undefined
            CollectViews();
        }

        public override void CollectViews()
        {
            base.CollectViews();

            views = GetViews();
            if (views == root)
                throw new UnityException("CellViewHolder: views == root not allowed: you should have a child of root that holds all the views, as the root should always be enabled for layouting purposes");
        }

        protected abstract RectTransform GetViews();
    }
}
