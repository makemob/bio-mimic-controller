using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DebugPanel : MonoBehaviour
{
	public string m_title;				//Title of panel to be drawn in UI
	public Text m_titleText;			//UI text element to draw title
	public Transform m_elementParent;	//Added debug elements will be attached to this
	private bool m_minimised = true;

	private List<DebugUIElement> m_elements = new List<DebugUIElement>();

	void Start()
	{
		UpdateTitle();
		UpdateElementVisibility();
	}

	public void AddElement(DebugUIElement element)
	{
		if (!m_elements.Contains(element))
			m_elements.Add(element);
		element.transform.SetParent(m_elementParent);

		UpdateTitle();
		SortElements();
		UpdateElementVisibility();
	}
		
	public int NumElements() 
	{
		return m_elements.Count;
	}

	public void Minimise() 
	{
		m_minimised = true;
		UpdateElementVisibility();
	}

	public void Maximise() 
	{
		m_minimised = false;
		UpdateElementVisibility();
	}

	public void ToggleMinimised()
	{
		m_minimised = !m_minimised;
		UpdateElementVisibility();
	}

	private void UpdateElementVisibility()
	{
		foreach(DebugUIElement e in m_elements)
		{
			e.gameObject.SetActive(!m_minimised);
		}
	}

	private void UpdateTitle()
	{
		m_titleText.text = m_title + " (" + NumElements () + ")";
	}

	private void SortElements()
	{
		SortChildrenByName(m_elementParent);
	}

	static private void SortChildrenByName(Transform parent)
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
