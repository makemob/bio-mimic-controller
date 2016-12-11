using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ActuatorDebugUI : DebugUIElement, IPointerEnterHandler, IPointerExitHandler
{
	public Actuator m_actuator;
	public Text m_label;
	public GraphElement m_simulationBar;
	public GraphElement m_sensorBar;
	public Text m_state;
	public UnityEvent m_onActuatorRead;

	private Color m_canvasColor;
	private bool m_highighlighted;

	public void OnPointerEnter(PointerEventData e)
	{
		m_canvasColor = new Color32 (192, 192, 192, 128);
		m_highighlighted = true;
		//GetComponent<Image> ().color = new Color32 (255, 128, 128, 128);
	}

	public void OnPointerStay(PointerEventData e)
	{
		OnPointerEnter (e);
	}

	public void OnPointerExit(PointerEventData e)
	{
		m_highighlighted = false;
		m_canvasColor = new Color32 (255, 255, 255, 128);

		//GetComponent<Image> ().color = new Color32 (255, 255, 255, 128);
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


			if (!m_highighlighted) 
			{
				Color newColor = new Color (1.0f, 1.0f, 1.0f, 0.5f);

				//if (m_actuator.m_state.m_innerCurrentTripped || m_actuator.m_state.m_outerCurrentTripped) 
				if (m_actuator.m_moveSpeed > 0.0f) {
					newColor.r = 0.0f;
					newColor.g = 0.8f;
					newColor.b = 0.0f;

				} 
				else if (m_actuator.m_moveSpeed < 0.0f) 
				{
					newColor.r = 0.0f;
					newColor.g = 0.8f;
					newColor.b = 0.8f;

				}
				else if (m_actuator.m_state.m_innerCurrentTripped || m_actuator.m_state.m_outerCurrentTripped) {
					newColor.r = 0.9f;
					newColor.g = 0.0f;
					newColor.b = 0.0f;
				}

				m_canvasColor = newColor;
			}

			GetComponent<Image>().color = m_canvasColor;

		}
	}

	public override void SetDebuggableObject(GameObject debuggableObject)
	{
		if (debuggableObject) {
			m_actuator = debuggableObject.GetComponent<Actuator> ();
			m_actuator.m_onStateUpdate.AddListener (DrawState);
			m_actuator.m_onActuatorRead.AddListener (m_onActuatorRead.Invoke);
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
		string maxExtentString = "\nmax (mm): " + m_actuator.m_config.maxExtent;
		string moveSpeedString = "\nspeed: " + m_actuator.m_moveSpeed;

		m_state.text = m_actuator.name + maxExtentString + moveSpeedString + m_actuator.m_state.ToString ();
	}
}
