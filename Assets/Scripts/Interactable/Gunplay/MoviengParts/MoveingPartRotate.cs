using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MoveingPartRotate : MonoBehaviour, IMoveingPart
{
    [SerializeField] Vector3 rotateBy;
    [SerializeField] float timeIn;
    [SerializeField] Ease easeIn;
    [SerializeField] float pauseIn;

    [Space]
    [SerializeField] bool rotateBack;
    [SerializeField] [ShowIf("rotateBack")] float timeOut;
    [SerializeField] [ShowIf("rotateBack")] Ease easeOut;
    [SerializeField] [ShowIf("ShowPauseOut")] float pauseOut;

    [Space]
    [SerializeField] bool loop;
    [SerializeField] [ShowIf("loop")] bool endlessLoop;
    [SerializeField] [ShowIf("ShowLoops")] int loops;

    Quaternion _startRot;

    Sequence _seq;

    bool ShowLoops => loop && !endlessLoop;
    bool ShowPauseOut => loop && rotateBack;

    void Start()
    {
        _startRot = transform.localRotation;
    }

    void SetupSequence()
    {
        _seq = DOTween.Sequence();

        _seq.Append(transform.DOBlendableLocalRotateBy(rotateBy, timeIn).SetEase(easeIn));
        if (pauseIn != 0f)
            _seq.AppendInterval(pauseIn);

        if (rotateBack)
        {
            _seq.Append(transform.DOLocalRotateQuaternion(_startRot, timeOut).SetEase(easeOut));
            if (pauseOut != 0f && loop)
                _seq.AppendInterval(pauseOut);
        }

        if (loop)
        {
            if (endlessLoop)
                _seq.SetLoops(-1);
            else
                _seq.SetLoops(loops);
        }
    }

    public void Execute()
    {
        _seq.Kill(true);
        SetupSequence();
    }

    public void Revert()
    {
        transform.rotation = _startRot;
    }

    public void Revert(float time)
    {
        transform.DOKill();
        transform.DORotateQuaternion(_startRot, time);
    }
}
