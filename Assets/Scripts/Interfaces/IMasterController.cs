using UnityEngine;
using System.Collections;

namespace UKI 
{	
	public interface IMasterController 
	{
		void Startup();

		void Shutdown();

		void TestSequence ();

		void StopRobotics ();

		void StopLighting ();
	}
}
