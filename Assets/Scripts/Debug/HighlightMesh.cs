using UnityEngine;
using System.Collections;

public class HighlightMesh : MonoBehaviour 
{
	public Color m_color;
	private MaterialPropertyBlock m_block;

	void Awake()
	{
		m_block = new MaterialPropertyBlock();
	}

	public void Highlight () 
	{
		m_block.SetColor("_Color", m_color);
		UpdatePropertyBlock();	
	}

	public void ClearColor() 
	{
		m_block.Clear();
		UpdatePropertyBlock();	
	}

	void UpdatePropertyBlock()
	{
		Renderer [] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in renderers) {
			r.SetPropertyBlock (m_block);
		}
	}
}
