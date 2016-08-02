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
	public Transform m_audioDebugPanel;

	private Dictionary<DebugPanelType, Transform> m_panels;

	void Awake () 
	{
		Instance = this;

		//Create map of debug panel type to transform
		m_panels = new Dictionary<DebugPanelType, Transform>();
		m_panels.Add(DebugPanelType.Actuators, m_actuatorsDebugPanel);
		m_panels.Add(DebugPanelType.Lights, m_lightsDebugPanel);
		m_panels.Add(DebugPanelType.Audio, m_audioDebugPanel);
	}

	public void AddElement(GameObject objectToAdd, DebugPanelType panelType)
	{
		Transform debugPanel = m_panels[panelType];

		if (debugPanel && objectToAdd)
		{
			objectToAdd.transform.SetParent(debugPanel);	//Add to the debug hierarchy
			SortChildrenByName(debugPanel);					//Sort by name so everything is easy to locate
			objectToAdd.SetActive(false);					//Want to hide objects at first so menus are minimised
		}
	}

	void SortChildrenByName(Transform parent)
	{
		List<Transform> children = new List<Transform>(parent.childCount);

		//Create list of transforms
		for(int i=0; i< parent.childCount; i++)
		{
			children.Add(parent.GetChild(i));
		}

		//Sort transforms by name
		children.Sort((a,b) => a.name.CompareTo(b.name));

		//Re-add transforms with new indices
		for(int i=0; i<parent.childCount; i++)
		{
			children[i].SetSiblingIndex(i);
		}
	}
}
