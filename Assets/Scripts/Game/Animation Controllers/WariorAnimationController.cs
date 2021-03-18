using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WariorAnimationController : MonoBehaviour, IAnimationController
{
    [SerializeField] private Animator anim;
    private static readonly int IsRun = Animator.StringToHash("IsRun");
    private static readonly int Attack1 = Animator.StringToHash("Attack");
    private static readonly int IsDrag = Animator.StringToHash("IsDrag");
    private static readonly int IsWin = Animator.StringToHash("IsWin");

    public void Run()
    {
        anim.SetBool(IsRun, true);
    }

    public void Attack()
    {
        anim.SetTrigger(Attack1);
    }

    public void Idle()
    {
        anim.SetBool(IsRun, false);
    }

    public void OnDrag(bool isStart)
    {
        anim.SetBool(IsDrag, isStart);
    }

    public void OnWin()
    {
        anim.SetTrigger(IsWin);
    }

    public void OnDead()
    {
        anim.SetTrigger("IsDead");
        anim.SetBool("ISDEAD", true);
    }
}
