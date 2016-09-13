using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UKI;



[RequireComponent(typeof(ModbusComms))]
public class ModbusRoboticsController : RoboticsController 
{
	const ushort STILL = 0;
	const ushort FORWARDS = 1;
	const ushort BACKWARDS = 2;
	const int MAX_ACTUATORS = 16;

	public bool m_useMultiRegister = true;
	public int m_timeout = 1000;

	private ModbusComms m_modbus;
	private List<Actuator> m_actuators = new List<Actuator>();

	public override void Startup()
	{
		m_modbus = GetComponent<ModbusComms>();
		m_modbus.Startup();
	}

	public override void Shutdown()
	{
		m_modbus.Shutdown();
	}

	public override void RegisterActuator(Actuator actuator)
	{
		if (!m_actuators.Contains(actuator))
			m_actuators.Add(actuator);

		m_actuators.Sort((a,b) => { return a.m_id.CompareTo(b.m_id); });
	}

	public override void SetActuatorSpeed(int actuatorID, float normalisedSpeed)
	{
		m_actuators[actuatorID].SetActuatorSpeed(normalisedSpeed);

//		ushort direction = 0;
//		ushort speed = 0;
//
//		GetSpeedAndDirection (normalisedSpeed, out speed, out direction);
//
//		ushort[] data = new ushort[] { direction, speed };

		//if (m_useMultiRegister) 
		//{
		//	m_modbus.WriteMultipleRegisters ((byte)actuatorID, 0, data);
		//} 
		//else 
		//{
		m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_MOTOR_SETPOINT, (ushort)(normalisedSpeed * 100.0f));
			//m_modbus.WriteSingleRegister ((byte)actuatorID, 1, speed);
		//}

		//m_modbus.WriteSingleRegister (MB_SCARAB_ID1, MB_MOTOR_SPEED, 255);
		//m_modbus.
	}

	public override void SetAllActuatorSpeeds(float normalisedSpeed)
	{
		for (int i=0; i < m_actuators.Count; i++)
			SetActuatorSpeed (i, normalisedSpeed);
	}

	public override void SetAllActuatorSpeeds(List<float> speeds)
	{
		int count = Mathf.Min (speeds.Count, m_actuators.Count);
		for (int i=0; i < count; i++)
			SetActuatorSpeed (i, speeds[i]);
	}

	public override void StopActuator(int actuatorID)
	{
		m_actuators[actuatorID].SetActuatorSpeed(0.0f);

		m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_MOTOR_SETPOINT, (ushort)0);
	}

	public override void StopAllActuators()
	{
		for(var i=0; i < m_actuators.Count; i++)
			StopActuator(i);	
	}

	public override void Stop()
	{
		StopAllActuators ();
	}

	public override ActuatorState GetActuatorState (int actuatorID)
	{
		//MB_BRIDGE_CURRENT =	100,
		//MB_BATT_VOLTAGE = 101,
		//MB_MAX_BATT_VOLTAGE = 102,
		//MB_MIN_BATT_VOLTAGE = 103,
		//MB_BOARD_TEMPERATURE = 104,

		//TODO: run on separate thread?
		//m_modbus.Rea
		ushort [] result = m_modbus.ReadHoldingRegisters ((byte)actuatorID, (ushort)ModbusRegister.MB_BRIDGE_CURRENT, (ushort)5);
		ActuatorState s = new ActuatorState ();
		if (result != null && result.Length > 0) 
		{
			s.m_bridgeCurrent = result [0];
			s.m_batteryVoltage = result [1];
			s.m_boardTemperature = result [4];
		}

		//Hacky state update here
		if (actuatorID < m_actuators.Count)
			m_actuators[actuatorID].m_state = s;

		//TODO: Fill out full state

		return s;
	}

	public void ToggleMultiRegister()
	{
		m_useMultiRegister = !m_useMultiRegister;
	}

	public void UseMultiRegister(bool doUseMultiRegister)
	{
		m_useMultiRegister = doUseMultiRegister;
	}

	//
	// Helper function to return the speed and direction from a given normalised speed (-1.0f to 1.0f)
	//
	private void GetSpeedAndDirection (float normalisedSpeed, out ushort speed, out ushort direction)
	{
		speed =  Convert.ToUInt16(Math.Abs(normalisedSpeed) * 255.0f);

		direction = STILL;				//still by default
		if (normalisedSpeed > 0.0f)			
			direction = FORWARDS;		//forwards
		else if (normalisedSpeed > 0.0f)
			direction = BACKWARDS;		//backwards
	}
}
