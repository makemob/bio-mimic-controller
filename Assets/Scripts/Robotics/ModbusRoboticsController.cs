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
	//public bool m_useMultiRegister = true;
	public bool m_useMultiRead = false;
	//public int m_timeout = 1000;
	public float m_actuatorStateUpdateInterval = 1.0f;	//Time between state updates 

	ModbusComms m_modbus;
	Dictionary<int, Actuator> m_actuators = new Dictionary<int, Actuator>();

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

		//TODO: Check writing immediately no longer locks up. We should be ok.

		m_modbus.WriteSingleRegister((byte)actuator.GetID(), 
									 (ushort)ModbusRegister.MB_CURRENT_LIMIT_INWARD, 
									 (ushort)actuator.m_config.inwardCurrentLimit);
		
		m_modbus.WriteSingleRegister((byte)actuator.GetID(), 
									 (ushort)ModbusRegister.MB_CURRENT_LIMIT_OUTWARD, 
									 (ushort)actuator.m_config.outwardCurrentLimit);

		actuator.m_onMaxLimitReached.RemoveAllListeners ();
		actuator.m_onMaxLimitReached.AddListener(() => { this.StopActuator (actuator.GetID ());} );

		//Do an initial actuator read to determine current trip counts etc
		ActuatorState state = ReadActuatorState(actuator.GetID());
		UpdateActuator (actuator, state, false);				

		//TODO: Sort dictionary
		//m_actuators.Sort((a,b) => { return a.m_id.CompareTo(b.m_id); });

		return true;
	}

	public override void MoveActuatorTowardsPosition (int actuatorID, float position)
	{
		m_actuators [actuatorID].SetDesiredPosition (position);
		bool closeEnough = CloseEnoughToPosition (actuatorID, position);

		if (!closeEnough) 
		{
			float positionError = position - m_actuators[actuatorID].m_state.m_predictedExtension;
			SetActuatorSpeed (actuatorID, Mathf.Sign (positionError) * 1.0f);
		}
	}

	public override bool CloseEnoughToPosition (int actuatorID, float desiredPosition)
	{
		const float tolerance = 5.0f;
		float error = desiredPosition - m_actuators[actuatorID].m_state.m_predictedExtension;

		//This takes movement direction and overshoot in to account when testing for tolerance.
		if (m_actuators [actuatorID].m_moveSpeed == 0.0f && Mathf.Abs(error) <= tolerance) 
		{
			return true;
		}

		if (m_actuators [actuatorID].m_moveSpeed > 0.0f && error <= tolerance) 
		{
			return true;
		}

		if (m_actuators [actuatorID].m_moveSpeed < 0.0f && error >= -tolerance) 
		{
			return true;
		}
		
		return false;

	}

	public override void SetActuatorSpeed(int actuatorID, float normalisedSpeed)
	{
		//Check inner current trip limit 
		if (m_actuators [actuatorID].m_state.m_innerCurrentTripped || 
			m_actuators [actuatorID].m_state.m_atInnerLimit) 
		{
			if (normalisedSpeed <= 0.0f)
				return; //Prevent attempts at continued movement

			ResetEmergencyStop(actuatorID);
		}

		//Check outer current trip limit
		if (m_actuators [actuatorID].m_state.m_outerCurrentTripped || 
			m_actuators [actuatorID].m_state.m_atOuterLimit) 
		{
			if (normalisedSpeed >= 0.0f)
				return;	//Prevent attempts at continued movement

			ResetEmergencyStop(actuatorID);

		}
			
		m_actuators[actuatorID].SetActuatorSpeed(normalisedSpeed);
		m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_MOTOR_SETPOINT, (ushort)(normalisedSpeed * 30.0f));	//0.89
	}

	public override void SetActuatorAcceleration(int actuatorID, float acceleration)
	{
		m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_MOTOR_ACCEL, (ushort)(acceleration * 100.0f));	//0.89
	}


	public override void SetActuatorCallibration(int actuatorID, CallibrationResults results)
	{
		m_actuators [actuatorID].SetCallibration (results);
	}

	public override void SetAllActuatorSpeeds(float normalisedSpeed)
	{
		foreach(Actuator a in m_actuators.Values)
			SetActuatorSpeed (a.GetID(), normalisedSpeed);
	}

	public override void SetActuatorSpeeds (List<int> ids, float normalisedSpeed)
	{
		foreach(int id in ids)
			SetActuatorSpeed (id, normalisedSpeed);
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

	public override ActuatorState ReadActuatorState (int actuatorID)
	{
		//TODO: run on separate thread?
		//TODO: Fill out full state
		//TODO: Remap registers so we can do a state update with a single read

		ActuatorState s = new ActuatorState();

		//TODO: Re-enable 
		if (m_useMultiRead) 
		{
			ushort[] diagnostics;
			if (ReadRegisters (actuatorID, ModbusRegister.MB_BRIDGE_CURRENT, 5, out diagnostics)) {
				s.m_bridgeCurrent = diagnostics [0];
				s.m_batteryVoltage = diagnostics [1];
				s.m_boardTemperature = diagnostics [4];
			}
		}
			
		ushort[] data;
		if (ReadRegisters (actuatorID, ModbusRegister.MB_MOTOR_SETPOINT, 17, out data)) 			
		{
			s.m_motorSetPoint = data [0];
			s.m_motorSpeed = data [1];
			s.m_motorAcceleration = data [2];
			s.m_innerCurrentLimit = data [3];
			s.m_outerCurrentLimit = data [4];
			s.m_innerCurrentTrips = data [5];
			s.m_outerCurrentTrips = data [6];
			s.m_voltageTrips = data [7];
			s.m_innerLimitCount = data [14];
			s.m_outerLimitCount = data [15];

			//s.m_atInnerLimit = data [14] > 0;	//This doesn't work. Can we rely on count remaining the same and so simply test if this number has gone up by one?
			//s.m_atOuterLimit = data [15] > 0;
			s.m_heartBeat = data [16];

			//s.m_innerCurrentTrips = data[0];
			//s.m_outerCurrentTrips = data[1];
		}

		//Add second read back in for now, for reliable microswitch test
//		ushort[] extentSwitches;
//		if (ReadRegisters (actuatorID, ModbusRegister.MB_INWARD_ENDSTOP_STATE, 2, out extentSwitches)) 			
//		{
//			s.m_atInnerLimit = extentSwitches[0] > 0;
//			s.m_atOuterLimit = extentSwitches[1] > 0;
//		}

		if (m_useMultiRead) 
		{
			ushort[] currentLimits;
			if (ReadRegisters (actuatorID, ModbusRegister.MB_CURRENT_LIMIT_INWARD, 2, out currentLimits)) {
				s.m_innerCurrentLimit = currentLimits [0];
				s.m_outerCurrentLimit = currentLimits [1];
			}
		}

		return s;
	}

	public override ActuatorState GetActuatorState(int actuatorID)
	{
		return m_actuators[actuatorID].m_state;
	}

	public override ActuatorState [] GetAllActuatorStates()
	{
		List<ActuatorState> states = new List<ActuatorState> ();
		foreach (Actuator a in m_actuators.Values) {
			states.Add (a.m_state);
		}
		return states.ToArray();
	}

	public override ActuatorState [] GetActuatorStates (List<int> ids)
	{
		List<ActuatorState> states = new List<ActuatorState> (ids.Count);
		for(int i=0; i < ids.Count; i++)
		{
			int currentID = ids [i];
			states.Add (m_actuators[currentID].m_state);
		}
		return states.ToArray();
	}

		
	public override void UpdateAllActuatorStates()
	{
		foreach(Actuator a in m_actuators.Values)
			ReadActuatorState(a.GetID());
	}

	public void ToggleMultiRegister()
	{
		//m_useMultiRegister = !m_useMultiRegister;
	}

	public void UseMultiRegister(bool doUseMultiRegister)
	{
		//m_useMultiRegister = doUseMultiRegister;
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
			yield return new WaitForSeconds(m_actuatorStateUpdateInterval);

			int [] actuatorIDs = m_actuators.Keys.ToArray();
			int actuatorCount = actuatorIDs.Length;
			if (actuatorCount > 0)
			{
				index = (index + 1) % actuatorCount;
				int currentID = actuatorIDs [index];
				ActuatorState state = ReadActuatorState(currentID);
				UpdateActuator (m_actuators [currentID], state, true);
			}
		}
	}

	void UpdateActuator(Actuator actuator, ActuatorState newState, bool detectTripsAndSwitches)
	{
		if (detectTripsAndSwitches) 
		{
			int oldInnerLimitCount = actuator.m_state.m_innerLimitCount;
			int oldOuterLimitCount = actuator.m_state.m_outerLimitCount;

			//Set limit flags
			if (!m_useMultiRead) { //TODO: Remove multi read option when we can!
				if (newState.m_innerLimitCount > oldInnerLimitCount)
					newState.m_atInnerLimit = true;

				if (newState.m_outerCurrentTrips > oldOuterLimitCount)
					newState.m_atOuterLimit = true;
			}

			//set trip flags
			if (newState.m_innerCurrentTrips > actuator.m_state.m_innerCurrentTrips)
				newState.m_innerCurrentTripped = true;
			if (newState.m_outerCurrentTrips > actuator.m_state.m_outerCurrentTrips)
				newState.m_outerCurrentTripped = true;
		}

		//TODO: Refactor this and wait on current and switch events properly!
		actuator.SetState(newState);

		//TODO: trigger event 

		if (actuator.m_state.m_atInnerLimit && actuator.m_moveSpeed < 0.0f) {
			actuator.SetActuatorSpeed(0.0f);
			//Debug.Log ("Inner limit detected on actuator " + actuator.GetID ());
		}

		if (actuator.m_state.m_atOuterLimit && actuator.m_moveSpeed > 0.0f) {
			//Debug.Log ("Outer limit detected on actuator " + actuator.GetID ());
			actuator.SetActuatorSpeed(0.0f);
		}

		if (actuator.m_state.m_innerCurrentTripped) {
			Debug.Log ("Inner current tripped on actuator " + actuator.GetID ());
			actuator.SetActuatorSpeed (0.0f);
		}

		if (actuator.m_state.m_outerCurrentTripped) {
			Debug.Log ("Outer current tripped on actuator " + actuator.GetID ());
			actuator.SetActuatorSpeed (0.0f);
		}
			
		//Debug.Log ("Actuator " + currentID + ": " + state.ToString ());
	}

	void ResetEmergencyStop(int actuatorID)
	{
		m_actuators[actuatorID].m_state.ClearTripsAndLimits();	//Clear here just so state is immediately correct and we don't have to wait for a state read
		m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_RESET_ESTOP, (ushort)0x5050);	
	}

	public override void ResetEmergencyStopForAll()
	{
		foreach (Actuator a in m_actuators.Values) {
			//TODO: only reset stopped actuators
			//if (forceAll || a.m_state.m_innerCurrentTripped || a.m_state.m_outerCurrentTripped) 
			{
				int actuatorID = a.GetID ();
				m_modbus.WriteSingleRegister ((byte)actuatorID, (ushort)ModbusRegister.MB_RESET_ESTOP, (ushort)0x5050);	
				m_actuators [actuatorID].m_state.ClearTripsAndLimits ();	//Clear here just so state is immediately correct and we don't have to wait for a state read
			}
		}
	}

}
