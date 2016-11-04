using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour 
{
	public GameObject other;
		
	void Update () 
	{
		if (other)
			transform.LookAt (other.transform);
	}
}
