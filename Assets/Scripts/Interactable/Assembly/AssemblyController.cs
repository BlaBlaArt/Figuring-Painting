using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Part
{
    public PartController part;
    [SerializeReference] public HighlightController partDummy;
    public int id;
    [HideInInspector] public bool insterted;
}

public class AssemblyController : MonoBehaviour
{
    [SerializeField] AssemblySettings settings;

    [SerializeField] List<Part> parts;

    public UnityEvent onAssemblyFinished;
    public UnityEvent onPartInserted;
    public UnityEvent<PartController> onPartNew;

    [Header("Debug")]
    [SerializeField] bool dontDisassemble;

    int _currentStep;

    Part _currentPart;

    void Start()
    {
        foreach (var item in parts)
        {
            item.part.partID = item.id;
            item.partDummy.HideHighlight();
        }

        PartController.onGrabStart.AddListener(OnGrabStartPartOfAssembly);
        PartController.onGrabStop.AddListener(OnGrabStopPartOfAssembly);

        if (dontDisassemble)
        {
            foreach (var item in parts)
                SetPlacement(item);
            onAssemblyFinished.Invoke();
        }
    }

    void OnDestroy()
    {
        PartController.onGrabStart.RemoveListener(OnGrabStartPartOfAssembly);
        PartController.onGrabStop.RemoveListener(OnGrabStopPartOfAssembly);
    }

    public void PartPlacement()
    {
        foreach (var item in parts)
            item.part.PlacementReady();
    }

    public void ReadyToAssemble()
    {
        foreach (var item in parts)
            item.part.TogglePhysics(true);

      //  this.Invoke(0f, () => onPartNew.Invoke(parts[0].part));
      //  this.Invoke(1f, parts[_currentStep].partDummy.ShowHighlight);
    }

    void OnGrabStartPartOfAssembly(PartController partC)
    {
        _currentPart = parts.First(x => x.id == partC.partID);

        // if (partC == parts[_currentStep].part)
        //     _currentPart.partDummy.ShowHighlight();
    }

    void OnGrabStopPartOfAssembly(PartController partC)
    {
        // _currentPart.partDummy.HideHighlight();

        CheckPlacement(partC);
        if (IfFullyAssembled())
        {
            parts[0].partDummy.transform.parent.SetInactive();
        //    this.Invoke(.5f, Taptic.Medium);
            onAssemblyFinished.Invoke();
        }
    }

    void CheckPlacement(PartController partC)
    {
        float dist = Vector3.Distance(partC.transform.position, _currentPart.partDummy.transform.position);
        if (dist < settings.distFromTargetPos && partC == parts[_currentStep].part)
        {
            _currentPart.partDummy.HideHighlight();
            SetPlacement(partC, dist);
            if (settings.stepAssembly)
                if (_currentStep < parts.Count - 1)
                    parts[++_currentStep].partDummy.ShowHighlight();

            onPartNew.Invoke(parts[_currentStep].part);
        }
        else
        {
            partC.PlacementReport(false);
        }
    }

    void SetPlacement(PartController partC, float distance)
    {
        _currentPart.insterted = true;

        partC.transform.DOMove(_currentPart.partDummy.transform.position, distance * settings.snapPartSpeed);
        partC.transform.DORotateQuaternion(_currentPart.partDummy.transform.rotation, distance * settings.snapPartSpeed);
        partC.PlacementReport(true);

        Taptic.Light();

        onPartInserted.Invoke();
    }

    void SetPlacement(Part part)
    {
        part.insterted = true;

        part.part.transform.position = part.partDummy.transform.position;
        part.part.transform.rotation = part.partDummy.transform.rotation;

        part.part.PlacementReport(true);
    }

    bool IfFullyAssembled()
    {
        foreach (var item in parts)
            if (!item.insterted)
                return false;
        return true;
    }

    [Button]
    public void GenerateList()
    {
        Transform partParent = GetComponentInChildren<PartController>().transform.parent;
        Transform dummyParent = GetComponentInChildren<HighlightController>().transform.parent;

        List<PartController> pcs = partParent.GetComponentsInChildren<PartController>().ToList();
        List<HighlightController> hcs = dummyParent.GetComponentsInChildren<HighlightController>().ToList();

        parts = new List<Part>();
        for (int i = 0; i < pcs.Count; i++)
        {
            parts.Add(new Part { part = pcs[i], partDummy = hcs[i], id = i });
            pcs[i].Setup();
            hcs[i].Setup();
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }

    [Button]
    public void PhysicsOn()
    {
        foreach (var item in parts)
            item.part.TogglePhysics(true);
    }

    [Button]
    public void PhysicsOff()
    {
        foreach (var item in parts)
            item.part.TogglePhysics(false);
    }
}
