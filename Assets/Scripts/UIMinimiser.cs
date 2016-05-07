using UnityEngine;
using System.Collections;

public class UIMinimiser : MonoBehaviour 
{
	DebugUIElement [] m_disabledElements;
	bool m_minimised = false;

	public void Minimise () 
	{
		m_disabledElements = GetComponentsInChildren<DebugUIElement>();
		foreach(DebugUIElement e in m_disabledElements)
		{
			e.gameObject.SetActive(false);
		}

		m_minimised = true;
	}

	public void Maximise () 
	{
		foreach(DebugUIElement e in m_disabledElements)
		{
			e.gameObject.SetActive(true);
		}
		m_minimised = false;
	}

	public void Toggle()
	{
		if (m_minimised)
		{
			Maximise();
		}
		else
		{
			Minimise();
		}
	}
	

}
