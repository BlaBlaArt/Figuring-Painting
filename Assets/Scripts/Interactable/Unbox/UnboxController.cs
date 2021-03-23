using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public enum StepType
{
    NumOfSteps,
    LengthOfStep
}

public class UnboxController : MonoBehaviour
{
    [SerializeField] UnboxSettings settings;

    [Space]
    [SerializeField] Animator anim;
    [SerializeField] BoxCollider col;

    [Space]
    [SerializeField] StepType stepType;
    [SerializeField] [ShowIf("IsNumOfSteps")] int steps = 4;
    [SerializeField] [HideIf("IsNumOfSteps")] float stepLength = .05f;

    [Space]
    public Transform partPositionsParent;
    [ReadOnly] public List<Transform> partPositions;

    [Space]
    public float openAnimDelay = .5f;

    public UnityEvent onBoxOpen;

    float _startMousePos;

    float _distanceOfStep;

    float _stepTime = .1f;

    bool IsNumOfSteps => stepType == StepType.NumOfSteps;

    void Start()
    {

        if (IsNumOfSteps)
            stepLength = 1f / steps;

        _distanceOfStep = settings.distanceToOpen * stepLength;
    }

    void OnMouseDown()
    {
        _startMousePos = LerpedMousePos();
    }

    void OnMouseDrag()
    {
        if (_distanceOfStep <= LerpedMousePos() - _startMousePos)
            StepComplete();
    }

    void OnMouseUp()
    {
        if (anim.GetFloat("Progress") > .9f)
        {
            anim.Play("End");
            col.enabled = false;
            Taptic.Medium();
            onBoxOpen.Invoke();
        }

     //   TutorialController.inst.HideTutorialUnbox();
    }

    void StepComplete()
    {
        _startMousePos = LerpedMousePos();
        DOTween.To(() => anim.GetFloat("Progress"), x => anim.SetFloat("Progress", x),
            anim.GetFloat("Progress") + stepLength, _stepTime);
        Taptic.Light();
    }

    float LerpedMousePos()
    {
        return Mathf.InverseLerp(0, Screen.width, Input.mousePosition.x);
    }

    internal void DOMove()
    {
        throw new NotImplementedException();
    }
}
