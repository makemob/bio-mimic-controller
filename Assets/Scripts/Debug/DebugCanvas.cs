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

	public DebugPanel m_actuatorsDebugPanel;
	public DebugPanel m_lightsDebugPanel;
	public DebugPanel m_audioDebugPanel;

	private Dictionary<DebugPanelType, DebugPanel> m_panels;

	void Awake () 
	{
		Instance = this;

		//Create map of debug panel type to transform
		m_panels = new Dictionary<DebugPanelType, DebugPanel>();
		m_panels.Add(DebugPanelType.Actuators, m_actuatorsDebugPanel);
		m_panels.Add(DebugPanelType.Lights, m_lightsDebugPanel);
		m_panels.Add(DebugPanelType.Audio, m_audioDebugPanel);
	}

	public void AddElement(DebugUIElement objectToAdd, DebugPanelType panelType)
	{
		DebugPanel debugPanel = m_panels[panelType];

		if (debugPanel && objectToAdd)
			debugPanel.AddElement(objectToAdd);
	}

}
