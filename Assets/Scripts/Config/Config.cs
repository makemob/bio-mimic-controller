using UnityEngine;
using System.Collections;

namespace UKI
{
	[System.Serializable]
	public class Config 
	{
		public int version;
		public ActuatorConfig[] actuators;

		public bool FindActuator(string _name, ref ActuatorConfig _config)
		{
			foreach(ActuatorConfig a in actuators)
			{
				if (a.name == _name) 
				{
					_config = a;
					return true;
				}
			}
			return false;
		}
	}

	[System.Serializable]
	public class ActuatorConfig
	{
		public string name;
		public int id;
		public float minExtent;
		public float maxExtent;
		public float inwardCurrentLimit;
		public float outwardCurrentLimit;
	}


}
