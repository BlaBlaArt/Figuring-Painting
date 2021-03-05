using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveingPart
{
    void Execute();
    void Revert();
    void Revert(float time);
}
