using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UKI;

enum ModbusSlaveMap
{
	MB_SCARAB_ID1 = 0,
	MB_SCARAB_ID2 = 1,
	MB_SCARAB_ID3 = 2,
	MB_SCARAB_ID4 = 3,
	MB_SCARAB_ID5 = 4,
	MB_SCARAB_ID6 = 5,
	MB_SCARAB_ID7 = 6,
	MB_SCARAB_ID8 = 7,
	MB_SCARAB_ID9 = 8,
	MB_SCARAB_ID10 = 9,
	MB_SCARAB_ID11 = 10,
	MB_SCARAB_ID12 = 11,
	MB_SCARAB_ID13 = 12,
	MB_SCARAB_ID14 = 13,
	MB_SCARAB_ID15 = 14,
	MB_SCARAB_ID16 = 15,
}

enum ModbusRegisterMap
{
	//MB_SCARAB_ID1 = 0,
	// ...
	//MB_SCARAB_ID15 = 15,
	MB_BOARD_VERSION,
	MB_FW_VERSION_MAJOR,
	MB_FW_VERSION_MINOR,
	MB_MODBUS_ERROR_COUNT,
	MB_UPTIME_MSW,
	MB_UPTIME_LSW,

	MB_BRIDGE_CURRENT = 100,
	MB_BATT_VOLTAGE,
	MB_MAX_BATT_VOLTAGE,
	MB_MIN_BATT_VOLTAGE,
	MB_BOARD_TEMPERATURE,
	MB_EXT_1_ADC,
	MB_EXT_2_ADC,
	MB_EXT_1_DIG,
	MB_EXT_2_DIG,
	MB_EXT_3_DIG,
	MB_EXT_4_DIG,
	MB_EXT_5_DIG,
	MB_EXT_6_DIG,
	MB_BLUE_LED,
	MB_GREEN_LED,

	MB_MOTOR_SETPOINT = 200,
	MB_MOTOR_SPEED,
	MB_MOTOR_ACCEL,
	MB_CURRENT_LIMIT_INWARD,
	MB_CURRENT_LIMIT_OUTWARD,
	MB_CURRENT_TRIPS_INWARD,
	MB_CURRENT_TRIPS_OUTWARD,
	MB_VOLTAGE_TRIPS,
	MB_ESTOP,
	MB_RESET_ESTOP,    // Write 0x5050 to reset emergency stop
	MB_MOTOR_PWM_FREQ_MSW,
	MB_MOTOR_PWM_FREQ_LSW,
	MB_MOTOR_PWM_DUTY_MSW,
	MB_MOTOR_PWM_DUTY_LSW,

	// Position info etc. = 300


	MB_UNLOCK_CONFIG = 9000,    // Write 0xA0A0 to unlock regs, anything else to lock
	MB_MODBUS_ADDRESS,
	MB_OPERATING_MODE,   // eg. Limit switches, encoders
	MB_OPERATING_CONFIG, // specific config for the selected mode
	MB_DEFAULT_CURRENT_LIMIT_INWARD,
	MB_DEFAULT_CURRENT_LIMIT_OUTWARD,
	MB_MAX_CURRENT_LIMIT_INWARD,
	MB_MAX_CURRENT_LIMIT_OUTWARD,

	NUM_MODBUS_REGS
};

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

		ushort direction = 0;
		ushort speed = 0;

		GetSpeedAndDirection (normalisedSpeed, out speed, out direction);

		ushort[] data = new ushort[] { direction, speed };

		if (m_useMultiRegister) 
		{
			m_modbus.WriteMultipleRegisters ((byte)actuatorID, 0, data);
		} 
		else 
		{
			m_modbus.WriteSingleRegister ((byte)actuatorID, 0, direction);
			m_modbus.WriteSingleRegister ((byte)actuatorID, 1, speed);
		}

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
