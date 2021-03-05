using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssemblySettings", menuName = "ScriptableObjects/AssemblySettings", order = 2)]
public class AssemblySettings : ScriptableObject
{
	public bool stepAssembly;

	[Space]
	public float distFromTargetPos = .08f;
    public float snapPartSpeed = .2f;
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
