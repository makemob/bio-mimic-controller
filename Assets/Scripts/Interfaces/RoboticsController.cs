using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UKI 
{	
	public abstract class RoboticsController : MonoBehaviour 
	{
		public abstract void Startup();

		public abstract void Shutdown();

		public abstract bool RegisterActuator(Actuator actuator);

		public abstract void MoveActuatorTowardsPosition (int actuatorID, float position);

		public abstract bool CloseEnoughToPosition (int actuatorID, float position);

		public abstract void SetActuatorAcceleration(int actuatorID, float acceleration);

		public abstract void SetActuatorSpeed (int actuatorID, float speed);

		public abstract void SetActuatorCallibration (int actuatorID, CallibrationResults results);

		public abstract void SetAllActuatorSpeeds (float speed);

		public abstract void SetActuatorSpeeds (List<int> ids, float normalisedSpeed);

		//public abstract void SetAllActuatorSpeeds (List<float> speeds);

		public abstract void StopActuator (int actuatorID);

		public abstract void StopAllActuators ();

		public abstract void Stop();

		public abstract ActuatorState ReadActuatorState (int actuatorID);

		public abstract ActuatorState GetActuatorState (int actuatorID);

		public abstract ActuatorState [] GetAllActuatorStates ();

		public abstract void UpdateAllActuatorStates();

		public abstract void ResetEmergencyStopForAll ();

	}
}
