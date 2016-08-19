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

	private ModbusComms m_modbus;
	private List<Actuator> m_actuators = new List<Actuator>();

	void Start()
	{
		m_modbus = GetComponent<ModbusComms>();
	}

	public override void Startup()
	{
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

		ushort direction = 0;
		ushort speed = 0;

		GetSpeedAndDirection (normalisedSpeed, out speed, out direction);

		ushort[] data = new ushort[] { direction, speed };

		m_modbus.WriteMultipleRegisters ((byte)actuatorID, 0, data);
	}

	public override void StopActuator(int actuatorID)
	{
		m_actuators[actuatorID].SetActuatorSpeed(0.0f);

		m_modbus.WriteSingleRegister ((byte)actuatorID, 0, 0);
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
