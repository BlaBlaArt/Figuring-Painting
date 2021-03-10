using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAnimationController : MonoBehaviour, IAnimationController
{
    [SerializeField] private Animator anim;
    private static readonly int IsRun = Animator.StringToHash("IsRun");
    private static readonly int Attack1 = Animator.StringToHash("Attack");

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

    public void OnDrag()
    {
        
    }
}
