using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimationController
{
    public void Run();
    public void Attack();
    public void Idle();
    public void OnDrag(bool isStart);
    public void OnWin();
}
