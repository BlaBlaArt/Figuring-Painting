using UnityEditor;
using UnityEngine;

public static class EditorTools
{
    [MenuItem("Tools/Editor Tools/Set Selected Position &z")]
    public static void SetSelectedPosition()
    {
        if(Selection.activeObject != null)
        {
            (Selection.activeObject as GameObject).transform.localPosition = Vector3.zero;
            (Selection.activeObject as GameObject).transform.localRotation = Quaternion.identity;
        }
    }

    [MenuItem("Tools/Editor Tools/TogleGameObjectActive &x")]
    public static void TogleGameObjectActive()
    {
        GameObject go = Selection.activeObject as GameObject;
        if (go != null)
        {
            go.SetActive(!go.activeSelf);
        }
    }
}