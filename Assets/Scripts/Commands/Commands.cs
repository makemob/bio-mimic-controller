using UnityEngine;
using System.Collections;
using System;
using UKI;

public interface ICommand
{
	void Execute ();
}

//
// 
//
public class Command_StopActuator : ICommand {
	
	IRoboticsController m_roboticsController;
	byte m_actuatorID;

	public Command_StopActuator (IRoboticsController r, byte actuatorID) {
		m_roboticsController = r;
		m_actuatorID = actuatorID;
	}

	public void Execute () { 
		Debug.Log ("Executing Command_StopActuator");
		m_roboticsController.StopActuator (m_actuatorID); 
	}
}

//
// 
//
public class Command_SetActuatorSpeed : ICommand
{
	IRoboticsController m_roboticsInterface; 
	float m_speed; 
	byte m_actuatorID;

	public Command_SetActuatorSpeed (IRoboticsController r, byte actuatorID, float speed) {
		m_roboticsInterface = r; 
		m_speed = speed; 
		m_actuatorID = actuatorID;
	}

	public void Execute () { 
		Debug.Log ("Executing Command_SetActuatorSpeed");
		m_roboticsInterface.SetActuatorSpeed(m_actuatorID, m_speed); 
	}
}

//
//
//
public class Command_GenericAction : ICommand
{
	Action m_action;

	public Command_GenericAction (Action a) {
		m_action = a;
	}

	public void Execute () { 
		Debug.Log ("Executing Command_GenericAction");
		m_action.Invoke();
	}
}
