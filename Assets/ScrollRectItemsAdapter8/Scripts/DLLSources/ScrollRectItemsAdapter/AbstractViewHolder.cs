
namespace frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter
{
    public abstract class AbstractViewHolder
    {
        public virtual int itemIndex { get; set; }

        public UnityEngine.RectTransform root;


        public virtual void CollectViews()
        { }
    }
}
