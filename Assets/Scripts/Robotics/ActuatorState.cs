using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ActuatorState 
{
	public int m_errorCount;
	public int m_bridgeCurrent;
	public int m_batteryVoltage;
	public int m_boardTemperature;
	public int m_motorSetPoint;	//Target speed
	public int m_motorSpeed;	//Actual speed
	public int m_motorAcceleration;
	public int m_innerLimit;
	public int m_outerLimit;
	public int m_innerTrips;	//Number of current trips in reverse since boot
	public int m_outerTrips;	//Number of current trips in forward since boot
	public int m_voltageTrips;	//Number of voltage limit trips since boot
}
