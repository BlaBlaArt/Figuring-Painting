using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartSettings", menuName = "ScriptableObjects/PartSettings", order = 3)]
public class PartSettings : ScriptableObject
{
	public float maxDistFromCam;
	public LayerMask floorMask;
	public float floorHeight;

	[Space]
	public float snapBackSpeed;

	[Space]
	public float dropTime = .6f;

	[Space]
	public float moveingPartDelay = .25f;

	public ParticleSystem insertVfx;
	// void Awake()
	// {
        	
	// }

	// void OnEnable()
	// {
        	
	// }

	// void OnDisable()
	// {
        	
	// }

	// void OnDestroy()
	// {
        	
	// }
}
