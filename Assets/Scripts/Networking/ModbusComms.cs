using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using Modbus.Device;
using UKI;

public class ModbusComms : SerialComms
{
	//TODO: Import register map

	//Transport values. See documentation for details.
	public int m_readTimeout = 200;		//milliseconds
	public int m_writeTimeout = 200;	//milliseconds
	public int m_waitToRetry = 200;		//milliseconds
	public int m_retries = 3;			//max num retries

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

		Debug.Log("Minimum interval is " + GetMinimumInterval(m_baudRate));

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

	public void WriteSingleRegister(byte slaveID, ushort register, ushort data)
	{
		QueueInternalCommand (() => {
			if (m_modbusMaster != null && m_serial.IsOpen)
				m_modbusMaster.WriteSingleRegister(slaveID, register, data);
			Debug.Log(Time.realtimeSinceStartup + " ModbusSingleRegister. SlaveID: " + slaveID + " Register: " + register + " Data:" + data);
		});

	}

	public void WriteMultipleRegisters(byte slaveID, ushort startRegister, ushort [] data)
	{
		QueueInternalCommand (() => {
			if (m_modbusMaster != null && m_serial.IsOpen)
				m_modbusMaster.WriteMultipleRegisters(slaveID, startRegister, data);
			string dataString = "[";
			foreach(ushort u in data)
				dataString += u.ToString() + " ";
			dataString += "]";
			
			Debug.Log(Time.realtimeSinceStartup + " ModbusMultiRegister. SlaveID: " + slaveID + " Register: " + startRegister + " Data:" + dataString);

		});
	}

	public ushort [] ReadHoldingRegisters(byte slaveID, ushort startRegister, ushort numRegistersToRead)
	{
		if (m_modbusMaster != null && m_serial.IsOpen)
			return m_modbusMaster.ReadHoldingRegisters(slaveID, startRegister, numRegistersToRead);

		return null;
	}

	public void ClearCommandQueue()
	{
		if (m_commandQueue)
			m_commandQueue.Clear();
	}

	void Update()
	{
		if (m_readTimeout != m_modbusMaster.Transport.ReadTimeout)
			m_modbusMaster.Transport.ReadTimeout = m_readTimeout;
		
		if (m_writeTimeout != m_modbusMaster.Transport.WriteTimeout)
			m_modbusMaster.Transport.WriteTimeout = m_writeTimeout;

		if (m_waitToRetry != m_modbusMaster.Transport.WaitToRetryMilliseconds)
			m_modbusMaster.Transport.WaitToRetryMilliseconds = m_waitToRetry;

		if (m_retries != m_modbusMaster.Transport.Retries)			
			m_modbusMaster.Transport.Retries = m_retries;
	}

	//
	// Helper to add a new generic command to the command queue
	//
	private void QueueInternalCommand (Action a)
	{
		if (m_commandQueue && m_commandQueue.enabled)
			m_commandQueue.Add(new Command_GenericAction(a));
	}


	public static double GetMinimumInterval(int baud)
	{
		const int bitsPerCharacter = 11;	//Modbus rtu is 11-bits per character
		const double minimumIntervalMultiplier = 3.5; 
		double timePerBit = 1.0 / (double)baud;
		double timePerCharacter = timePerBit * bitsPerCharacter;
		double minimumInterval = timePerCharacter * minimumIntervalMultiplier;

		return minimumInterval;
	}

}
