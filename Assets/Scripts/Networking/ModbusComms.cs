using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using Modbus.Device;
using UKI;

public class ModbusComms : SerialComms
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

		m_commandQueue.Stop();
		m_commandQueue.Clear();

		//TODO: Send a final stop message??
		m_modbusMaster.Dispose ();
	}

	public void WriteSingleRegister(byte address, ushort register, ushort data)
	{
		QueueInternalCommand (() => {
			if (m_modbusMaster != null && m_serial.IsOpen)
				m_modbusMaster.WriteSingleRegister(address, register, data);
			Debug.Log(Time.realtimeSinceStartup + " ModbusSingleRegister. Address: " + address + " Register: " + register + " Data:" + data);
		});
	}

	public void WriteMultipleRegisters(byte address, ushort startRegister, ushort [] data)
	{
		QueueInternalCommand (() => {
			if (m_modbusMaster != null && m_serial.IsOpen)
				m_modbusMaster.WriteMultipleRegisters(address, startRegister, data);
			string dataString = "[";
			foreach(ushort u in data)
				dataString += u.ToString() + " ";
			dataString += "]";
			
			Debug.Log(Time.realtimeSinceStartup + " ModbusMultiRegister. Address: " + address + " Register: " + startRegister + " Data:" + dataString);

		});
	}

	public void ClearCommandQueue()
	{
		if (m_commandQueue)
			m_commandQueue.Clear();
	}

	//
	// Helper to add a new generic command to the command queue
	//
	private void QueueInternalCommand (Action a)
	{
		if (m_commandQueue && m_commandQueue.enabled)
			m_commandQueue.Add(new Command_GenericAction(a));
	}

}
