using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

[System.Serializable]
public struct ActuatorState 
{
	//TODO: Constructor
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

	public override string ToString()
	{
		string output = "m_errorCount: " + m_errorCount +			
			"\nm_bridgeCurrent: " + m_bridgeCurrent +
			"\nm_batteryVoltage: " + m_batteryVoltage +
			"\nm_boardTemperature: " + m_boardTemperature +
			"\nm_motorSetPoint: " + m_motorSetPoint +
			"\nm_motorSpeed: " + m_motorSpeed +
			"\nm_motorAcceleration: " + m_motorAcceleration +
			"\nm_innerLimit: " + m_innerLimit +
			"\nm_outerLimit: " +  m_outerLimit +
			"\nm_innerTrips: " + m_innerTrips +
			"\nm_outerTrips: " + m_outerTrips + 
			"\nm_voltageTrips: " + m_voltageTrips;

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
