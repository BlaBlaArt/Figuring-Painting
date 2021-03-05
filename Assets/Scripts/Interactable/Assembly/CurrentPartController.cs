using System.Collections.Generic;
using DG.Tweening;
//using MEC;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPartController : MonoBehaviour
{
    [SerializeField] MeshFilter mesh;
    [SerializeField] MeshRenderer rend;
    [SerializeField] Outline outline;

    [SerializeField] GameObject bg;
 
    [Space]
    [SerializeField] float assembleDelay = 1f;

    [Space]
    [SerializeField] Vector2 minMaxSize;

    UnboxController unboxC;

    PartController _currentPart;

    void Start()
    {
       // GameC.inst.onLevelLoad += OnLevelLoad;
       // GameC.inst.onLevelUnload += Help;

        void Help()
        {
            if (unboxC)
                unboxC.onBoxOpen.RemoveAllListeners();
            bg.SetInactive();
        }
    }

    void OnLevelLoad(int _)
    {
        gameObject.SetInactive();

        unboxC = FindObjectOfType<UnboxController>();
        if (unboxC)
            unboxC.onBoxOpen.AddListener(OnAssemble);
    }

    void OnAssemble()
    {
        unboxC.onBoxOpen.RemoveListener(OnAssemble);

        AssemblyController assemblyC = FindObjectOfType<AssemblyController>();
        assemblyC.onPartNew.AddListener(ChangePart);
        assemblyC.onAssemblyFinished.AddListener(OnAssemblyFinished);
        outline.enabled = true;

        //Timing.RunCoroutine(_Help());
        //IEnumerator<float> _Help()
        //{
        //    yield return Timing.WaitForSeconds(assembleDelay);
        //    gameObject.SetActive();
        //    bg.SetActive();
        //}
    }

    void OnAssemblyFinished()
    {
        gameObject.SetInactive();
        bg.SetInactive();
    }

    void ChangePart(PartController pc)
    {
        rend.material = pc.meshRend.material;
        mesh.mesh = pc.GetComponent<MeshFilter>().mesh;
        transform.localScale = Vector3.one;
        float size = Mathf.Max(mesh.mesh.bounds.size.z, mesh.mesh.bounds.size.y);
        transform.localScale = Vector3.one * Mathf.Lerp(minMaxSize.y, minMaxSize.x, Mathf.InverseLerp(.1f, .6f, size));
    }
}
