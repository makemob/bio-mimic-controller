using UnityEngine;
using System.Collections;

public class UIMinimiser : MonoBehaviour 
{
	private bool m_minimised = true;

	public void Minimise () 
	{
		DebugUIElement [] elements = GetComponentsInChildren<DebugUIElement>(true);
		foreach(DebugUIElement e in elements)
		{
			e.gameObject.SetActive(false);
		}

		m_minimised = true;
	}

	public void Maximise () 
	{
		DebugUIElement [] elements = GetComponentsInChildren<DebugUIElement>(true);
		foreach(DebugUIElement e in elements)
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
