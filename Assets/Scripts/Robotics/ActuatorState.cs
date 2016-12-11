using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

[System.Serializable]
public struct ActuatorState 
{
	//TODO: Constructor

	//Diagnostics
	public int m_errorCount;
	public int m_bridgeCurrent;
	public int m_batteryVoltage;
	public int m_boardTemperature;

	//Speeds
	public int m_motorSetPoint;	//Target speed
	public int m_motorSpeed;	//Actual speed
	public int m_motorAcceleration;

	//Current trips
	public int m_innerCurrentLimit;
	public int m_outerCurrentLimit;
	public bool m_innerCurrentTripped;
	public bool m_outerCurrentTripped;
	public int m_innerCurrentTrips;	//Number of current trips in reverse since boot
	public int m_outerCurrentTrips;	//Number of current trips in forward since boot

	//Voltage trips
	public int m_voltageTrips;	//Number of voltage limit trips since boot

	public int m_stopped;

	//Limit microswitches
	public bool m_atInnerLimit;
	public bool m_atOuterLimit;
	public int m_innerLimitCount;
	public int m_outerLimitCount;

	public int m_heartBeat;

	//Prediction
	public float m_predictedExtension;

	public void ClearTripsAndLimits()
	{
		m_innerCurrentTripped = false;
		m_outerCurrentTripped = false;
		m_atInnerLimit = false;
		m_atOuterLimit = false;
		m_heartBeat = 0;
	}

	public override string ToString()
	{
		int innerTrip = m_innerCurrentTripped ? 1 : 0;
		int outerTrip = m_outerCurrentTripped ? 1 : 0;

		string output = 
			//"errorCount: " + m_errorCount +			
			"\nbridgeCurrent: " + m_bridgeCurrent +
			"\nbatteryVoltage: " + m_batteryVoltage +
			//"\nboardTemperature: " + m_boardTemperature +
			//"\nmotorSetPoint: " + m_motorSetPoint +
			"\nmotorSpeed: " + m_motorSpeed +
			"\nmotorAcceleration: " + m_motorAcceleration +
			"\ninnerCurrentLimit: " + m_innerCurrentLimit +
			"\nouterCurrentLimit: " + m_outerCurrentLimit +
			"\ninnerMicroswitchCount: " + m_innerLimitCount +
			"\nouterMicroswitchCount: " + m_outerLimitCount +
			"\ninnerTrips: " + m_innerCurrentTrips +
			"\nouterTrips: " + m_outerCurrentTrips + 
			"\nemergencyStop: " + m_stopped +
			"\nheartBeat: " + m_heartBeat +
			"\ncurrentTripInnerNow: " + innerTrip +
			"\ncurrentTripOuterNow: " + outerTrip +
			"\nextension (mm): " + (int)m_predictedExtension;

		return output;

	}

//	public override string ToString()
//	{
//		Type type = this.GetType();
//		FieldInfo[] fields = type.GetFields();
//		PropertyInfo[] properties = type.GetProperties();
//		ActuatorState state = this;
//
//		string output = "";
//		Dictionary<string, object> values = new Dictionary<string, object>();
//		Array.ForEach(fields, (field) => values.Add(field.Name, field.GetValue(state)));
//		Array.ForEach(properties, (property) =>
//			{
//				//if (property.CanRead)
//				{
//					values.Add(property.Name, property.GetValue(state, null));
//					output = output + property.Name +  ": " + property.GetValue(state, null) + "\n";
//				}
//			});
//
//		return output;
//	}
}
