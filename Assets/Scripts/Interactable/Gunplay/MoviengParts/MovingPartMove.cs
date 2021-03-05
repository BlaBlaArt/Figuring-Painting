using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MovingPartMove : MonoBehaviour, IMoveingPart
{
    [SerializeField] Vector3 moveBy;
    [SerializeField] float timeIn;
    [SerializeField] Ease easeIn;
    [SerializeField] float pauseIn;

    [Space]
    [SerializeField] bool moveBack;
    [SerializeField] [ShowIf("moveBack")] float timeOut;
    [SerializeField] [ShowIf("moveBack")] Ease easeOut;
    [SerializeField] [ShowIf("ShowPauseOut")] float pauseOut;

    [Space]
    [SerializeField] bool loop;
    [SerializeField] [ShowIf("loop")] bool endlessLoop;
    [SerializeField] [ShowIf("ShowLoops")] int loops;

    Vector3 _startPos;

    Sequence _seq;

    bool ShowLoops => loop && !endlessLoop;
    bool ShowPauseOut => loop && moveBack;

    void Start()
    {
        _startPos = transform.localPosition;
    }

    void SetupSequence()
    {
        _seq = DOTween.Sequence();

        _seq.Append(transform.DOBlendableLocalMoveBy(moveBy, timeIn).SetEase(easeIn));
        if (pauseIn != 0f)
            _seq.AppendInterval(pauseIn);

        if (moveBack)
        {
            _seq.Append(transform.DOLocalMove(_startPos, timeOut).SetEase(easeOut));
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
        transform.position = _startPos;
    }

    public void Revert(float time)
    {
        transform.DOKill();
        transform.DOMove(_startPos, time);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveBy);
    }
}
