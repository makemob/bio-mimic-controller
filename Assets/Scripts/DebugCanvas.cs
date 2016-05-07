using UnityEngine;
using System.Collections;

public class DebugCanvas : MonoBehaviour 
{
	public enum DebugPanelType
	{
		Actuators,
		Lights,
		Audio
	}

	public static DebugCanvas Instance;

	public Transform m_actuatorsDebugPanel;
	public Transform m_lightsDebugPanel;

	void Awake () 
	{
		Instance = this;
	}

	public void AddElement(GameObject objectToAdd, DebugPanelType panelType)
	{
		bool success = false;

		switch(panelType)
		{
		case DebugPanelType.Actuators:
			if (m_actuatorsDebugPanel)
			{
				objectToAdd.transform.SetParent(m_actuatorsDebugPanel);
				success = true;
			}
			break;
		case DebugPanelType.Lights:
			if (m_lightsDebugPanel)
			{
				objectToAdd.transform.SetParent(m_lightsDebugPanel);
				success = true;
			}
			break;
		default:
			break;
		}

		if (!success)
			Debug.Log("Unable to add debug element. No parent!");
	}

	void Update () 
	{
	
	}
}
