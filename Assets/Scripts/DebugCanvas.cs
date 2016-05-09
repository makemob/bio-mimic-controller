using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
				SortChildrenByName(m_actuatorsDebugPanel);
				objectToAdd.SetActive(false);
				success = true;
			}
			break;
		case DebugPanelType.Lights:
			if (m_lightsDebugPanel)
			{
				objectToAdd.transform.SetParent(m_lightsDebugPanel);
				SortChildrenByName(m_lightsDebugPanel);
				objectToAdd.SetActive(false);
				success = true;
			}
			break;
		default:
			break;
		}
			
		if (!success)
			Debug.Log("Unable to add debug element. No parent!");
	}


	void SortChildrenByName(Transform parent)
	{
		List<Transform> children = new List<Transform>(parent.childCount);

		for(int i=0; i< parent.childCount; i++)
		{
			children.Add(parent.GetChild(i));
		}

		children.Sort((a,b) => a.name.CompareTo(b.name));

		for(int i=0; i<parent.childCount; i++)
		{
			children[i].SetSiblingIndex(i);
		}
	}

	void Update () 
	{
	
	}
}
