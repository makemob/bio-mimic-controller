using UnityEngine;
using System.Collections;
using System.IO.Ports;
using Modbus.Device;

public class ModbusComms : SerialComms 
{
	public float m_commandDelay = 0.1f;
	private ModbusSerialMaster m_modbusMaster;

	public struct ModbusCommand
	{
		float timeStamp;
		byte slaveAddress;
		short registerAddress;
		short value;
	}

	
	IEnumerator Start () 
	{
		Init();
		yield return new WaitForSeconds(m_commandDelay);
		m_modbusMaster = Modbus.Device.ModbusSerialMaster.CreateRtu(m_serial);
		yield return new WaitForSeconds(m_commandDelay);
		Debug.Log ("ModbusComms ready");
	}

	void Update()
	{

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			MoveActuatorBackward(1);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			MoveActuatorForward(1);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			MoveActuatorBackward(2);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			MoveActuatorForward(2);
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			StopAll();
		}
		//m_serial.
	}
	
	public void StopActuator (int actuatorID) 
	{
		Debug.Log ("StopActuator called");
		if (m_modbusMaster != null)
			m_modbusMaster.WriteSingleRegister((byte)actuatorID,0,0);
	}

	public IEnumerator MoveForwardImpl (int actuatorID) 
	{
		Debug.Log ("MoveActuatorForward called");

		if (m_modbusMaster != null)
			m_modbusMaster.WriteSingleRegister((byte)actuatorID,0,1);

		yield return new WaitForSeconds(0.1f);


		Debug.Log ("MoveActuatorForward actuator direction change");

		if (m_modbusMaster != null)
			m_modbusMaster.WriteSingleRegister((byte)actuatorID,1,255);
		//ushort[] data = new ushort[] {1,255};
		//m_modbusMaster.WriteMultipleRegisters((byte)actuatorID, 0, data);

		Debug.Log ("MoveActuatorForward called");

	}

	public void MoveActuatorBackward (int actuatorID) 
	{
		StartCoroutine(MoveBackImpl(actuatorID));
	}

	public void MoveActuatorForward (int actuatorID) 
	{
		StartCoroutine(MoveForwardImpl(actuatorID));
	}


	private IEnumerator MoveBackImpl(int actuatorID)
	{
		Debug.Log ("MoveActuatorBackward called");
		if (m_modbusMaster != null)
			m_modbusMaster.WriteSingleRegister((byte)actuatorID,0,2);

		Debug.Log ("MoveActuatorBackward actuator direction change");

		yield return new WaitForSeconds(0.1f);

		if (m_modbusMaster != null)
			m_modbusMaster.WriteSingleRegister((byte)actuatorID,1,255);
		//ushort[] data = new ushort[] {2,255};
		//m_modbusMaster.WriteMultipleRegisters((byte)actuatorID, 0, data);
		Debug.Log ("MoveActuatorBackward called");

	}



	public void StopAll()
	{
		//StopActuator(1);
		//StopActuator(2);
		StartCoroutine("StopAllImpl");
	}

	public IEnumerator StopAllImpl()
	{
		StopActuator(1);
		yield return new WaitForSeconds(0.1f);
		StopActuator(2);
	}


	private IEnumerator Coroutine_WriteSingleRegister(byte slaveAdress, ushort registerAddress, ushort value) 
	{
		yield return new WaitForSeconds(0.1f);
		//m_modbusMaster.WriteSingleRegister((byte)actuatorID,1,255);
		//m_modbusMaster.Writ
	}

}
