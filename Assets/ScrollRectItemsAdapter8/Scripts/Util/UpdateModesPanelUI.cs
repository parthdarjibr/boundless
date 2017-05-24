using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;

public class UpdateModesPanelUI : MonoBehaviour {

    public ScrollRectItemsAdapterExample hor, vert;
    public Toggle a, b, c;

	void Start ()
    {
        a.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                hor.Params.updateMode = vert.Params.updateMode = BaseParams.UpdateMode.ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE;
        });
        b.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                hor.Params.updateMode = vert.Params.updateMode = BaseParams.UpdateMode.ON_SCROLL;
        });
        c.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                hor.Params.updateMode = vert.Params.updateMode = BaseParams.UpdateMode.MONOBEHAVIOUR_UPDATE;
        });
    }
}
