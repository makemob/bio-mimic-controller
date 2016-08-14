using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using Modbus.Device;
using UKI;

public class ModbusComms : SerialComms, IRoboticsController
{
	//Register values from scarab
	const ushort STILL = 0;
	const ushort FORWARDS = 1;
	const ushort BACKWARDS = 2;
	const int MAX_ACTUATORS = 16;

	private ModbusSerialMaster m_modbusMaster;
	private CommandQueue m_commandQueue;

	//
	// Startup the modbus connection and commence the command queue, enforcing gaps between all networking calls
	//
	public override void Startup () 
	{
		Debug.Log ("ModbusComms.Startup called.");

		base.Startup();

		m_modbusMaster = Modbus.Device.ModbusSerialMaster.CreateRtu(m_serial);
		if (!m_commandQueue)
			m_commandQueue = gameObject.AddComponent<CommandQueue> ();
		m_commandQueue.Run();

		Debug.Log ("ModbusComms ready");
	}

	//
	// Shutdown the modbus connection and stop processing commands
	//
	public override void Shutdown ()
	{
		Debug.Log ("ModbusComms.Shutdown called.");

		m_commandQueue.Stop ();
		m_commandQueue.Clear();

		//TODO: Send a final stop message??
		m_modbusMaster.Dispose ();
	}

	//
	// Queue up a command to set the speed of a given actuator. Fires off the message to modbus without bounds checking.
	// _actuatorID is the id of given actuator. _speed is normalised speed from -1.0 to 1.0f.
	//
	public void SetActuatorSpeed (int _actuatorID, float _speed)
	{
		QueueInternalCommand (() => {
			ushort direction = 0;
			ushort speed = 0;

			GetSpeedAndDirection (_speed, out speed, out direction);

			ushort[] data = new ushort[] { direction, speed };
			if (m_modbusMaster != null)
				m_modbusMaster.WriteMultipleRegisters ((byte)_actuatorID, 0, data);
		});
	}

	//
	// Queue up a command to stop a given acuator 
	//
	public void StopActuator (int actuatorID) 
	{
		Debug.Log ("StopActuator called");

		QueueInternalCommand (() => {
			if (m_modbusMaster != null)
				m_modbusMaster.WriteSingleRegister ((byte)actuatorID, 0, 0);
		});
	}

	//
	// Queue up a list of commands to stop all actuators. TODO: investigate a single command version of this.
	//
	public void StopAllActuators ()
	{
		for(var i=0; i < MAX_ACTUATORS; i++)
			StopActuator(i);	
	}

	//
	// Stop everything!
	//
	public void Stop ()
	{
		StopAllActuators();
	}

	//
	// Helper to add a new generic command to the command queue
	//
	private void QueueInternalCommand (Action a)
	{
		if (m_commandQueue && m_commandQueue.enabled)
			m_commandQueue.Add(new Command_GenericAction(a));
	}

	//
	// Helper function to return the speed and direction from a given normalised speed (-1.0f to 1.0f)
	//
	private void GetSpeedAndDirection (float _normalisedSpeed, out ushort _speed, out ushort _direction)
	{
		_speed =  Convert.ToUInt16(Math.Abs(_normalisedSpeed) * 255.0f);

		_direction = STILL;				//still by default
		if (_normalisedSpeed > 0.0f)			
			_direction = FORWARDS;		//forwards
		else if (_normalisedSpeed > 0.0f)
			_direction = BACKWARDS;		//backwards
	}
}
