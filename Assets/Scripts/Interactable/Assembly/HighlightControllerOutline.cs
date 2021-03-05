using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HighlightControllerOutline : HighlightController
{
    [SerializeField] Outline outline;

    public override void ShowHighlight()
    {
        outline.enabled = true;
    }

    public override void HideHighlight()
    {
        outline.enabled = false;
    }


    [Button]
    public override void Setup()
    {
        if(!outline)
            outline = gameObject.AddComponent<Outline>();

        outline.OutlineWidth = 10;

        gameObject.layer = 2;

        GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        #if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
        #endif
    }
}
