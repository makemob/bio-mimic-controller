using System.Collections.Generic;

//static class ModbusData
//{
//	static Dictionary<string, string> s_dictionary = new Dictionary<ModbusRegister, ModbusRegisterData>
//	{
//		{"entry", "entries"},
//		{"image", "images"},
//		{"view", "views"},
//		{"file", "files"},
//		{"result", "results"},
//		{"word", "words"},
//		{"definition", "definitions"},
//		{"item", "items"},
//		{"megabyte", "megabytes"},
//		{"game", "games"}
//	};
//
//
//}
//
//[System.Serializable]
//public struct ModbusRegisterData
//{
//	public string Description;
//	public System.Type DataType;
//	public 
//
//}

enum ModbusRegister
{
	MB_SCARAB_ID0 =	0,
	MB_SCARAB_ID1 =	1,
	MB_SCARAB_ID2 =	2,
	MB_SCARAB_ID3 =	3,
	MB_SCARAB_ID4 =	4,
	MB_SCARAB_ID5 =	5,
	MB_SCARAB_ID6 =	6,
	MB_SCARAB_ID7 =	7,
	MB_SCARAB_ID8 =	8,
	MB_SCARAB_ID9 =	9,
	MB_SCARAB_ID10 = 10,
	MB_SCARAB_ID11 = 11,
	MB_SCARAB_ID12 = 12,
	MB_SCARAB_ID13 = 13,
	MB_SCARAB_ID14 = 14,
	MB_SCARAB_ID15 = 15,

	MB_BOARD_VERSION = 16,
	MB_FW_VERSION_MAJOR = 17,
	MB_FW_VERSION_MINOR = 18,
	MB_MODBUS_ERROR_COUNT =	19,
	MB_UPTIME_MSW = 20,
	MB_UPTIME_LSW = 21,

	MB_BRIDGE_CURRENT =	100,
	MB_BATT_VOLTAGE = 101,
	MB_MAX_BATT_VOLTAGE = 102,
	MB_MIN_BATT_VOLTAGE = 103,
	MB_BOARD_TEMPERATURE = 104,
	MB_EXT_1_ADC = 105,
	MB_EXT_2_ADC = 106,
	MB_EXT_1_DIG = 107,
	MB_EXT_2_DIG = 108,
	MB_EXT_3_DIG = 109,
	MB_EXT_4_DIG = 110,
	MB_EXT_5_DIG = 111,
	MB_EXT_6_DIG = 112,
	MB_BLUE_LED = 113,
	MB_GREEN_LED = 114,

	MB_INWARD_ENDSTOP_STATE	= 115,
	MB_OUTWARD_ENDSTOP_STATE = 116,

	MB_MOTOR_SETPOINT =	200,
	MB_MOTOR_SPEED = 201,
	MB_MOTOR_ACCEL = 202,
	MB_CURRENT_LIMIT_INWARD = 203,
	MB_CURRENT_LIMIT_OUTWARD = 204,
	MB_CURRENT_TRIPS_INWARD = 205,
	MB_CURRENT_TRIPS_OUTWARD = 206,
	MB_VOLTAGE_TRIPS = 207,
	MB_ESTOP = 208,
	MB_RESET_ESTOP = 209,
	MB_MOTOR_PWM_FREQ_MSW =	210,
	MB_MOTOR_PWM_FREQ_LSW =	211,
	MB_MOTOR_PWM_DUTY_MSW =	212,
	MB_MOTOR_PWM_DUTY_LSW =	213,
	MB_INWARD_ENDSTOP_COUNT = 214,
	MB_OUTWARD_ENDSTOP_COUNT = 215,

	MB_UNLOCK_CONFIG = 9000,
	MB_MODBUS_ADDRESS =	9001,
	MB_OPERATING_MODE =	9002,
	MB_OPERATING_CONFIG = 9003,
	MB_DEFAULT_CURRENT_LIMIT_INWARD = 9004,
	MB_DEFAULT_CURRENT_LIMIT_OUTWARD = 9005,
	MB_MAX_CURRENT_LIMIT_INWARD = 9006,
	MB_MAX_CURRENT_LIMIT_OUTWARD = 9007
}