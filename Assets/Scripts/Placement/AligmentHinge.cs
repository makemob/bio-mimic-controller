﻿using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class AligmentHinge : MonoBehaviour 
{
	public GameObject start;
	public GameObject end;
	public GameObject centre;
	public bool hideNodes = true;

	private Vector3 lastStart;
	private Vector3 lastEnd;

	void Update () 
	{	
			
		if (start && end) 
		{
#if UNITY_EDITOR
            if (hideNodes) {
				start.hideFlags |= HideFlags.HideInHierarchy;
				end.hideFlags |= HideFlags.HideInHierarchy;
				if (centre)
					centre.hideFlags |= HideFlags.HideInHierarchy;
			} else {
				start.hideFlags &= ~HideFlags.HideInHierarchy;
				end.hideFlags &= ~HideFlags.HideInHierarchy;
				if (centre)
					centre.hideFlags &= ~HideFlags.HideInHierarchy;
			}
#endif

            if (lastStart == start.transform.position && lastEnd == end.transform.position) 
			{
				start.transform.LookAt (end.transform);
				end.transform.forward = start.transform.forward;

				start.transform.SetParent (null, true);
				end.transform.SetParent (null, true);

				transform.position = (start.transform.position + end.transform.position) * 0.5f;
				transform.LookAt (end.transform);

				start.transform.SetParent (transform, true);
				end.transform.SetParent (transform, true);
			}

			lastStart = start.transform.position;
			lastEnd = end.transform.position;
		}
	}
}
