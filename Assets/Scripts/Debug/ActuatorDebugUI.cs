using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActuatorDebugUI : DebugUIElement, IPointerEnterHandler, IPointerExitHandler
{
	public Actuator m_actuator;
	public Text m_label;
	public GraphElement m_simulationBar;
	public GraphElement m_sensorBar;
	public Text m_state;

	public void OnPointerEnter(PointerEventData e)
	{
		GetComponent<Image> ().color = new Color32 (255, 128, 128, 128);
	}

	public void OnPointerExit(PointerEventData e)
	{
		GetComponent<Image> ().color = new Color32 (255, 255, 255, 128);
	}


	void Update () 
	{
		
		//GetComponent<Selectable>().OnDeselect(
		if (m_actuator)
		{
			if (m_label)
				m_label.text = m_actuator.m_config.id.ToString();

			if (m_simulationBar)
			{
				float position = m_actuator.GetNormalisedPosition();
				m_simulationBar.SetNormalisedValue(position);
			}

			if (m_sensorBar) 
			{
				float position = m_actuator.GetNormalisedDesiredPosition ();
				m_sensorBar.SetNormalisedValue (position);
			}
		}
	}

	public override void SetDebuggableObject(GameObject debuggableObject)
	{
		if (debuggableObject) {
			m_actuator = debuggableObject.GetComponent<Actuator> ();
			m_actuator.m_onStateUpdate.AddListener (DrawState);
		}
	}

	public void OnUp()
	{
		Debug.Log("Up pressed.");
		MasterController.Instance.SetActuatorSpeed (GetActuatorID (), 1.0f);
	}
		
	public void OnDown()
	{
		Debug.Log("Down pressed.");
		MasterController.Instance.SetActuatorSpeed (GetActuatorID (), -1.0f);

	}

	public void OnStop()
	{
		Debug.Log("Stop pressed.");
		MasterController.Instance.StopActuator(GetActuatorID());

	}

	public void OnTest()
	{
		MasterController.Instance.CallibrateActuator (GetActuatorID ());
	}

	public void Highlight(bool doHighlight)
	{
		if (doHighlight) 
			m_actuator.GetComponent<HighlightMesh> ().Highlight();
		else
			m_actuator.GetComponent<HighlightMesh> ().ClearColor();	
	}

	private int GetActuatorID()
	{
		return m_actuator.GetID();
	}

	private void DrawState()
	{
		m_state.text = m_actuator.name + m_actuator.m_state.ToString ();
	}
}
