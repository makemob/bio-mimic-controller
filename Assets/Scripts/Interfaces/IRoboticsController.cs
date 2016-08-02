using UnityEngine;
using System.Collections;

namespace UKI 
{	
	public interface IRoboticsController 
	{
		void Startup();

		void Shutdown();

		void SetActuatorSpeed (int actuatorID, float speed);

		void StopActuator (int actuatorID);

		void StopAllActuators ();

		void Stop();
	}
}
