using UnityEngine;
using System.Collections;

public class DebugUIElement : MonoBehaviour 
{
	public DebugCanvas.DebugPanelType m_type;

	public virtual void Start()
	{
		//Add this object to the debug canvas 
		DebugCanvas.Instance.AddElement(gameObject, m_type);
	}

	//Override this to set subclass members and attach graphs, text etc
	public virtual void SetDebuggableObject(GameObject debuggableObject)
	{
		
	}
}
