using UnityEngine;
using System.Collections;

public struct CallibrationResults
{
	public float m_extensionTime;
	public float m_retractionTime;

	public override string ToString ()
	{
		return string.Format ("[CallibrationResults]\nExtension Time: {0}\nRetraction Time: {1}", 
			m_extensionTime, 
			m_retractionTime);
	}
}

public class Callibrate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
}
