﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollToPanelUI : MonoBehaviour {

    public ScrollRectItemsAdapterExample vert, hor;
    public int location, locationScrollAndExpand;
    public Button scroll, scrollExpand;

    private void Start()
    {
        scroll.onClick.AddListener(() =>
        {
            if (vert.Data != null && vert.Data.Count > location)
                vert.Adapter.SmoothScrollTo(location, 1f, null);
            if (hor.Data != null && hor.Data.Count > location)
                hor.Adapter.SmoothScrollTo(location, 1f, null);
        });

        scrollExpand.onClick.AddListener(() =>
        {
            if (vert.Data != null && vert.Data.Count > locationScrollAndExpand)
                vert.Adapter.SmoothScrollTo(locationScrollAndExpand, 1f,
                    progress =>
                    {
                        if (progress == 1f)
                        {
                            var vh = vert.Adapter.GetItemViewsHolderIfVisible(locationScrollAndExpand);
                            if (vh != null)
                            {
                                if (vh.expandOnCollapseComponent != null)
                                    vh.expandOnCollapseComponent.OnClicked();
                            }
                        }

                        return true;
                    }
                );
        });
    }

}
