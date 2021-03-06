﻿using UnityEngine;
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
	public bool m_logOutput = false;
	public bool m_logReadTime = false;
	//Register values from scarab
	const ushort STILL = 0;
	const ushort FORWARDS = 1;
	const ushort BACKWARDS = 2;
	const int MAX_ACTUATORS = 16;

	private ModbusSerialMaster m_modbusMaster;
	//private CommandQueue m_commandQueue;
	private CommandQueueThreaded m_commandQueue;
	//private CommandQueueThreadPool m_commandQueue;
	private DateTime m_startTime;
	private bool m_running = false;

	public struct ReadResults
	{
		public byte slaveID;
		public int startAddress;
		public ushort [] data;
	}

	object m_readLock = new object();
	Queue<ReadResults> m_readResults = new Queue<ReadResults> (32);

	//
	// Startup the modbus connection and commence the command queue, enforcing gaps between all networking calls
	//
	public override void Startup () 
	{
		Debug.Log ("ModbusComms.Startup called.");

		base.Startup();

		StartClock ();

		try 
		{
			m_modbusMaster = Modbus.Device.ModbusSerialMaster.CreateRtu(m_serial);
		}
		catch (Exception e) 
		{
			Debug.LogError (e);
		}

		//if (!m_commandQueue)
		//	m_commandQueue = gameObject.AddComponent<CommandQueue> ();
		if (!m_commandQueue)
			m_commandQueue = gameObject.AddComponent<CommandQueueThreaded> ();
		//if (!m_commandQueue)
		//	m_commandQueue = gameObject.AddComponent<CommandQueueThreadPool> ();

		Debug.Log("Minimum interval is " + GetMinimumInterval(m_baudRate));

		Debug.Log ("ModbusComms ready");

		m_running = true;
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
		try 
		{			
			m_modbusMaster.Dispose ();
		}
		catch (Exception e) 
		{
			Debug.LogError (e);
		}

		m_running = false;
	}

	public void WriteSingleRegister(byte slaveID, ushort register, ushort data)
	{
		QueueInternalCommand (() => {

			if (m_logOutput)
				Debug.Log(GetClock() + "Write ModbusSingleRegister. SlaveID: " + slaveID + " Register: " + register + " Data:" + data);

			try 
			{
				if (m_modbusMaster != null && m_serial.IsOpen)
					m_modbusMaster.WriteSingleRegister(slaveID, register, data);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

		});

	}

	public void WriteMultipleRegisters(byte slaveID, ushort startRegister, ushort [] data)
	{
		QueueInternalCommand (() => {

			string dataString = "[";
			foreach(ushort u in data)
				dataString += u.ToString() + " ";
			dataString += "]";
			Debug.Log(GetClock() + " ModbusMultiRegister. SlaveID: " + slaveID + " Register: " + startRegister + " Data:" + dataString);

			try 
			{
				if (m_modbusMaster != null && m_serial.IsOpen)
					m_modbusMaster.WriteMultipleRegisters(slaveID, startRegister, data);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}		

		});
	}

	public void AddReadData(ReadResults data)
	{
		lock (m_readLock) 
		{
			m_readResults.Enqueue(data);
		}
	}

	public bool HasReadData()
	{
		bool hasReadData = false;

		lock (m_readLock) 
		{
			hasReadData = m_readResults.Count > 0;
		}

		return hasReadData;
	}

	public ReadResults GetReadData()
	{
		ReadResults r;

		lock (m_readLock) {
			r = m_readResults.Dequeue ();
		}

		return r;
	}

	public void ReadHoldingRegisters(byte slaveID, ushort startRegister, ushort numRegistersToRead)
	{
		QueueInternalCommand (() => {
			
			if (m_logOutput)
				Debug.Log (" Reading Holding Register. SlaveID: " + slaveID + " StartRegister: " + startRegister + " Count:" + numRegistersToRead);

			ushort [] result = null;

			try 
			{
				if (m_modbusMaster != null && m_serial.IsOpen) 
				{
					float timeA = GetClockMS ();

					result = m_modbusMaster.ReadHoldingRegisters (slaveID, startRegister, numRegistersToRead);

					float timeB = GetClockMS ();

					if (m_logReadTime)
						Debug.Log ("ReadTime " + slaveID + ": " + (timeB - timeA).ToString ());
				}
			} 
			catch (Exception e) 
			{
				Debug.Log ("Failed to read holding register.");
				Debug.LogError (e);
			}
		
			if (result != null)
			{
				ReadResults r = new ReadResults();
				r.slaveID = slaveID;
				r.startAddress = startRegister;
				r.data = result;

				AddReadData(r);
			}
		});
	}

	public void ClearCommandQueue()
	{
		if (m_commandQueue)
			m_commandQueue.Clear();
	}

	void Update()
	{
		if (!m_running)
			return;
		
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

	private void StartClock()
	{
		m_startTime = System.DateTime.UtcNow;
	}

	private float GetClock()
	{
		System.TimeSpan t = System.DateTime.UtcNow - m_startTime;
		return (float)(t.TotalMilliseconds/1000.0);
	}

	private float GetClockMS()
	{
		System.TimeSpan t = System.DateTime.UtcNow - m_startTime;
		return (float)(t.TotalMilliseconds);
	}
}
