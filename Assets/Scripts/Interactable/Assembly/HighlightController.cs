using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HighlightController : MonoBehaviour
{
    public abstract void ShowHighlight();
    public abstract void HideHighlight();

    public abstract void Setup();
}
