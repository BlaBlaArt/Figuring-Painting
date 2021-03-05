using System.Net;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum LayMode
{
    Default,
    Side,
    Back
}

public class PartController : MonoBehaviour
{
    public MeshRenderer meshRend;

    [Space]
    [SerializeField] PartSettings settings;
    [SerializeField] Rigidbody rb;

    [Space]
    [SerializeField] LayMode layMode;

    [Space]
    [SerializeField] int placementIndex = -1;

    [Space]
    [ReadOnly] public int partID;

    public static UnityEvent<PartController> onGrabStart;
    public static UnityEvent<PartController> onGrabStop;

    float _followSmooth = 15f;

    Vector3 _startPos;
    Quaternion _startRot;

    Vector3 _endPos;
    Quaternion _endRot;
    float _startDist;

    Vector3 _targetPosition;

    List<IMoveingPart> _moveingParts;

    void Awake()
    {
        _moveingParts = GetComponentsInChildren<IMoveingPart>().ToList();

        _endPos = transform.position;
        _endRot = transform.rotation;
        //this needs to be in awake
    }

    void OnMouseDown()
    {
        transform.DOKill();

        TogglePhysics(false);

        Taptic.Light();
      //  onGrabStart.Invoke(this);
    }

    void OnMouseDrag()
    {
        float dist = Mathf.InverseLerp(0, _startDist, Vector3.Distance(transform.position, _endPos));

        //Ray ray = InputController.inst.GetCameraRay();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, settings.maxDistFromCam, settings.floorMask))
            _targetPosition = new Vector3(hit.point.x, Mathf.Lerp(_endPos.y, hit.point.y + settings.floorHeight, dist), hit.point.z);

        transform.position = Vector3.Lerp(transform.position, _targetPosition, _followSmooth * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(_endRot, _startRot, dist);
    }

    void OnMouseUp()
    {
        TogglePhysics(true);

      //  onGrabStop.Invoke(this);
    }

    public void PlacementReady()
    {
        transform.rotation = Quaternion.Euler(layMode == LayMode.Back ? -90f : 0f, Random.Range(-20f, 20f), layMode == LayMode.Side ? 90f : 180f);//rotating first coz meshRend.bounds is world based
       // if (placementIndex > -1)
       //     transform.position = LevelController.inst.RequestPosition(placementIndex) + Vector3.up * meshRend.bounds.extents.y;
       // else
       //     transform.position = LevelController.inst.RequestPosition() + Vector3.up * meshRend.bounds.extents.y;

        _startPos = transform.position;
        _startRot = transform.rotation;

        _targetPosition = _startPos;

        _startDist = Vector3.Distance(_startPos, _endPos);
    }

    public void PlacementReport(bool successful)
    {
        if (successful)
        {
            TogglePhysics(false);

            gameObject.layer = 2;


            if (_moveingParts != null)
            {
                //this.Invoke(settings.moveingPartDelay, () => _moveingParts.ForEach(x => x.Execute()));
            }

            if (settings.insertVfx)
            {
                
            }
               // Lean.Pool.LeanPool.Spawn(settings.insertVfx, transform.position, transform.rotation);
           
           // TutorialController.inst.HideTutorialAssembly();
        }
    }

    public void TogglePhysics(bool on)
    {
        rb.isKinematic = !on;
        rb.useGravity = on;
    }

    [Button]
    public void Setup()
    {
        meshRend = GetComponent<MeshRenderer>();

        MeshCollider mc = GetComponent<MeshCollider>();
        if (!mc)
            mc = gameObject.AddComponent<MeshCollider>();
        mc.convex = true;

        if (!rb)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Extrapolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        gameObject.layer = 10;

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }
}
