using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActuatorDebugUI : DebugUIElement 
{
	public Actuator m_actuator;
	public Text m_label;
	public GraphElement m_simulationBar;
	public GraphElement m_sensorBar;

	void Update () 
	{
		if (m_actuator)
		{
			if (m_label)
				m_label.text = GetActuatorID().ToString();//m_actuator.name;

			if (m_simulationBar)
			{
				float position = m_actuator.GetNormalisedPosition();
				m_simulationBar.SetNormalisedValue(position);
			}

			if (m_sensorBar)
				m_sensorBar.SetNormalisedValue(0.5f);
		}
	}

	public override void SetDebuggableObject(GameObject debuggableObject)
	{
		if (debuggableObject)
			m_actuator = debuggableObject.GetComponent<Actuator>();
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

	private int GetActuatorID()
	{
		return m_actuator.GetID();
	}
}
