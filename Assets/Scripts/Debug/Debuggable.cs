using UnityEngine;
using System.Collections;

//Base class for any object for which we'd like a corresponding ui element added to the menu.
public class Debuggable : MonoBehaviour 
{
	public GameObject m_debugPrefab;	//Prefab used to instantiate the debug ui for displaying info about this Debuggable object

	void Awake () 
	{
		if (m_debugPrefab)
		{
			//Create the object first
			GameObject debugObject = Instantiate(m_debugPrefab);		

			//Then connect this object to the debug ui
			if (debugObject)
			{
				debugObject.name = "DebugUI_" + gameObject.name;
				DebugUIElement uiElement = debugObject.GetComponent<DebugUIElement>();
				if (uiElement)
					uiElement.SetDebuggableObject(gameObject);				
			}
		}
	}
}
