using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public float m_actuatorStateUpdateInterval = 1.0f;	//Time between state updates 

	private ModbusComms m_modbus;
	private Dictionary<int, Actuator> m_actuators = new Dictionary<int, Actuator>();

	public override void Startup()
	{
		m_modbus = GetComponent<ModbusComms>();
		m_modbus.Startup();

		StartCoroutine("UpdateActuatorStateLoop");
	}

	public override void Shutdown()
	{
		m_modbus.Shutdown();

		StopCoroutine("UpdateActuatorStateLoop");
	}

	public override bool RegisterActuator(Actuator actuator)
	{
		Assert.IsNotNull(m_modbus, "Failed to register actuator. Modbus not started.");

		m_actuators [actuator.GetID()] = actuator;

		m_modbus.WriteSingleRegister((byte)actuator.GetID(), 
									 (ushort)ModbusRegister.MB_DEFAULT_CURRENT_LIMIT_INWARD, 
									 (ushort)actuator.m_config.inwardCurrentLimit);
		
		m_modbus.WriteSingleRegister((byte)actuator.GetID(), 
									 (ushort)ModbusRegister.MB_CURRENT_LIMIT_OUTWARD, 
									 (ushort)actuator.m_config.outwardCurrentLimit);
		
		//TODO: Sort dictionary
		//m_actuators.Sort((a,b) => { return a.m_id.CompareTo(b.m_id); });

		return true;
	}

	public override void SetActuatorSpeed(int actuatorID, float normalisedSpeed)
	{
		m_actuators[actuatorID].SetActuatorSpeed(normalisedSpeed);
		m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_MOTOR_SETPOINT, (ushort)(normalisedSpeed * 89.0f));
	}

	public override void SetAllActuatorSpeeds(float normalisedSpeed)
	{
		foreach(Actuator a in m_actuators.Values)
			SetActuatorSpeed (a.GetID(), normalisedSpeed);
	}

	public override void StopActuator(int actuatorID)
	{
		m_actuators[actuatorID].SetActuatorSpeed(0.0f);

		m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_MOTOR_SETPOINT, (ushort)0);
	}

	public override void StopAllActuators()
	{
		foreach(Actuator a in m_actuators.Values)
			StopActuator(a.GetID());	
	}

	public override void Stop()
	{
		StopAllActuators ();
	}

	public override ActuatorState GetActuatorState (int actuatorID)
	{
		//TODO: run on separate thread?
		//TODO: Fill out full state
		//TODO: Remap registers so we can do a state update with a single read

		ActuatorState s = new ActuatorState();

		ushort[] diagnostics;
		if (ReadRegisters (actuatorID, ModbusRegister.MB_BRIDGE_CURRENT, 5, out diagnostics)) 
		{
			s.m_bridgeCurrent = diagnostics[0];
			s.m_batteryVoltage = diagnostics[1];
			s.m_boardTemperature = diagnostics[4];
		}
			
		ushort[] currentrips;
		if (ReadRegisters (actuatorID, ModbusRegister.MB_CURRENT_TRIPS_INWARD, 2, out currentrips)) 			
		{
			s.m_innerCurrentTrips = currentrips[0];
			s.m_outerCurrentTrips = currentrips[1];
		}

		ushort[] extentSwitches;
		if (ReadRegisters (actuatorID, ModbusRegister.MB_INWARD_ENDSTOP_STATE, 2, out extentSwitches)) 			
		{
			s.m_atInnerLimit = extentSwitches[0] > 0;
			s.m_atOuterLimit = extentSwitches[1] > 0;
		}
			
		//Hacky state update here
		if (actuatorID < m_actuators.Count)
			m_actuators[actuatorID].m_state = s;

		return s;
	}

	public override void UpdateAllActuatorStates()
	{
		foreach(Actuator a in m_actuators.Values)
			GetActuatorState(a.GetID());
	}

	public void ToggleMultiRegister()
	{
		m_useMultiRegister = !m_useMultiRegister;
	}

	public void UseMultiRegister(bool doUseMultiRegister)
	{
		m_useMultiRegister = doUseMultiRegister;
	}

	bool ReadRegisters(int actuatorID, ModbusRegister startRegister, int count, out ushort [] result)
	{
		result =  m_modbus.ReadHoldingRegisters ((byte)actuatorID, (ushort)startRegister, (ushort)count);
		return result != null && result.Length == count;
	}

	//Loop thorugh each actuator and update its state
	IEnumerator UpdateActuatorStateLoop()
	{
		int index = -1;
		while (true) {
			int [] actuatorIDs = m_actuators.Keys.ToArray();
			int actuatorCount = actuatorIDs.Length;
			if (actuatorCount > 0)
			{
				index = (index + 1) % actuatorCount;
				int currentID = actuatorIDs [index];
				ActuatorState state = GetActuatorState(currentID);
				//Debug.Log ("Actuator " + currentID + ": " + state.ToString ());
			}
			yield return new WaitForSeconds(m_actuatorStateUpdateInterval);
		}
	}
}
