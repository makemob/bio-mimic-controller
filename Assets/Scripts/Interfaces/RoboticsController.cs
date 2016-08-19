using UnityEngine;
using System.Collections;

namespace UKI 
{	
	public abstract class RoboticsController : MonoBehaviour 
	{
		public abstract void Startup();

		public abstract void Shutdown();

		public abstract void RegisterActuator(Actuator actuator);

		public abstract void SetActuatorSpeed (int actuatorID, float speed);

		public abstract void StopActuator (int actuatorID);

		public abstract void StopAllActuators ();

		public abstract void Stop();
	}
}
