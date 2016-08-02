using UnityEngine;
using System.Collections;

public class GraphElement : MonoBehaviour 
{	
	public void SetNormalisedValue (float value) 
	{
		Vector3 newScale = transform.localScale;
		newScale.y = value;
		transform.localScale = newScale;
	}
}
