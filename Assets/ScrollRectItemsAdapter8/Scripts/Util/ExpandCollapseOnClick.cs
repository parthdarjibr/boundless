using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using frame8.Logic.Misc.Other.Extensions;

public class ExpandCollapseOnClick : MonoBehaviour {

    [Tooltip("will be taken from this object, if not specified")]
    public Button button;
    [HideInInspector]
    public float nonExpandedSize = -1f;
    [HideInInspector]
    public bool expanded;

    public float expandFactor = 2f;
    public float animDuration = .2f;
    //float nonExpandedSize;
    float startSize;
    float endSize;
    float animStart;
    float animEnd;
    bool animating = false;
    RectTransform rectTransform;

    public ISizeChangesHandler sizeChangesHandler;

    // Use this for initialization
    void Awake ()
    {
        rectTransform = transform as RectTransform;

        //nonExpandedSize = (transform as RectTransform).rect.height;

        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnClicked);

    }

    public void OnClicked()
    {
        if (animating)
            return;

        if (nonExpandedSize < 0f)
            return;

        animating = true;
        animStart = Time.time;
        animEnd = animStart + animDuration;
        //startSize = (transform as RectTransform).rect.height;

        if (expanded) // shrinking
        {
            startSize = nonExpandedSize * expandFactor;
            endSize = nonExpandedSize;
        }
        else // expanding
        {
            startSize = nonExpandedSize;
            endSize = nonExpandedSize * expandFactor;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (animating)
        {
            float elapsedTime = Time.time - animStart;
            float t01 = elapsedTime / animDuration;
            if (t01 >= 1f) // done
            {
                t01 = 1f; // fill/clamp animation
                animating = false;
            }

            float size = Mathf.Lerp(startSize, endSize, t01);
            //adapter.RequestChangeItemSizeAndUpdateLayout(transform as RectTransform, size);

            //(transform as RectTransform).SetSizeFromParentEdgeWithCurrentAnchors(RectTransform.Edge.Top, Mathf.Lerp(startSize, endSize, t01));
            if (sizeChangesHandler != null)
            {
                bool accepted = sizeChangesHandler.HandleSizeChangeRequest(rectTransform, size);

                // Interruption
                if (!accepted)
                    animating = false;

                if (!animating) // done; even if it wasn't accepted, wether we should or shouldn't change the "expanded" state depends on the user's requirements. We chose to change it
                {
                    expanded = !expanded;
                    sizeChangesHandler.OnExpandedStateChanged(rectTransform, expanded);
                }
            }
            //else // prefab


        }
	}

    public interface ISizeChangesHandler
    {
        bool HandleSizeChangeRequest(RectTransform rt, float newSize);
        void OnExpandedStateChanged(RectTransform rt, bool expanded);
    }
}
